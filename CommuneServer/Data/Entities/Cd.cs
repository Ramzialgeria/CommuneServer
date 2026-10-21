using CommuneSecurity.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class Cd  // Candidat
    {
        public long Id { get; set; }

        //public string FullNameAr { get; set; }

        //public string FullNameFr { get; set; }

        public string NAEncrypted { get; set; }  // FullNameAr

        public string NFEncrypted { get; set; }  // FullNameFr

        public int? O { get; set; }  // Order

        public long LId { get; set; } // ListeElectoralId

        public virtual LE LE { get; set; }

        [NotMapped]
        public string NA
        {
            get
            {
                return EncryptionService.Decrypt(NAEncrypted);
            }
            set
            {
                NAEncrypted = EncryptionService.Encrypt(value);
            }
        }

        [NotMapped]
        public string NF
        {
            get
            {
                return EncryptionService.Decrypt(NFEncrypted);
            }
            set
            {
                NFEncrypted = EncryptionService.Encrypt(value);
            }
        }
    }
}
