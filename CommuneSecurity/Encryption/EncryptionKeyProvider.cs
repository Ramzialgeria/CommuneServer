using System;
using System.IO;
using System.Security.Cryptography;
using CommuneSecurity.Security;

namespace CommuneSecurity.Encryption
{
    public static class EncryptionKeyProvider
    {
        private static readonly object SyncRoot =
            new object();

        private static byte[] _key;

        private static readonly string FolderPath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "CommuneServer",
                "Security");

        private static readonly string KeyFilePath =
            Path.Combine(
                FolderPath,
                "encryption.key");

        public static string KeyPath
        {
            get { return KeyFilePath; }
        }

        public static byte[] GetKey()
        {
            if (_key != null)
                return (byte[])_key.Clone();

            lock (SyncRoot)
            {
                if (_key != null)
                    return (byte[])_key.Clone();

                _key = LoadOrCreateKey();

                return (byte[])_key.Clone();
            }
        }

        private static byte[] LoadOrCreateKey()
        {
            Directory.CreateDirectory(FolderPath);

            if (File.Exists(KeyFilePath))
            {
                return LoadKey();
            }

            return CreateKey();
        }

        private static byte[] CreateKey()
        {
            byte[] key = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            byte[] protectedKey =
                DpapiProtector.Protect(key);

            /*
             * Write only the DPAPI-protected key.
             * The raw AES key is never written to disk.
             */
            File.WriteAllBytes(
                KeyFilePath,
                protectedKey);

            return key;
        }

        private static byte[] LoadKey()
        {
            byte[] protectedKey =
                File.ReadAllBytes(KeyFilePath);

            if (protectedKey.Length == 0)
            {
                throw new CryptographicException(
                    "The encryption key file is empty.");
            }

            byte[] key =
                DpapiProtector.Unprotect(protectedKey);

            if (key.Length != 32)
            {
                throw new CryptographicException(
                    "Invalid encryption key length.");
            }

            return key;
        }

        public static bool KeyExists()
        {
            return File.Exists(KeyFilePath);
        }

        /*
         * Useful during installation/setup.
         */
        public static void Initialize()
        {
            GetKey();
        }

        /*
         * Clears the key from the process memory.
         *
         * The key file itself is NOT deleted.
         */
        public static void ClearMemory()
        {
            lock (SyncRoot)
            {
                if (_key != null)
                {
                    Array.Clear(
                        _key,
                        0,
                        _key.Length);

                    _key = null;
                }
            }
        }
    }
}