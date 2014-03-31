using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace Server
{
    public class Program
    {
        public static IPAddress ipaddr;
        public static CardSet whiteCardSet;
        public static CardSet blackCardSet;
        public static GameManager gameManager;
        public static string cardDirectory = Path.Combine(Environment.CurrentDirectory, "cards");

        static void Main(string[] args)
        {

            DisplayLogo();

            ipaddr = SetIP();

            Console.WriteLine("\nThis is the server window. \n");
            Console.WriteLine("Hit enter for a standard game, otherwise enter 'custom'");
            if (Console.ReadLine().ToLower() == "custom")
            {
                //do custom setup
                gameManager = new GameManager();
            }
            else
            {
                gameManager = new GameManager();
            }
            Console.WriteLine("All players will be asked to input the host IP, which is: " + ipaddr.ToString() + "\n");
            Console.WriteLine("This window will now keep a log of all connections and server actions.\n");
            Console.WriteLine("If anything unexpected happens ; check here.");
            Console.ReadLine();

            Random r = new Random();
            Random random = new Random();
            Shuffler shuffler = new Shuffler();
            whiteCardSet = DeserializeCards(Path.Combine(cardDirectory, "white/base.json"));
            blackCardSet = DeserializeCards(Path.Combine(cardDirectory, "black/base.json"));
            int[] whiteList = Enumerable.Range(0, whiteCardSet.cards.Count - 1).ToArray();
            int[] blackList = Enumerable.Range(0, blackCardSet.cards.Count - 1).ToArray();
            shuffler.Shuffle(whiteList);
            shuffler.Shuffle(blackList);

            foreach (int value in whiteList)
            {
                gameManager.whiteDeck.push(whiteCardSet.cards[value]);
            }

            foreach (int value in blackList)
            {
                gameManager.blackDeck.push(blackCardSet.cards[value]);
            }

            gameManager.blackDeck.display();

            Thread tcplistener = new Thread(listener);
            tcplistener.Start();
<<<<<<< HEAD
            while (true)
            {

                if (gameManager.players.Count > 0)
                {
                    string responce = parseCommand(Console.ReadLine());
                    Console.WriteLine(responce);
                }
            }
=======
>>>>>>> e6a68aa863f859a5b87f43b2ca11f07471856356

        }

        static CardSet DeserializeCards(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                CardSet cards = (CardSet)serializer.Deserialize(file, typeof(CardSet));
                return cards;
            }
        }

        static void listener()
        {
            TcpListener server = null;
            try
            {
                int port = 1337;

                server = new TcpListener(ipaddr, port);
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // listening loop. 
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    if (Console.KeyAvailable)
                    {
                        Console.WriteLine("\n");
                        Thread.Sleep(1000);
                    }
                    // Perform a blocking call to accept requests. 
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

<<<<<<< HEAD
                        if (data.Substring(0,1) == "!")
                        {
                            data = parseCommand(data);
                            Console.WriteLine(data);
                        }
                        else
                        {
=======
                        if (data == "!send obj")
                        {
                            data = "!expecting obj";
                        }
                        else
                        {
                            // Process the data sent by the client.
>>>>>>> e6a68aa863f859a5b87f43b2ca11f07471856356
                            data = "Message Received";
                        }

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    stream.Close();
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


            Console.Read();
        }

        static IPAddress SetIP()
        {
            UdpClient u = new UdpClient("8.8.8.8", 1);
            return ((IPEndPoint)u.Client.LocalEndPoint).Address;
        }

        static string parseCommand(string command)
        {
            if (command.StartsWith("!player.join"))
            {
                string [] playerinfo = parseFields(command);
                gameManager.players.Add(new Player(playerinfo[0], playerinfo[1], playerinfo[2], playerinfo[3]));
                return "Added player: " + playerinfo[0];
            }
            else if (command.StartsWith("!player.draw"))
            {
                int numCards;
                if(command.Substring(command.IndexOf('|')+1) == "max")
                {
                    numCards = gameManager.maxCards;
                }
                else
                {
                    numCards = int.Parse(command.Substring(command.IndexOf('|') + 1));
                }
                string hand = "";

                for (int i = 0; i < numCards; i++)
                {
                    hand += gameManager.whiteDeck.pop().toHand();
                    hand += "-";
                }
                return hand;
            }
            else if (command.StartsWith("!player.isCzar"))
            {
                return gameManager.players[gameManager.CzarCounter].IpAddress.ToString();
            }
            else if (command.StartsWith("!player.play"))
            {
                string[] playerinfo = parseFields(command);

                gameManager.currentPlayerCards.Add(new PlayInfo(playerinfo[0],playerinfo[1]));

                return "";
            }
            else if (command.StartsWith("!game.blackcard"))
            {
                return gameManager.currentBlackCard.text;
            }
            else if (command.StartsWith("!player.hasWon"))
            {
                bool temp;

                foreach (Player p in gameManager.players)
                {
                    temp = p.hasWon();
                    if (temp)
                    {
                        return p.Name;
                    }
                }

                return "no";
            }
            else if (command.StartsWith("!game.start"))
            {
                gameManager.gameStarted = true;

                gameManager.players = gameManager.players.OrderBy(o => o.TimeSinceDump()).ToList();
                gameManager.players[0].IsCzar = true;

                return gameManager.players[0].Name + " is the Card Czar";

            }
            else if (command.StartsWith("!game.hasStarted"))
            {
                return gameManager.gameStarted.ToString();
            }
            else if (command.StartsWith("!game.roundPlayed"))
            {
                return gameManager.played().ToString();
            }
            else if (command.StartsWith("!game.roundWinner"))
            {
                return gameManager.roundWinner;
            }
            else if (command.StartsWith("!game.newRound"))
            {
                gameManager.NewRound();
                return "0";
            }
            else if (command.StartsWith("!game.roundEntries"))
            {
                string temp = "";
                foreach (PlayInfo p in gameManager.currentPlayerCards)
                {
                    temp += p.card + "-";
                }

                temp = temp.Substring(0, temp.Length - 1);

                return temp;
            }
            else if (command.StartsWith("!game.setWinner"))
            {
                command = command.Substring(command.IndexOf('|') + 1);
                gameManager.roundWinner = gameManager.currentPlayerCards[int.Parse(command)].cardPlayer;
                foreach (Player p in gameManager.players)
                {
                    if (p.Name == gameManager.roundWinner)
                    {
                        p.Points++;
                    }
                }
                return "";
            }
            else if (command == "\n")
            {
                return "";
            }
            else
            {
                return "Unknown Command";
            }
        }

        static string[] parseFields(string command)
        {
            command = command.Substring(command.IndexOf('|') + 1);
            string[] playerinfo = command.Split('-');

            return playerinfo;
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
