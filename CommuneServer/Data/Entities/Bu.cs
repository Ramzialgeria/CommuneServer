using CommuneSecurity.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class Bu  // Bureau
    {
        public long Id { get; set; }
        public string C { get; set; }  // Code
        public string T { get; set; }  // Type
        public int NI { get; set; }  // NbInscrit

        public long Cnd { get; set; } // CentreId

        public virtual Cn Cn { get; set; } // Centre


    }
}
