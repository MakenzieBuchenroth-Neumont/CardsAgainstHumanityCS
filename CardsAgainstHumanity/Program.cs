using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
<<<<<<< HEAD
using System.Threading;
using System.Collections.Generic;
=======
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
>>>>>>> e6a68aa863f859a5b87f43b2ca11f07471856356

namespace CardsAgainstHumanity
{
    class Program
    {
        public static Player player = new Player();
        public static string ipaddr;
        public static int port = 1337;
        public static Stopwatch stopwatch = new Stopwatch();
        public static bool isServer;

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
                isServer = true;
            }
            else
            {
                isServer = false;
            }

            Console.WriteLine("Enter a username for the game:");
            player.Name = Console.ReadLine();
            bool correct = false;

            while (correct == false)
            {
                Console.WriteLine("Enter Hours since your last dump e.g '1', '2' etc");
                int hours;
                if (!int.TryParse(Console.ReadLine(),out hours))
                {
                    Console.WriteLine("entry not correctly formatted, must be a single or d whole number");
                    continue;
                }

                Console.WriteLine("Enter Minutes since your last dump");
                int minutes;
                if (!int.TryParse(Console.ReadLine(),out minutes))
                {
                    Console.WriteLine("entry not correctly formatted, must be a single or double digit whole number");
                    continue;
                }

                correct = true;
                Console.WriteLine(hours.ToString() + ":" + minutes.ToString());
                player.LastDump = hours.ToString() + ":" + minutes.ToString();
            }
            if (isServer)
            {
                ipaddr = player.IpAddress;
            }
            else
            {
                Console.WriteLine("Enter the server ip address:");
                ipaddr = Console.ReadLine();
            }

            Console.WriteLine(Connect("!player.join|" + player));
            Console.ReadLine();

//             if (player.IpAddress == "failed")
//             {
//                 Console.WriteLine("failed to obtain ip address");
//                 Console.WriteLine("Please enter the ip address assigned to you by the network you wish to play on:");
//                 player.IpAddress = Console.ReadLine();
//             }
//             
            string handString = Connect("!player.draw|max");
            player.SeperateHand(handString);

            while (Connect("!game.hasStarted") == "False")
            {
                Console.WriteLine("Waiting for game to start.");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Waiting for game to start..");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Waiting for game to start...");
                Thread.Sleep(1000);
                Console.Clear();
            }

            while (Connect("!player.hasWon") == "no")
            {
                if (Connect("!player.isCzar") == player.IpAddress)
                {
                    CzarLoop();
                }
                else
                {
                    PlayerLoop();
                }
            }

            Console.ReadLine();

        }

        static string Connect(String message)
        {
            try
            {
                stopwatch.Start();

                TcpClient client = new TcpClient();

                client.Connect(ipaddr, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();

<<<<<<< HEAD
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);
=======
                // Receive the TcpServer.response.
>>>>>>> e6a68aa863f859a5b87f43b2ca11f07471856356

                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                
                Console.WriteLine("Received: {0}", responseData);

                if (message == "!send object" && responseData == "!expecting obj")
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, player);
                }

                // Close everything.
                stream.Close();
                client.Close();

                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);

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

        static void PlayerLoop()
        {
            Console.Clear();

            Console.WriteLine(Connect("!game.blackcard") + "\n\n");

            player.DisplayHand();

            Console.WriteLine("Enter the number of the card you wish to play");
            int cardToPlay = int.Parse(Console.ReadLine());

            Connect("!player.playCard|" + player.hand[cardToPlay] + "-" + player.Name);

            Console.Clear();

            while (Connect("!game.roundWinner") == "wait")
            {
                Console.WriteLine("Waiting for Czar to choose.");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Waiting for Czar to choose..");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine("Waiting for Czar to choose...");
                Thread.Sleep(1000);
                Console.Clear();
            }

            Console.Clear();

            Console.WriteLine(Connect("!game.roundWinner") + "Has won the round!");
            
        }

        static void CzarLoop()
        {
            Connect("!game.newRound");

            string blackcard = Connect("!game.blackcard");

            while (Connect("!game.roundPlayed") == "False")
            {
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card.");
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card..");
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card...");
            }

            Console.Clear();
            Console.WriteLine(blackcard + "\n\n");

            string parse = Connect("!game.roundEntries");
            string[] entries = parse.Split('-');

            for (int i = 0; i < entries.Length; i++)
            {
                Console.WriteLine(i + ".\t" + entries[i] + "\n");
            }

            Console.WriteLine("Enter the number of the card who wins");
            int winner = int.Parse(Console.ReadLine());

            Connect("!game.setWinner");

            Console.Clear();

            Console.WriteLine(Connect("!game.roundWinner") + "Has won the round!");

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