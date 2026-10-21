using System;

namespace CommuneSecurity.Encryption
{
    public sealed class EncryptedValue
    {
        /*
         * Version allows us to change the encryption format
         * in the future without breaking existing data.
         */
        public byte Version { get; set; }

        /*
         * AES CBC IV.
         */
        public byte[] IV { get; set; }

        /*
         * Encrypted plaintext.
         */
        public byte[] CipherText { get; set; }

        /*
         * HMAC-SHA256 authentication tag.
         */
        public byte[] Hmac { get; set; }

        public EncryptedValue()
        {
        }

        public EncryptedValue(
            byte version,
            byte[] iv,
            byte[] cipherText,
            byte[] hmac)
        {
            Version = version;
            IV = iv;
            CipherText = cipherText;
            Hmac = hmac;
        }
    }
}