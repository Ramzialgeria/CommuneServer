using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class RL   // ResultList
    {
        public string Id { get; set; } // id = IdBureau + idListeElectoral
        public long Lid { get; set; }  // ListeElectoralId
        public string CB { get; set; }  // CreatedBy
        public DateTime CA { get; set; }  // CreatedAt
        public string UBy { get; set; }   // UpdatedBy
        public DateTime? UA { get; set; }  // UpdatedAt
        public string VB { get; set; }  // ValidatedBy
        public DateTime? VA { get; set; }  // ValidatedAt
        public virtual LE L { get; set; }
    }
}
