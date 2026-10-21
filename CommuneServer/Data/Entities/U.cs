using CommuneSecurity.Encryption;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Data.Entities
{
    public class U  // User
    {
        public string Id { get; set; }

        public string Un { get; set; }// UserName

        public string PH { get; set; }// PasswordHash

        public bool IA { get; set; }// IsActive

        public string Rl { get; set; }// Role

        // For APC users: 0101
        // For APW users: 01
        public string CS { get; set; } // CodeStructure

    }
}
