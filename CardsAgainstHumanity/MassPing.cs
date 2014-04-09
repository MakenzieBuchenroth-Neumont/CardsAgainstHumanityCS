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
        public bool[] tried;

        int indexer;

        public MassPing()
        {
            tried = new bool[256];
            ipaddr = FindIP();
            indexer = 0;
            threads = new Thread[256];
            online = new List<string>();
            addresses = new string[256];
        }

        public void NetPing()
        {
            string clientPrefix = ipaddr;
            clientPrefix = clientPrefix.Substring(0, clientPrefix.LastIndexOf('.') + 1);

            for (int i = 0; i < tried.Length; i++)
            {
                tried[i] = false;
            }

            for (int i = 0; i < 256; i++)
            {
                addresses[i] = clientPrefix + i;
            }


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
                    tried[indexer] = true;
                }
            }
            catch (Exception)
            {
            }
        }

        public bool isDone()
        {
            bool done = true;

            foreach (bool b in tried)
            {
                done = b;

                if (done == false)
                {
                    return done; 
                }
            }

            return true;
        }
    }
}
