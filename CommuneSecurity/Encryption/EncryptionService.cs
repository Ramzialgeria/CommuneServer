using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommuneSecurity.Encryption
{
    public static class EncryptionService
    {
        private const byte CurrentVersion = 1;

        private const int KeySize = 32;      // AES-256
        private const int IvSize = 16;       // AES block size
        private const int HmacSize = 32;     // SHA-256

        /*
         * Encrypt a UTF-8 string and return Base64.
         */
        public static string Encrypt(string plainText)
        {
            if (plainText == null)
                return null;

            byte[] plainBytes =
                Encoding.UTF8.GetBytes(plainText);

            byte[] encrypted =
                EncryptBytes(plainBytes);

            return Convert.ToBase64String(encrypted);
        }

        /*
         * Decrypt a Base64 string.
         */
        public static string Decrypt(string encryptedText)
        {
            if (encryptedText == null)
                return null;

            byte[] encryptedBytes;

            try
            {
                encryptedBytes =
                    Convert.FromBase64String(encryptedText);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException(
                    "Encrypted value is not valid Base64.",
                    ex);
            }

            byte[] plainBytes =
                DecryptBytes(encryptedBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        /*
         * Encrypt arbitrary bytes.
         */
        public static byte[] EncryptBytes(byte[] plainBytes)
        {
            if (plainBytes == null)
                return null;

            byte[] key =
                EncryptionKeyProvider.GetKey();

            byte[] iv = new byte[IvSize];

            using (var rng =
                RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            byte[] cipherText;

            using (var aes =
                new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor =
                    aes.CreateEncryptor())
                {
                    cipherText =
                        encryptor.TransformFinalBlock(
                            plainBytes,
                            0,
                            plainBytes.Length);
                }
            }

            /*
             * Authenticate:
             *
             * Version + IV + CipherText
             *
             * This protects against tampering.
             */
            byte[] authenticatedData =
                BuildAuthenticatedData(
                    CurrentVersion,
                    iv,
                    cipherText);

            byte[] hmac =
                ComputeHmac(key, authenticatedData);

            /*
             * Final format:
             *
             * [Version]
             * [IV]
             * [CipherText]
             * [HMAC]
             */
            using (var ms =
                new MemoryStream())
            {
                ms.WriteByte(CurrentVersion);

                ms.Write(
                    iv,
                    0,
                    iv.Length);

                ms.Write(
                    cipherText,
                    0,
                    cipherText.Length);

                ms.Write(
                    hmac,
                    0,
                    hmac.Length);

                return ms.ToArray();
            }
        }

        /*
         * Decrypt arbitrary bytes.
         */
        public static byte[] DecryptBytes(
            byte[] encryptedBytes)
        {
            if (encryptedBytes == null)
                return null;

            if (encryptedBytes.Length <
                1 + IvSize + HmacSize)
            {
                throw new CryptographicException(
                    "Encrypted data is too short.");
            }

            byte version =
                encryptedBytes[0];

            if (version != CurrentVersion)
            {
                throw new CryptographicException(
                    "Unsupported encryption version: " +
                    version);
            }

            byte[] iv =
                new byte[IvSize];

            Buffer.BlockCopy(
                encryptedBytes,
                1,
                iv,
                0,
                IvSize);

            int cipherTextLength =
                encryptedBytes.Length -
                1 -
                IvSize -
                HmacSize;

            if (cipherTextLength <= 0)
            {
                throw new CryptographicException(
                    "Encrypted data does not contain ciphertext.");
            }

            byte[] cipherText =
                new byte[cipherTextLength];

            Buffer.BlockCopy(
                encryptedBytes,
                1 + IvSize,
                cipherText,
                0,
                cipherTextLength);

            byte[] storedHmac =
                new byte[HmacSize];

            Buffer.BlockCopy(
                encryptedBytes,
                1 + IvSize + cipherTextLength,
                storedHmac,
                0,
                HmacSize);

            byte[] key =
                EncryptionKeyProvider.GetKey();

            byte[] authenticatedData =
                BuildAuthenticatedData(
                    version,
                    iv,
                    cipherText);

            byte[] calculatedHmac =
                ComputeHmac(
                    key,
                    authenticatedData);

            /*
             * IMPORTANT:
             * Verify HMAC BEFORE decrypting.
             */
            if (!FixedTimeEquals(
                    storedHmac,
                    calculatedHmac))
            {
                throw new CryptographicException(
                    "Encrypted data authentication failed.");
            }

            using (var aes =
                new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor =
                    aes.CreateDecryptor())
                {
                    try
                    {
                        return decryptor.TransformFinalBlock(
                            cipherText,
                            0,
                            cipherText.Length);
                    }
                    catch (CryptographicException ex)
                    {
                        throw new CryptographicException(
                            "Unable to decrypt the value.",
                            ex);
                    }
                }
            }
        }

        private static byte[] BuildAuthenticatedData(
            byte version,
            byte[] iv,
            byte[] cipherText)
        {
            using (var ms =
                new MemoryStream())
            {
                ms.WriteByte(version);

                ms.Write(
                    iv,
                    0,
                    iv.Length);

                ms.Write(
                    cipherText,
                    0,
                    cipherText.Length);

                return ms.ToArray();
            }
        }

        private static byte[] ComputeHmac(
            byte[] key,
            byte[] data)
        {
            using (var hmac =
                new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }

        private static bool FixedTimeEquals(
            byte[] left,
            byte[] right)
        {
            if (left == null || right == null)
                return false;

            if (left.Length != right.Length)
                return false;

            int difference = 0;

            for (int i = 0;
                 i < left.Length;
                 i++)
            {
                difference |=
                    left[i] ^ right[i];
            }

            return difference == 0;
        }

        /*
         * Convenience methods for common data types.
         */

        public static string EncryptInt64(long value)
        {
            return Encrypt(value.ToString(
                System.Globalization.CultureInfo.InvariantCulture));
        }

        public static long DecryptInt64(string encrypted)
        {
            string value = Decrypt(encrypted);

            return long.Parse(
                value,
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string EncryptInt32(int value)
        {
            return Encrypt(value.ToString(
                System.Globalization.CultureInfo.InvariantCulture));
        }

        public static int DecryptInt32(string encrypted)
        {
            string value = Decrypt(encrypted);

            return int.Parse(
                value,
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string EncryptBoolean(bool value)
        {
            return Encrypt(value ? "1" : "0");
        }

        public static bool DecryptBoolean(string encrypted)
        {
            return Decrypt(encrypted) == "1";
        }

        public static string EncryptDateTime(
            DateTime value)
        {
            return Encrypt(
                value.ToUniversalTime()
                    .ToString("O"));
        }

        public static DateTime DecryptDateTime(
            string encrypted)
        {
            return DateTime.Parse(
                Decrypt(encrypted),
                null,
                System.Globalization.DateTimeStyles.RoundtripKind);
        }

        public static string EncryptNullableDateTime(
            DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return EncryptDateTime(value.Value);
        }

        public static DateTime? DecryptNullableDateTime(
            string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return null;

            return DecryptDateTime(encrypted);
        }
    }
}