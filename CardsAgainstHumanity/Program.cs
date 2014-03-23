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

namespace CardsAgainstHumanity
{
    class Program
    {
        
        public static Player player = new Player();
        public static IPAddress ipaddr;
        public static TcpListener tcplst;
        public static RootObject cards;
        public static string cardDirectory = Path.Combine(Environment.CurrentDirectory, "cards");

        static void Main(string[] args)
        {

            Console.WriteLine("Enter a username for the game:");
            player.Name = Console.ReadLine();

            if (player.IpAddress == "failed")
            {
                Console.WriteLine("failed to obtain ip address");
                Console.WriteLine("Please enter the ip address assigned to you by the network you wish to play on:");
                player.IpAddress = Console.ReadLine();
            }

            Console.WriteLine(player.ToString());
            cards = DeserializeCards(Path.Combine(cardDirectory, "white/base.json"));
            Stack<Card> stack = new Stack<Card>();
            Random r = new Random();
            int[] list = Enumerable.Range(0, cards.cards.Count-1).ToArray();
            Random random = new Random();
            Shuffler shuffler = new Shuffler();
            shuffler.Shuffle(list);

            foreach (int value in list)
            {
                stack.push(cards.cards[value]);
            }

            stack.display();

            try
            {
                ipaddr = IPAddress.Parse(player.IpAddress);
                tcplst = new TcpListener(ipaddr, 1337);

                tcplst.Start();

                Console.WriteLine("Listening on port 1337...");
                Console.WriteLine("The local end point is  :" + tcplst.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error:\n" + ex.StackTrace);
            }

            Console.ReadLine();


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
    }
}
