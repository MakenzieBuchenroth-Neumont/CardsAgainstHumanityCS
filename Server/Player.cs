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
    public class Player
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

        private int points;

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        public Stack<string> messages;

        public Player()
        {
            name = "";
            IpAddress = "0.0.0.0";
            isCzar = false;
            lastDump = "100:100";
            messages = new Stack<string>();
        }

        public Player(string name, string ipAddress, string isCzar, string lastDump)
        {
            this.name = name;
            messages = new Stack<string>();
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.isCzar = bool.Parse(isCzar);
            this.lastDump = lastDump;
            this.points = 0;
        }

        public int TimeSinceDump()
        {
            string[] temp = lastDump.Split(':');
            int hours = int.Parse(temp[0]);
            int minutes = int.Parse(temp[1]);

            return minutes + (hours * 60);
        }

        public bool hasWon()
        {
            if (this.points == Program.returnPointThresh())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return name + "`" + ipAddress + "`" + isCzar.ToString();
        }
    }
}
