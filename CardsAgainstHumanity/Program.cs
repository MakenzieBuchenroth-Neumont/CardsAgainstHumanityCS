using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace CardsAgainstHumanity
{
    class Program
    {
        public static int maxHand;
        public static Player player = new Player();
        public static string ipaddr;
        public static int port = 1337;
        public static Stopwatch stopwatch = new Stopwatch();
        public static Stopwatch playerTimer = new Stopwatch();
        public static bool isServer;
        public static int bufferSize = 720;

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

            while (Connect("!player.join|" + player) == "0")
            {
                Console.WriteLine("Connection unresponsive, check the ip and try again");
                Console.WriteLine("Enter the server ip address:");
                ipaddr = Console.ReadLine();
            }

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
            maxHand = player.hand.Count;

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

            string hasWon = "no";
            int numPlayers;

            numPlayers = int.Parse(Connect("!game.numPlayers"));
            bufferSize = bufferSize * numPlayers;

            while (hasWon == "no" && hasWon.Length>0)
            {

                if (Connect("!player.isCzar") == player.IpAddress)
                {
                    CzarLoop();
                }
                else
                {
                    PlayerLoop();
                }

                hasWon = Connect("!player.hasWon");
            }

            Console.Clear();

            if (hasWon == player.Name)
            {
                Console.WriteLine(" You have Won the game! congratulations!");
            }
            else
            {
                Console.WriteLine(hasWon + "has Won the game! congratulations!");
            }

            ThanksForPlaying();

            Console.ReadLine();

            Environment.Exit(0);

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

                stream.Write(data, 0, data.Length);
                //Console.WriteLine("Sent: {0}", message);

                data = new Byte[bufferSize];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                
                //Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();

                stopwatch.Stop();
                stopwatch.Reset();

                //Console.WriteLine(stopwatch.Elapsed);

                return responseData;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode.ToString() == "TimedOut")
                {
                    return "0";
                }
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error occurred, please restart the client");
                Console.WriteLine("Hit enter to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }

            return "<<THE SHIT HAS HIT THE FAN>>";

        }

        static void PlayerLoop()
        {
            Thread.Sleep(1000);

            string blackcard = Connect("!game.blackcard");

            Console.Clear();

            int fields = numFields(blackcard);


            if (fields == 1)
            {
                Console.WriteLine(blackcard + "\n\n");

                player.DisplayHand();

                Console.WriteLine("Enter the number of the card you wish to play or dp to display the points tally");
                playerTimer.Start();
                if (playerTimer.ElapsedMilliseconds == 60000)
                {
                    TimeoutScreen();
                    return;
                }
                string temp = Console.ReadLine();
                playerTimer.Stop();
                playerTimer.Reset();
                if (temp.ToLower() == "dp")
                {
                    Console.WriteLine(Connect("!game.viewPoints"));
                    temp = Console.ReadLine();
                }
                int cardToPlay = int.Parse(temp);
                Connect("!player.playCard|" + player.hand[cardToPlay] + "`" + player.Name);
                player.hand.RemoveAt(cardToPlay);
            }
            else
            {
                Random rand = new Random();

                string temp = "";

                for (int i = 0; i < fields; i++)
                {
                    Console.Clear();
                    Console.WriteLine(blackcard + "\n\n");
                    player.DisplayHand();
                    Console.WriteLine(" \n Enter the number of the card you wish to go in field " + (i + 1) + "or dp to display the points tally:");
                    playerTimer.Start();
                    if (playerTimer.ElapsedMilliseconds == 60000)
                    {
                        TimeoutScreen();
                        return;
                    }
                    playerTimer.Stop();
                    playerTimer.Reset();
                    if (temp.ToLower() == "dp")
                    {
                        Console.WriteLine(Connect("!game.viewPoints"));
                        temp = Console.ReadLine();
                    }
                    int cardToPlay = int.Parse(Console.ReadLine());;
                    Connect("!player.playCard|" + player.hand[cardToPlay] + "`" + player.Name);
                    player.hand.RemoveAt(cardToPlay);

                }
            }

            player.SeperateHand(Connect("!player.draw|" + (maxHand - player.hand.Count)));

            Console.Clear();

            while (Connect("!game.roundWinner") == "wait")
            {
                Console.WriteLine("Waiting for Czar to choose.");
                Thread.Sleep(500);
                Console.Clear();
                Console.WriteLine("Waiting for Czar to choose..");
                Thread.Sleep(500);
                Console.Clear();
                Console.WriteLine("Waiting for Czar to choose...");
                Thread.Sleep(500);
                Console.Clear();
            }

            Console.Clear();

            Console.WriteLine(Connect("!game.roundWinner") + "Has won the round!");

            Thread.Sleep(3000);
            
        }

        static void TimeoutScreen()
        {
            Console.Clear();
            Console.WriteLine("You have taken too long to play your cards and have been made sit out");
            Console.WriteLine("To rejoin enter r");
            Console.WriteLine("To quit enter q");
            string action = Console.ReadLine();
            if (action.ToLower().Trim() == "q")
            {
                Connect("!player.leave|" + player.Name);
                Environment.Exit(0);
            }

            Connect("!player.rejoin|" + player.Name);
            return;

        }

        static void CzarLoop()
        {
            Connect("!game.newRound");

            string blackcard = Connect("!game.blackcard");
            int fields = numFields(blackcard);

            int timeout = 0;

            while (Connect("!game.roundPlayed") == "False")
            {
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card.");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card..");
                Thread.Sleep(1000);
                Console.Clear();
                Console.WriteLine(blackcard + "\n\n");

                Console.WriteLine("Waiting for players to choose a card...");
                Thread.Sleep(1000);
                timeout++;
                if (timeout == 20)
                {
                    Connect("!game.playerTimeout");
                }
            }

            Console.Clear();
            Console.WriteLine(blackcard + "\n\n");

            string parse = Connect("!game.roundEntries");
            string[] entries = parse.Split('`');

            if (numFields(blackcard) == 1)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    Console.WriteLine(i + ".\t" + entries[i] + "\n");
                }
                Console.WriteLine("Enter the number of the card who wins or dp to display the points tally");
            }
            else
            {
                int count = 0;
                for (int i = 0; i < entries.Length; i+=fields)
                {
                    Console.WriteLine(count + ".\t" + entries[i] + "\n");
                    for (int j = 1; j < fields; j++)
                    {
                        Console.WriteLine(".\t" + entries[i+j] + "\n");
                    }
                    count++;
                }
                Console.WriteLine("Enter the number of the set who wins or dp to display the points tally");
            }

            string temp = Console.ReadLine();
            if (temp.ToLower() == "dp")
            {
                Console.WriteLine(Connect("!game.viewPoints"));
                temp = Console.ReadLine();
            }

            int winner = int.Parse(temp);

            Connect("!game.setWinner|" + winner);

            Console.Clear();

            Console.WriteLine(Connect("!game.roundWinner") + " has won the round!");

            Connect("!game.setNextCzar");

            Thread.Sleep(4000);

        }

        static int numFields(string qhuest)
        {
            int count = 0;
            foreach (char c in qhuest)
            {
                if (c == '_') count++;
            }

            return count / 3;
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

        static void ThanksForPlaying()
        {
            Console.WriteLine(@"
╔╦╗┬ ┬┌─┐┌┐┌┬┌─┌─┐ 
 ║ ├─┤├─┤│││├┴┐└─┐ 
 ╩ ┴ ┴┴ ┴┘└┘┴ ┴└─┘ 
╔═╗┌─┐┬─┐          
╠╣ │ │├┬┘          
╚  └─┘┴└─          
╔═╗┬  ┌─┐┬ ┬┬┌┐┌┌─┐
╠═╝│  ├─┤└┬┘│││││ ┬
╩  ┴─┘┴ ┴ ┴ ┴┘└┘└─┘");
        }
    }
}