using System;
using System.Security.Cryptography;
using System.Text;

namespace CommuneSecurity.Activation
{
    public class ActivationFileException : Exception
    {
        public ActivationFileException(string message) : base(message) { }
    }

    /// <summary>
    /// Opens an Activation.enc file produced by central.
    ///
    /// File layout (integers are big-endian):
    ///
    ///   4   bytes  magic "CMAF"
    ///   1   byte   version (1)
    ///   4   bytes  PBKDF2 iterations
    ///   16  bytes  salt
    ///   16  bytes  AES IV
    ///   4   bytes  cipher text length
    ///   N   bytes  cipher text  = AES-256-CBC( "CMAP" + JSON utf-8 )
    ///   rest       RSA-2048 / SHA-256 / PKCS#1 v1.5 signature of everything above
    ///
    /// AES key = PBKDF2-HMAC-SHA256(activation code, salt, iterations, 32 bytes)
    ///
    /// 1) the signature is checked first  -> file really comes from central
    ///                                       and was not modified
    /// 2) then the file is decrypted with the activation code.
    ///
    /// Nothing secret is stored on the CommuneServer PC.
    /// </summary>
    public static class ActivationFileReader
    {
        private static readonly byte[] Magic = { (byte)'C', (byte)'M', (byte)'A', (byte)'F' };
        private static readonly byte[] PlainMarker = { (byte)'C', (byte)'M', (byte)'A', (byte)'P' };

        public const byte FileVersion = 1;
        public const int SaltSize = 16;
        public const int IvSize = 16;
        public const int KeySize = 32;
        public const int HeaderSize = 4 + 1 + 4 + SaltSize + IvSize + 4;

        private const int MinIterations = 100000;
        private const int MaxIterations = 5000000;

        public static string Open(
            byte[] file,
            string activationCode,
            string publicKeyXml)
        {
            if (file == null || file.Length < HeaderSize + IvSize + 64)
                throw new ActivationFileException("Invalid activation file.");

            for (int i = 0; i < Magic.Length; i++)
            {
                if (file[i] != Magic[i])
                    throw new ActivationFileException("Invalid activation file.");
            }

            if (file[4] != FileVersion)
                throw new ActivationFileException(
                    "Unsupported activation file version.");

            int iterations = ReadInt32(file, 5);
            if (iterations < MinIterations || iterations > MaxIterations)
                throw new ActivationFileException("Invalid activation file.");

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(file, 9, salt, 0, SaltSize);

            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(file, 9 + SaltSize, iv, 0, IvSize);

            int cipherLength = ReadInt32(file, 9 + SaltSize + IvSize);
            long signedLength = (long)HeaderSize + cipherLength;

            if (cipherLength <= 0 ||
                cipherLength % 16 != 0 ||
                signedLength >= file.Length)
            {
                throw new ActivationFileException("Invalid activation file.");
            }

            int signedLen = (int)signedLength;
            byte[] signature = new byte[file.Length - signedLen];
            Buffer.BlockCopy(file, signedLen, signature, 0, signature.Length);

            // ---------------------------------------------------------
            // 1) Signature (issued by central, not modified)
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(publicKeyXml) ||
                !publicKeyXml.TrimStart().StartsWith("<RSAKeyValue>"))
            {
                throw new ActivationFileException(
                    "The central public key is not configured in this application.");
            }

            bool signatureOk;
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKeyXml);

                    signatureOk = rsa.VerifyData(
                        file, 0, signedLen,
                        signature,
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1);
                }
            }
            catch (CryptographicException)
            {
                signatureOk = false;
            }

            if (!signatureOk)
            {
                throw new ActivationFileException(
                    "The activation file is not valid " +
                    "(modified, or not issued by the central authority).");
            }

            // ---------------------------------------------------------
            // 2) Decryption with the activation code
            // ---------------------------------------------------------

            string code = NormalizeCode(activationCode);
            if (code.Length == 0)
                throw new ActivationFileException("The activation code is required.");

            byte[] key;
            using (var kdf = new Rfc2898DeriveBytes(
                Encoding.ASCII.GetBytes(code),
                salt,
                iterations,
                HashAlgorithmName.SHA256))
            {
                key = kdf.GetBytes(KeySize);
            }

            byte[] cipher = new byte[cipherLength];
            Buffer.BlockCopy(file, HeaderSize, cipher, 0, cipherLength);

            byte[] plain;
            try
            {
                System.Diagnostics.Debug.WriteLine(
    $"Iterations: {iterations}");

                System.Diagnostics.Debug.WriteLine(
                    $"Cipher length: {cipherLength}");

                System.Diagnostics.Debug.WriteLine(
                    $"AES key length: {key.Length} bytes");

                System.Diagnostics.Debug.WriteLine(
                    $"IV length: {iv.Length} bytes");

                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    {
                        plain = decryptor.TransformFinalBlock(
                            cipher, 0, cipher.Length);
                    }
                }
            }
            catch (CryptographicException)
            {
                throw new ActivationFileException("The activation code is incorrect.");
            }
            finally
            {
                Array.Clear(key, 0, key.Length);
            }

            if (plain.Length <= PlainMarker.Length)
                throw new ActivationFileException("The activation code is incorrect.");

            for (int i = 0; i < PlainMarker.Length; i++)
            {
                if (plain[i] != PlainMarker[i])
                    throw new ActivationFileException("The activation code is incorrect.");
            }

            return Encoding.UTF8.GetString(
                plain, PlainMarker.Length, plain.Length - PlainMarker.Length);
        }

        /// <summary>
        /// "abcd-1234 efgh" -> "ABCD1234EFGH" (typing errors on separators
        /// and case are tolerated).
        /// </summary>
        public static string NormalizeCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return "";

            var sb = new StringBuilder();
            foreach (char c in code)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(char.ToUpperInvariant(c));
            }
            return sb.ToString();
        }

        private static int ReadInt32(byte[] data, int offset)
        {
            return (data[offset] << 24) |
                   (data[offset + 1] << 16) |
                   (data[offset + 2] << 8) |
                    data[offset + 3];
        }
    }
}
