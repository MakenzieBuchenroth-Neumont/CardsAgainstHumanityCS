using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace Server
{
    class Program
    {
        
        public static IPAddress ipaddr;
        public static RootObject cards;
        public static string cardDirectory = Path.Combine(Environment.CurrentDirectory, "cards");

        static void Main(string[] args)
        {
            SetIP();
            DisplayLogo();
            Console.WriteLine("\nThis is the server window. \n");
            Console.WriteLine("All players will be asked to input the host IP, which is: " + ipaddr.ToString() + "\n");
            Console.WriteLine("This window will now keep a log of all connections and server actions.\n");
            Console.WriteLine("If anything unexpected happens ; check here.");
            Console.ReadLine();

            cards = DeserializeCards(Path.Combine(cardDirectory, "white/base.json"));
            Stack<Card> stack = new Stack<Card>();
            Random r = new Random();
            int[] list = Enumerable.Range(0, cards.cards.Count - 1).ToArray();
            Random random = new Random();
            Shuffler shuffler = new Shuffler();
            shuffler.Shuffle(list);

            foreach (int value in list)
            {
                stack.push(cards.cards[value]);
            }

            stack.display();

            Thread tcplistener = new Thread(listener);
            tcplistener.Start();

        }

        static RootObject DeserializeCards(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                RootObject cards = (RootObject)serializer.Deserialize(file, typeof(RootObject));
                return cards;
            }
        }

        static void listener()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 1337 ;).
                Int32 port = 1337;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client. 
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        if (data == "!send obj")
                        {
                            data = "!expecting obj";
                        }
                        else
                        {
                            // Process the data sent by the client.
                            data = "Message Received";
                        }

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        static void SetIP()
        {
            UdpClient u = new UdpClient("8.8.8.8", 1);
            ipaddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;
        }

        static void DisplayLogo()
        {
            Console.WriteLine(@"
██████╗  ██████╗ ███╗   ██╗████████╗ 
██╔══██╗██╔═══██╗████╗  ██║╚══██╔══╝ 
██║  ██║██║   ██║██╔██╗ ██║   ██║    
██║  ██║██║   ██║██║╚██╗██║   ██║    
██████╔╝╚██████╔╝██║ ╚████║   ██║    
╚═════╝  ╚═════╝ ╚═╝  ╚═══╝   ╚═╝    
                                     
██████╗  █████╗ ███╗   ██╗██╗ ██████╗
██╔══██╗██╔══██╗████╗  ██║██║██╔════╝
██████╔╝███████║██╔██╗ ██║██║██║     
██╔═══╝ ██╔══██║██║╚██╗██║██║██║     
██║     ██║  ██║██║ ╚████║██║╚██████╗
╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝ ╚═════╝");
        }
    }
}
