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
        public Stack<string> addresses;
        public string ipaddr;
        public int tried;

        public MassPing()
        {
            tried = 0;
            ipaddr = FindIP();
            threads = new Thread[256];
            online = new List<string>();
            addresses = new Stack<string>();
        }

        public void NetPing()
        {
            string clientPrefix = ipaddr;
            clientPrefix = clientPrefix.Substring(0, clientPrefix.LastIndexOf('.') + 1);

            for (int i = 0; i < 256; i++)
            {
                addresses.Push(clientPrefix + i);
            }


            for (int i=0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadPing);
                threads[i].Start();
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
                string pingMe = addresses.Pop();
                Ping pingSender = new Ping();
                IPAddress address = IPAddress.Parse(pingMe);
                PingReply reply = pingSender.Send(address);

                if (reply.Status == IPStatus.Success)
                {
                    online.Add(pingMe);
                }

                tried++;
            }
            catch (Exception)
            {
            }
        }

        public bool isDone()
        {
            if (tried > 254)
            {
                return true;
            }

            return false;
        }
    }
}
