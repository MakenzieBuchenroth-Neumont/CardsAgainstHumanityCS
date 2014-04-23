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
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace CardsAgainstHumanityGUI
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static Player player;
        public static int chosenWinner;
        public static int maxHand;
        public static Stopwatch playerTimer = new Stopwatch();

        public ObservableCollection<CardBinding> whitecards { get; set; }

        private string _blackcard;

        public string blackcard
        {
            get { return _blackcard; }
            set
            {
                _blackcard = value;
                NotifyPropertyChanged("blackcard");
            }
        }

        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public Game()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WhiteCard_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

            if(player.IsCzar)
            {
                chosenWinner = int.Parse(button.Name.Substring(1));
                return;
            }

        }

        public void Start(Player playerDetails)
        {
            whitecards = new ObservableCollection<CardBinding>();

            for (int i = 0; i < 10; i++)
            {
                whitecards.Add(new CardBinding());
            }

            player = playerDetails;

            string handString = Connection.Connect("!player.draw|max");
            player.SeperateHand(handString);
            maxHand = player.hand.Count;

            for (int i = 0; i < whitecards.Count; i++)
            {
                whitecards[i].Card = player.hand[i];
            }

            waitingForStart.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Visible;

            while (Connection.Connect("!game.hasStarted") == "False")
            {
                Yield(10000000);
            }

            waitingForStart.Visibility = Visibility.Collapsed;
            progressBar.Visibility = Visibility.Collapsed;

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
                winner.Visibility = Visibility.Visible;
            }
            else
            {
                otherWinner.Visibility = Visibility.Visible;

            }

            Yield(2000000);

            Environment.Exit(0);

        }

        private void PlayerLoop()
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

        private void TimeoutScreen()
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

        private void CzarLoop()
        {
            Connection.Connect("!game.newRound");

            blackcard = Connection.Connect("!game.blackcard");
            int fields = numFields(blackcard);

            int timeout = 0;

            waitingForPlayers.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Visible;
            

            while (Connection.Connect("!game.roundPlayed") == "False")
            {
                Yield(10000000);
                timeout++;
                if (timeout == 60)
                {
                    Connection.Connect("!game.playerTimeout");
                }
            }

            waitingForPlayers.Visibility = Visibility.Collapsed;
            progressBar.Visibility = Visibility.Collapsed;

            string parse = Connection.Connect("!game.roundEntries");
            string[] cards = parse.Split('`');

            if (numFields(blackcard) == 1)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    whitecards.Add( new CardBinding(cards[i]));
                }
            }
            else
            {
                int offset = 0;
                string temp= "";
                for (int i = 0; i < cards.Length/fields; i++)
                {
                    for (int j = 0; j < fields; j++)
                    {
                        temp += cards[offset + j] + "\n\n\n";
                    }

                    whitecards.Add( new CardBinding(temp));
                    offset += fields;

                    MessageBox.Show(whitecards[0].Card);
                }
            }

            chosenWinner = -1;

            while (chosenWinner == -1)
            {
                Yield(100000);
            }

            Connection.Connect("!game.setWinner|" + chosenWinner);

            Console.WriteLine(Connection.Connect("!game.roundWinner") + " has won the round!");

            Connection.Connect("!game.setNextCzar");

            Thread.Sleep(4000);

        }

        private void Yield(long ticks)
        {

            // Note: a tick is 100 nanoseconds

            long dtEnd = DateTime.Now.AddTicks(ticks).Ticks;

            while (DateTime.Now.Ticks < dtEnd)
            {

                this.Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object unused) { return null; }, null);

            }

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
