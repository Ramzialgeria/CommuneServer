using CommuneSecurity.Encryption;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommuneServer.Data.Entities
{
    public class Cn  // Centre
    {
        public long Id { get; set; }

        public string C { get; set; }  // Code

        public string LEncrypted { get; set; }  // Lib

        public string T { get; set; }  // Type

        public int NI { get; set; } // NbInscrit

        public long Cmd { get; set; } // CommuneId

        public virtual Cm Cm { get; set; }  // Commune

        public virtual ICollection<Bu> Bux { get; set; }  = new List<Bu>();  // Bureaux

        [NotMapped]
        public string L
        {
            get
            {
                return EncryptionService.Decrypt(LEncrypted);
            }
            set
            {
                LEncrypted = EncryptionService.Encrypt(value);
            }
        }


    }
}