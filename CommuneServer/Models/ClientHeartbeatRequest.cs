using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneServer.Models
{
    public class ClientHeartbeatRequest
    {
        public string ClientCode { get; set; }
        public string MachineName { get; set; }
        public string ApplicationVersion { get; set; }
        public bool ApiConnected { get; set; }

       
    }

   
}
