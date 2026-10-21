
using System;
using System.Security.Cryptography;

public static class AspNetIdentityPasswordVerifier
{
    public static bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) ||
            providedPassword == null)
        {
            return false;
        }

        byte[] decoded;

        try
        {
            decoded = Convert.FromBase64String(hashedPassword);
        }
        catch (FormatException)
        {
            return false;
        }

        // ASP.NET Core Identity V3 format:
        // 1 byte format marker
        // 4 bytes PRF
        // 4 bytes iteration count
        // 4 bytes salt length
        // salt + subkey
        if (decoded.Length < 13 || decoded[0] != 0x01)
            return false;

        uint prf = ReadUInt32BigEndian(decoded, 1);
        uint iterations = ReadUInt32BigEndian(decoded, 5);
        uint saltLength = ReadUInt32BigEndian(decoded, 9);

        if (iterations == 0 ||
            saltLength < 16 ||
            saltLength > (uint)(decoded.Length - 13))
        {
            return false;
        }

        int saltOffset = 13;
        int subkeyOffset = saltOffset + (int)saltLength;
        int subkeyLength = decoded.Length - subkeyOffset;

        if (subkeyLength < 16)
            return false;

        HashAlgorithmName algorithm;

        switch (prf)
        {
            case 0:
                algorithm = HashAlgorithmName.SHA1;
                break;
            case 1:
                algorithm = HashAlgorithmName.SHA256;
                break;
            case 2:
                algorithm = HashAlgorithmName.SHA512;
                break;
            default:
                return false;
        }

        try
        {
            byte[] salt = new byte[(int)saltLength];
            Buffer.BlockCopy(decoded, saltOffset, salt, 0, salt.Length);

            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(
                decoded, subkeyOffset, expectedSubkey, 0, subkeyLength);

            using (var deriveBytes = new Rfc2898DeriveBytes(
                providedPassword,
                salt,
                checked((int)iterations),
                algorithm))
            {
                byte[] actualSubkey = deriveBytes.GetBytes(subkeyLength);

                if (actualSubkey.Length != expectedSubkey.Length)
                    return false;

                int difference = 0;

                for (int i = 0; i < expectedSubkey.Length; i++)
                {
                    difference |= actualSubkey[i] ^ expectedSubkey[i];
                }

                return difference == 0;
            }
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private static uint ReadUInt32BigEndian(byte[] data, int offset)
    {
        return ((uint)data[offset] << 24)
             | ((uint)data[offset + 1] << 16)
             | ((uint)data[offset + 2] << 8)
             | data[offset + 3];
    }
}
