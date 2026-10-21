using CommuneSecurity.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class Cm  // Commune
    {
        public long Id { get; set; }
        public string CC { get; set; } // CodeCommune
        public string CW { get; set; }  // CodeWilaya
        public string NCEncrypted { get; set; } // NameCommune
        public virtual ICollection<Cn> Cns { get; set; } = new List<Cn>(); // Centre

        [NotMapped]
        public string NC
        {
            get
            {
                return EncryptionService.Decrypt(NCEncrypted);
            }
            set
            {
                NCEncrypted = EncryptionService.Encrypt(value);
            }
        }

    }
}
