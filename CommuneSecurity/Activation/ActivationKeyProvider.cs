using System;
using System.IO;
using System.Security.Cryptography;

namespace CommuneSecurity.Activation
{
    /// <summary>
    /// Provides the activation encryption key used by
    /// ActivationPackageEncryption.
    ///
    /// The key is generated/provisioned for a specific CommuneServer.
    ///
    /// IMPORTANT:
    /// - The raw key is NOT stored in app.config.
    /// - The key is NOT embedded in the DLL.
    /// - The key is NOT generated automatically during normal startup.
    /// - The key must be provisioned during installation/activation.
    /// </summary>
    public static class ActivationKeyProvider
    {
        private const int KeySize = 32; // AES-256

        private static readonly object SyncRoot =
            new object();

        private static byte[] _key;

        private static readonly string SecurityFolder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData),
                "CommuneServer",
                "Security");

        private static readonly string KeyFilePath =
            Path.Combine(
                SecurityFolder,
                "activation.key");


        /// <summary>
        /// Gets the location of the activation key.
        /// </summary>
        public static string KeyPath
        {
            get
            {
                return KeyFilePath;
            }
        }


        /// <summary>
        /// Returns true if an activation key has been provisioned.
        /// </summary>
        public static bool Exists()
        {
            return File.Exists(KeyFilePath);
        }


        /// <summary>
        /// Loads the existing activation key.
        ///
        /// This method DOES NOT generate a key.
        ///
        /// If the key does not exist, an exception is thrown.
        /// </summary>
        public static byte[] GetKey()
        {
            if (_key != null)
                return Clone(_key);

            lock (SyncRoot)
            {
                if (_key != null)
                    return Clone(_key);

                _key = LoadKey();

                return Clone(_key);
            }
        }


        /// <summary>
        /// Used during CommuneServer provisioning.
        ///
        /// Creates a new random AES-256 activation key.
        /// </summary>
        public static byte[] GenerateKey()
        {
            byte[] key =
                new byte[KeySize];

            using (var rng =
                RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return key;
        }


        /// <summary>
        /// Saves a provisioned activation key.
        ///
        /// This should normally be called by the installation/
        /// activation process, not during normal application startup.
        /// </summary>
        public static void InstallKey(
            byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(
                    nameof(key));

            ValidateKey(key);

            lock (SyncRoot)
            {
                Directory.CreateDirectory(
                    SecurityFolder);

                /*
                 * Write to a temporary file first.
                 *
                 * This prevents a partially written key if the
                 * application is interrupted during the write.
                 */

                string tempFile =
                    KeyFilePath + ".tmp";

                try
                {
                    File.WriteAllBytes(
                        tempFile,
                        key);

                    /*
                     * If a key already exists, do NOT silently replace it.
                     *
                     * Replacing the key would make the existing
                     * Activation.enc impossible to decrypt.
                     */

                    if (File.Exists(KeyFilePath))
                    {
                        throw new InvalidOperationException(
                            "An activation key is already installed.");
                    }

                    File.Move(
                        tempFile,
                        KeyFilePath);

                    ClearMemory();

                    _key = Clone(key);
                }
                finally
                {
                    if (File.Exists(tempFile))
                    {
                        try
                        {
                            File.Delete(tempFile);
                        }
                        catch
                        {
                            // Do not hide the original exception.
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Installs an activation key supplied as Base64.
        ///
        /// Useful when the provisioning process transfers
        /// the key as text.
        /// </summary>
        public static void InstallKey(
            string base64Key)
        {
            if (string.IsNullOrWhiteSpace(base64Key))
            {
                throw new ArgumentException(
                    "Activation key cannot be empty.",
                    nameof(base64Key));
            }

            byte[] key;

            try
            {
                key =
                    Convert.FromBase64String(
                        base64Key);
            }
            catch (FormatException ex)
            {
                throw new CryptographicException(
                    "Invalid Base64 activation key.",
                    ex);
            }

            try
            {
                InstallKey(key);
            }
            finally
            {
                Clear(key);
            }
        }


        /// <summary>
        /// Loads the activation key from disk.
        /// </summary>
        private static byte[] LoadKey()
        {
            if (!File.Exists(KeyFilePath))
            {
                throw new CryptographicException(
                    "Activation key has not been installed.");
            }

            byte[] key =
                File.ReadAllBytes(
                    KeyFilePath);

            ValidateKey(key);

            return key;
        }


        /// <summary>
        /// Validates that the key is exactly 256 bits.
        /// </summary>
        private static void ValidateKey(
            byte[] key)
        {
            if (key == null)
            {
                throw new CryptographicException(
                    "Activation key is null.");
            }

            if (key.Length != KeySize)
            {
                throw new CryptographicException(
                    "Invalid activation key length. " +
                    "Expected 32 bytes.");
            }
        }


        /// <summary>
        /// Returns a copy so callers cannot directly modify
        /// the internal cached key.
        /// </summary>
        private static byte[] Clone(
            byte[] value)
        {
            if (value == null)
                return null;

            byte[] result =
                new byte[value.Length];

            Buffer.BlockCopy(
                value,
                0,
                result,
                0,
                value.Length);

            return result;
        }


        /// <summary>
        /// Clears the cached key from process memory.
        ///
        /// The key file is NOT deleted.
        /// </summary>
        public static void ClearMemory()
        {
            lock (SyncRoot)
            {
                if (_key != null)
                {
                    Clear(_key);
                    _key = null;
                }
            }
        }


        private static void Clear(
            byte[] value)
        {
            if (value == null)
                return;

            Array.Clear(
                value,
                0,
                value.Length);
        }
    }
}