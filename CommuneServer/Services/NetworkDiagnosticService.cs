using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CommuneServer.Services
{
    public class NetworkDiagnosticService
    {
        public NetworkStatus GetStatus(
            string subnet,
            string testAddress = null)
        {
            var result = new NetworkStatus();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus !=
                    OperationalStatus.Up)
                    continue;

                var properties =
                    ni.GetIPProperties();

                foreach (var address in
                    properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily !=
                        AddressFamily.InterNetwork)
                        continue;

                    var ip = address.Address.ToString();

                    if (!IsInSubnet(ip, subnet))
                        continue;

                    result.Connected = true;
                    result.InterfaceName = ni.Name;
                    result.IpAddress = ip;

                    var gateway =
                        properties.GatewayAddresses
                            .FirstOrDefault();

                    if (gateway != null)
                    {
                        result.Gateway =
                            gateway.Address.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(testAddress))
                    {
                        result.Connected =
                            PingAddress(testAddress);
                    }

                    return result;
                }
            }

            return result;
        }

        private bool PingAddress(string address)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply =
                        ping.Send(address, 1000);

                    return reply.Status ==
                           IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsInSubnet(string ip, string subnet)
        {
            if (string.IsNullOrWhiteSpace(ip) ||
                string.IsNullOrWhiteSpace(subnet))
                return false;

            try
            {
                var ipAddress = System.Net.IPAddress.Parse(ip);

                var parts = subnet.Split('/');

                if (parts.Length != 2)
                    return false;

                var networkAddress =
                    System.Net.IPAddress.Parse(parts[0]);

                int prefixLength;

                if (!int.TryParse(parts[1], out prefixLength))
                    return false;

                if (prefixLength < 0 || prefixLength > 32)
                    return false;

                var ipBytes = ipAddress.GetAddressBytes();
                var networkBytes = networkAddress.GetAddressBytes();

                if (ipBytes.Length != networkBytes.Length)
                    return false;

                int fullBytes = prefixLength / 8;
                int remainingBits = prefixLength % 8;

                for (int i = 0; i < fullBytes; i++)
                {
                    if (ipBytes[i] != networkBytes[i])
                        return false;
                }

                if (remainingBits > 0)
                {
                    int mask = 256 - (1 << (8 - remainingBits));

                    if ((ipBytes[fullBytes] & mask) !=
                        (networkBytes[fullBytes] & mask))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class NetworkStatus
    {
        public bool Connected { get; set; }

        public string InterfaceName { get; set; }

        public string IpAddress { get; set; }

        public string Gateway { get; set; }
    }
}