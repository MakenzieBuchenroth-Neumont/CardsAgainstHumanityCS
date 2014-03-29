using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Player
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private IPAddress ipAddress;

        public string IpAddress
        {
            get { return ipAddress.ToString(); }
            set { ipAddress = IPAddress.Parse(value); }
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
            ipAddress = IPAddress.Parse("");
            isCzar = false;
        }

        public override string ToString()
        {
            return name + "-" + ipAddress + "-" + isCzar.ToString();
        }
    }
}
