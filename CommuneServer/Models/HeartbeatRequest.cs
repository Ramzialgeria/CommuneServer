using System;
using System.Collections.Generic;

namespace CommuneServer.Models
{ 
    public class HeartbeatRequest
    {
        public string ClientCode { get; set; } = null;
        //public bool ServerNetworkConnected { get; set; }
        //public string MachineName{get;set;}=null;
        //public string ApplicationVersion{get;set;}
        //public NetworkReport LocalNetwork{get;set;}
        //public NetworkReport ServerNetwork{get;set;}
        //public bool ApiConnected{get;set;}

        //public List<CommuneClientHeartbeat> Clients { get; set; }
    }

    public class NetworkReport{
        public bool Connected{get;set;}
        public string InterfaceName{get;set;}
        public string IpAddress{get;set;}
        public string Gateway{get;set;}
    }

    public class CommuneClientHeartbeat
    {
        public string ClientCode { get; set; }
        public string MachineName { get; set; }
        public string ApplicationVersion { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
