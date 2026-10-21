using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class ActivationInfo
    {
        public int Id { get; set; }

        public string ActivationId { get; set; }

        public int ActivationVersion { get; set; }

        public string CodeCommune { get; set; }

        public DateTime ActivatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
