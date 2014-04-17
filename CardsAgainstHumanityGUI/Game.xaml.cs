using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CardsAgainstHumanityGUI
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public static Player player;
        public static int maxHand;
        public static Stopwatch playerTimer = new Stopwatch();
        public static bool isServer;

        public Game(Player playerDetails)
        {
            InitializeComponent();
            player = playerDetails;
            Start();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WhiteCard1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 1");
        }

        private void WhiteCard2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 2");

        }

        private void WhiteCard3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 3");

        }

        private void WhiteCard4_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 4");

        }

        private void WhiteCard5_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 5");

        }

        private void WhiteCard6_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 6");

        }

        private void WhiteCard7_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 7");

        }

        private void WhiteCard8_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 8");

        }

        private void WhiteCard9_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 9");

        }

        private void WhiteCard10_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Picked Card 10");

        }

        public void Start()
        {

            string handString = Connection.Connect("!player.draw|max");
            player.SeperateHand(handString);
            maxHand = player.hand.Count;

            waitingForStart.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Visible;

            while (Connection.Connect("!game.hasStarted") == "False")
            {
                Thread.Sleep(1000);
            }

            waitingForStart.Visibility = Visibility.Hidden;
            progressBar.Visibility = Visibility.Hidden;

            string hasWon = "no";
            int numPlayers;

            numPlayers = int.Parse(Connection.Connect("!game.numPlayers"));
            Connection.bufferSize = Connection.bufferSize * numPlayers;

            while (hasWon == "no" && hasWon.Length > 0)
            {

                if (Connection.Connect("!player.isCzar") == player.IpAddress)
                {
                    CzarLoop();
                }
                else
                {
                    PlayerLoop();
                }

                hasWon = Connection.Connect("!player.hasWon");
            }

            Console.Clear();

            if (hasWon == player.Name)
            {
                Console.WriteLine("You have Won the game! congratulations!");
            }
            else
            {
                Console.WriteLine(hasWon + " has Won the game! congratulations!");
            }


            Console.ReadLine();

            Environment.Exit(0);

        }

        static void PlayerLoop()
        {
            Thread.Sleep(1000);

            string blackcard = Connection.Connect("!game.blackcard");

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
                    Console.WriteLine(Connection.Connect("!game.viewPoints"));
                    temp = Console.ReadLine();
                }
                int cardToPlay = int.Parse(temp);
                Connection.Connect("!player.playCard|" + player.hand[cardToPlay] + "`" + player.Name);
                player.hand.RemoveAt(cardToPlay);
            }
            else
            {
                Random rand = new Random();

                string temp = "";
                List<int> toRemove = new List<int>();

                for (int i = 0; i < fields; i++)
                {
                    Console.Clear();
                    Console.WriteLine(blackcard + "\n\n");
                    player.DisplayHand();
                    Console.WriteLine(" \n Enter the number of the card you wish to go in field " + (i + 1) + "or dp to display the points tally:");
                    playerTimer.Start();
                    if (temp.ToLower() == "dp")
                    {
                        Console.WriteLine(Connection.Connect("!game.viewPoints"));
                        temp = Console.ReadLine();
                    }
                    int cardToPlay = int.Parse(Console.ReadLine());

                    while (cardToPlay < player.hand.Count - 1 || cardToPlay > -1)
                    {
                        Console.WriteLine("invalid choice, pick the number of a card in your hand");
                        cardToPlay = int.Parse(Console.ReadLine());
                    }

                    temp += player.hand[cardToPlay] + "`";

                    if (playerTimer.ElapsedMilliseconds > 60000)
                    {
                        TimeoutScreen();
                        return;
                    }
                }

                playerTimer.Stop();
                playerTimer.Reset();

                Connection.Connect("!player.playCard|" + temp + player.Name);

                foreach (int i in toRemove)
                {
                    player.hand.RemoveAt(i);
                }
            }

            player.SeperateHand(Connection.Connect("!player.draw|" + (maxHand - player.hand.Count)));

            Console.Clear();

            while (Connection.Connect("!game.roundWinner") == "wait")
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

            Console.WriteLine(Connection.Connect("!game.roundWinner") + " has won the round!");

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
                Connection.Connect("!player.leave|" + player.Name);
                Environment.Exit(0);
            }

            Connection.Connect("!player.rejoin|" + player.Name);
            return;

        }

        static void CzarLoop()
        {
            Connection.Connect("!game.newRound");

            string blackcard = Connection.Connect("!game.blackcard");
            int fields = numFields(blackcard);

            int timeout = 0;

            while (Connection.Connect("!game.roundPlayed") == "False")
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
                    Connection.Connect("!game.playerTimeout");
                }
            }

            Console.Clear();
            Console.WriteLine(blackcard + "\n\n");

            string parse = Connection.Connect("!game.roundEntries");
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
                for (int i = 0; i < entries.Length; i += fields)
                {
                    Console.WriteLine(count + ".\t" + entries[i] + "\n");
                    for (int j = 1; j < fields; j++)
                    {
                        Console.WriteLine(".\t" + entries[i + j] + "\n");
                    }
                    count++;
                }
                Console.WriteLine("Enter the number of the set who wins or dp to display the points tally");
            }

            string temp = Console.ReadLine();
            if (temp.ToLower() == "dp")
            {
                Console.WriteLine(Connection.Connect("!game.viewPoints"));
                temp = Console.ReadLine();
            }

            int winner = int.Parse(temp);

            Connection.Connect("!game.setWinner|" + winner);

            Console.Clear();

            Console.WriteLine(Connection.Connect("!game.roundWinner") + " has won the round!");

            Connection.Connect("!game.setNextCzar");

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

    }
}
