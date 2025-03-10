﻿using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHumanityGUI
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

        private bool isPlayer;

        public bool IsPlayer
        {
            get { return isPlayer; }
            set { isPlayer = value; }
        }

        private string lastDump;

        public string LastDump
        {
            get { return lastDump; }
            set { lastDump = value; }
        }

        public List<string> hand;

        public bool HasPlayed { get; set; }

        public Player()
        {
            name = "";
            ipAddress = FindIP();
            isCzar = false;
            lastDump = "100:100";
            hand = new List<string>();
            HasPlayed = false;
        }

        public void SeperateHand(string handString)
        {
            hand.AddRange(handString.Split('`').ToList());
        }

        public void DisplayHand()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                Console.WriteLine(i + ".\t" + hand[i] + "\n");
            }
        }

        public IPAddress FindIP()
        {
            UdpClient u = new UdpClient("8.8.8.8", 1);
            IPAddress localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;
            return localAddr;
        }

        public override string ToString()
        {
            return name + "`" + ipAddress + "`" + isCzar.ToString() + "`" + lastDump;
        }
    }
}
