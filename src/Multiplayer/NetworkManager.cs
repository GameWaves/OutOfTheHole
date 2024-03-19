using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
namespace OutofTheHole.Multiplayer;

public partial class NetworkManager
{
    public static IPAddress GetLocalIpAddressNoInternet(string localNetworkCidrIp)
    {
        var localIps = GetLocalIPv4AddressList();
        if (localIps.Count == 1)
        {
            return localIps[0];
        }

        /*foreach (var ip in localIps)
        {
            if (ip, localNetworkCidrIp)
            {
                return ip;
            }
        }*/

        // If no IP addresses match CIDR IP or none were found, return 255.255.255.255
        return IPAddress.None;
    }

    public static List<IPAddress> GetLocalIPv4AddressList()
    {
        var localIps = new List<IPAddress>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            var ips =
                nic.GetIPProperties().UnicastAddresses
                    .Select(uni => uni.Address)
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();

            localIps.AddRange(ips);
        }

        return localIps;
    }
}