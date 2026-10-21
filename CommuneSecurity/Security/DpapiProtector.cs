using System;
using System.Security.Cryptography;
using System.Text;

namespace CommuneSecurity.Security
{
    public static class DpapiProtector
    {
        /*
         * This entropy is NOT the encryption key.
         *
         * It simply adds another application-specific value
         * to the Windows DPAPI protection.
         */
        private static readonly byte[] Entropy =
            Encoding.UTF8.GetBytes(
                "CommuneSecurity-DPAPI-2026-v1");

        public static byte[] Protect(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ProtectedData.Protect(
                data,
                Entropy,
                DataProtectionScope.LocalMachine);
        }

        public static byte[] Unprotect(byte[] encryptedData)
        {
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));

            return ProtectedData.Unprotect(
                encryptedData,
                Entropy,
                DataProtectionScope.LocalMachine);
        }

        public static string ProtectString(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] data = Encoding.UTF8.GetBytes(value);

            byte[] protectedData = Protect(data);

            return Convert.ToBase64String(protectedData);
        }

        public static string UnprotectString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "Encrypted value cannot be empty.",
                    nameof(value));

            byte[] protectedData =
                Convert.FromBase64String(value);

            byte[] data = Unprotect(protectedData);

            return Encoding.UTF8.GetString(data);
        }
    }
}