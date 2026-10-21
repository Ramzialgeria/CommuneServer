namespace CommuneSecurity.Activation
{
    /// <summary>
    /// Public key of the central authority (RSA-2048, XML format).
    ///
    /// It is only able to VERIFY that an activation file was issued by
    /// central. It cannot create activation files, so it is not a secret
    /// and can safely be embedded in the application.
    ///
    /// Generate the key pair with:  ActivationTool genkeys
    /// then paste the content of central_public.xml below.
    /// The PRIVATE key (central_private.xml) must never leave central.
    /// </summary>
    public static class CentralPublicKey
    {
        public const string Xml = "<RSAKeyValue><Modulus>zSM19xpHh+QZV4gkhTZlxHFuxx3IZFksiRJ3iAH7aGY5BMIxNKXF5fqWIauEWbvvKePn3+sXD9CzsKUuusijD5S6IJ/p2eHcfxxwsGcVSOvu28A6FwglrD9QEgjmy6Dnf1q9sWwN3ENfQqiqglv2wKnyIQnesRx9PDmwkPB6obntzT6NP7UzYhHvCIEu7U/Ce9E/elEVl2G9cIgvz2f+5FdBVvNCpnFZpKzlP5/S3NaOqc+qhHBWCtGwEBPOziF8Vv/M8A6Ay+Zco5hSI4REbl4yxq0kAptPcnYvOhJJ0dzMe5kA+3d4P9muM9auimSo9dzH73fbkHhZW1ZCspGmPQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public static bool IsConfigured
        {
            get { return Xml.StartsWith("<RSAKeyValue>"); }
        }
    }
}
