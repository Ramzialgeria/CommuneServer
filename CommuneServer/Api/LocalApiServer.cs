using CommuneServer.Security;
using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace CommuneServer.Api
{
    public class LocalApiServer
    {
        private HttpSelfHostServer _server;

        public void Start()
        {
            var port = ConfigurationManager.AppSettings["LocalApiPort"];
            var IpAdress = ConfigurationManager.AppSettings["LocalApiIp"];
            if (string.IsNullOrWhiteSpace(port))
                throw new InvalidOperationException(
                    "LocalApiPort is not configured.");

            //var url = "http://" + IpAdress + ":" + port + "/";
            // Production: leave LocalApiIp empty -> listen on every address of the
            // PC (the client user types the server IP). Test: set a specific IP.
            var host = string.IsNullOrWhiteSpace(IpAdress) ? "+" : IpAdress.Trim();

            var url = "http://" + host + ":" + port + "/";

            var config = new HttpSelfHostConfiguration(url);

            config.MapHttpAttributeRoutes();

            // JWT authentication
            config.MessageHandlers.Add(
                new JwtAuthenticationHandler());

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();
        }

        public void Stop()
        {
            if (_server == null)
                return;

            try
            {
                _server.CloseAsync().Wait();
            }
            finally
            {
                _server.Dispose();
                _server = null;
            }
        }


    }
}