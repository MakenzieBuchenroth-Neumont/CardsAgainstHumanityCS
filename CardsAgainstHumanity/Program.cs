using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardsAgainstHumanity
{
    class Program
    {

        public static Player player = new Player();
        public static TcpClient client;

        static void Main(string[] args)
        {
            DisplayLogo();

            Console.WriteLine("Please elect one and only one player to host the game.\n");
            Console.WriteLine("If you are that player, please type 'server' now.\n");
            Console.WriteLine("Otherwise, type whatever the hell you want, apart from 'server' of course.");
            string whoami = Console.ReadLine();

            if (whoami.ToLower().Trim() == "server" || whoami.ToLower().Trim() == "'server'")
            {
                Console.WriteLine("\nYou are the host\n");
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\server.exe"); //start server
            }

            Console.WriteLine("Enter a username for the game, and make sure its different from everyone else's:");
            player.Name = Console.ReadLine();
            Console.WriteLine("Enter the server ip address:");
            string ipaddr = Console.ReadLine();
            client = new TcpClient(ipaddr, 1337);
            Console.WriteLine(Connect("player.join"));
            Console.ReadLine();

//             if (player.IpAddress == "failed")
//             {
//                 Console.WriteLine("failed to obtain ip address");
//                 Console.WriteLine("Please enter the ip address assigned to you by the network you wish to play on:");
//                 player.IpAddress = Console.ReadLine();
//             }

            Console.WriteLine(player.ToString());

            Console.ReadLine();

        }

        static string Connect(String message)
        {
            try
            {
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing. 
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                //Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                if (message == "!send object" && responseData == "!expecting obj")
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, player);
                }

                // Close everything.
                stream.Close();
                client.Close();

                return responseData;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            return "<<THE SHIT HAS HIT THE FAN>>";
        }

        static void DisplayLogo()
        {
                        Console.WriteLine(@"
.------..------..------..------..------.                        
|C.--. ||A.--. ||R.--. ||D.--. ||S.--. |                        
| :/\: || (\/) || :(): || :/\: || :/\: |                        
| :\/: || :\/: || ()() || (__) || :\/: |                        
| '--'C|| '--'A|| '--'R|| '--'D|| '--'S|                        
`------'`------'`------'`------'`------'                        
.------..------..------..------..------..------..------.        
|A.--. ||G.--. ||A.--. ||I.--. ||N.--. ||S.--. ||T.--. |        
| (\/) || :/\: || (\/) || (\/) || :(): || :/\: || :/\: |        
| :\/: || :\/: || :\/: || :\/: || ()() || :\/: || (__) |        
| '--'A|| '--'G|| '--'A|| '--'I|| '--'N|| '--'S|| '--'T|        
`------'`------'`------'`------'`------'`------'`------'        
.------..------..------..------..------..------..------..------.
|H.--. ||U.--. ||M.--. ||A.--. ||N.--. ||I.--. ||T.--. ||Y.--. |
| :/\: || (\/) || (\/) || (\/) || :(): || (\/) || :/\: || (\/) |
| (__) || :\/: || :\/: || :\/: || ()() || :\/: || (__) || :\/: |
| '--'H|| '--'U|| '--'M|| '--'A|| '--'N|| '--'I|| '--'T|| '--'Y|
`------'`------'`------'`------'`------'`------'`------'`------'");
        }
    }
}