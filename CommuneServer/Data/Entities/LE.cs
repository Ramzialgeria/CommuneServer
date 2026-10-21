using CommuneSecurity.Encryption;
using CommuneServer.Activation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class LE // ListeElectoral
    {
        public long Id { get; set; }
        public string LEncrypted { get; set; }

        public int NI { get; set; }

        public virtual ICollection<Cd> Cds { get; set; } = new List<Cd>();

        [NotMapped]
        public string L
        {
            get
            {
                return EncryptionService.Decrypt(
                    LEncrypted);
            }
            set
            {
                LEncrypted =
                    EncryptionService.Encrypt(value);
            }
        }
    }
}
