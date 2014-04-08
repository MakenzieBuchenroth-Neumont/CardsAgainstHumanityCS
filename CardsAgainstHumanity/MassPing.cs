using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace CardsAgainstHumanity
{
    public class MassPing
    {
        public Thread[] threads;
        public List<string> online;
        public string[] addresses;
        public string ipaddr;

        int indexer;

        public MassPing()
        {
            ipaddr = FindIP();
            indexer = 0;
            threads = new Thread[256];
            online = new List<string>();
            addresses = new string[256];
        }

        public void NetPing()
        {
            addresses = new string[256];
            string clientPrefix = ipaddr;
            clientPrefix = clientPrefix.Substring(0, clientPrefix.LastIndexOf('.') + 1);

            for (int i = 0; i < 256; i++)
            {
                addresses[i] = clientPrefix + i;
            }


            Ping pingSender = new Ping();
            for (int i=0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadPing);
                threads[i].Start();
                indexer = i;

            }
        }

        public string FindIP()
        {
            UdpClient u = new UdpClient("8.8.8.8", 1);
            IPAddress localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;
            return localAddr.ToString();
        }

        public void ThreadPing()
        {
            try
            {
                string pingMe = addresses[indexer];
                Ping pingSender = new Ping();
                IPAddress address = IPAddress.Parse(pingMe);
                PingReply reply = pingSender.Send(address);
                if (reply.Status == IPStatus.Success)
                {
                    online.Add(pingMe);
                }
            }
            catch (Exception)
            {
            }

        }

        public bool IsDone()
        {
            if (indexer == 255 && online.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
