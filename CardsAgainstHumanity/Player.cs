using System;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHumanity
{
    class Player
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }
        private bool isCzar;

        public bool IsCzar
        {
            get { return isCzar; }
            set { isCzar = value; }
        }

        private string lastDump;

        public string LastDump
        {
            get { return lastDump; }
            set { lastDump = value; }
        }


        public Player()
        {
            name = "";
            ipAddress = FindIP();
            isCzar = false;
        }

        public string FindIP()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (ip.Address.ToString().Substring(0, 10) == "192.168.1.")
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }

            return "failed";
        }

        public override string ToString()
        {
            return name + "-" + ipAddress + "-" + isCzar.ToString();
        }
    }
}
