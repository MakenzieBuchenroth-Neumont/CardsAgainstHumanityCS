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

        public Player player;
        public int chosenWinner;

        public int chosenCard;
        public int maxHand;
        public Stopwatch playerTimer = new Stopwatch();

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

        private string _message;

        public string message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("message");
            }
        }

        private string _whitecard1;

        public string whitecard1
        {
            get { return _whitecard1; }
            set
            {
                _whitecard1 = value;
                NotifyPropertyChanged("whitecard1");
            }
        }

        private string _whitecard2;

        public string whitecard2
        {
            get { return _whitecard2; }
            set
            {
                _whitecard2 = value;
                NotifyPropertyChanged("whitecard2");
            }
        }
        private string _whitecard3;

        public string whitecard3
        {
            get { return _whitecard3; }
            set
            {
                _whitecard3 = value;
                NotifyPropertyChanged("whitecard3");
            }
        }
        private string _whitecard4;

        public string whitecard4
        {
            get { return _whitecard4; }
            set
            {
                _whitecard4 = value;
                NotifyPropertyChanged("whitecard4");
            }
        }
        private string _whitecard5;

        public string whitecard5
        {
            get { return _whitecard5; }
            set
            {
                _whitecard5 = value;
                NotifyPropertyChanged("whitecard5");
            }
        }
        private string _whitecard6;

        public string whitecard6
        {
            get { return _whitecard6; }
            set
            {
                _whitecard6 = value;
                NotifyPropertyChanged("whitecard6");
            }
        }
        private string _whitecard7;

        public string whitecard7
        {
            get { return _whitecard7; }
            set
            {
                _whitecard7 = value;
                NotifyPropertyChanged("whitecard7");
            }
        }
        private string _whitecard8;

        public string whitecard8
        {
            get { return _whitecard8; }
            set
            {
                _whitecard8 = value;
                NotifyPropertyChanged("whitecard8");
            }
        }
        private string _whitecard9;

        public string whitecard9
        {
            get { return _whitecard9; }
            set
            {
                _whitecard9 = value;
                NotifyPropertyChanged("whitecard9");
            }
        }
        private string _whitecard10;

        public string whitecard10
        {
            get { return _whitecard10; }
            set
            {
                _whitecard10 = value;
                NotifyPropertyChanged("whitecard10");
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
                int numPlayers = int.Parse(Connection.Connect("!game.numPlayers"));
                int choice = int.Parse(button.Name.Substring(1));
                if ( numPlayers -1 >= choice)
                {
                    chosenWinner = int.Parse(button.Name.Substring(1)) - 1;
                    return;
                }
            }

            if (player.IsPlayer)
            {
                button.Visibility = Visibility.Hidden;
                chosenCard = int.Parse(button.Name.Substring(1))-1;
                return;
            }

        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = messageBox.Text;
            messageBox.Text = "";

            message += "`" + player.Name + "`" + DateTime.Now.ToString("HH:mm:ss");

            Connection.Connect("!chat.sendMessage|" + message);
        }

        public void createMessageBox(string info)
        {
            MessageBox.Show(info);
        }

        public void Start(Player playerDetails)
        {
            Thread chatUpdater = new Thread(chatUpdate);

            chatUpdater.Start();

            player = playerDetails;

            string handString = Connection.Connect("!player.draw|max");
            player.SeperateHand(handString);
            maxHand = player.hand.Count;

            whitecard1 = player.hand[0];
            whitecard2 = player.hand[1];
            whitecard3 = player.hand[2];
            whitecard4 = player.hand[3];
            whitecard5 = player.hand[4];
            whitecard6 = player.hand[5];
            whitecard7 = player.hand[6];
            whitecard8 = player.hand[7];
            whitecard9 = player.hand[8];
            whitecard10 = player.hand[9];

            progressBar.Visibility = Visibility.Visible;
            message = "waiting for game to start";

            while (Connection.Connect("!game.hasStarted") == "False")
            {
                Yield(10000000);
            }

            progressBar.Visibility = Visibility.Collapsed;
            Message.Visibility = Visibility.Collapsed;

            string hasWon = "no";
            int numPlayers = 0;
            try
            {
                numPlayers = int.Parse(Connection.Connect("!game.numPlayers"));
            }
            catch (Exception)
            {
            	MessageBox.Show("The server is unresponsive\n" + "click ok to exit");
                Environment.Exit(0);
            }

            Connection.bufferSize = Connection.bufferSize * numPlayers;

            whitecard1 = "";
            whitecard2 = "";
            whitecard3 = "";
            whitecard4 = "";
            whitecard5 = "";
            whitecard6 = "";
            whitecard7 = "";
            whitecard8 = "";
            whitecard9 = "";
            whitecard10 = "";
            blackcard = "";

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

            if (hasWon == player.Name)
            {
                message = "you have won, congratulations!";
            }
            else
            {
                message = hasWon + " has won the game";

            }

            Message.Visibility = Visibility.Visible;

            Yield(2000000);

            Environment.Exit(0);

        }

        private void PlayerLoop()
        {
            Message.Visibility = Visibility.Visible;

            Yield(10000000);

            c1.Visibility = Visibility.Visible;
            c2.Visibility = Visibility.Visible;
            c3.Visibility = Visibility.Visible;
            c4.Visibility = Visibility.Visible;
            c5.Visibility = Visibility.Visible;
            c6.Visibility = Visibility.Visible;
            c7.Visibility = Visibility.Visible;
            c8.Visibility = Visibility.Visible;
            c9.Visibility = Visibility.Visible;
            c10.Visibility = Visibility.Visible;

            blackcard = Connection.Connect("!game.blackcard");

            int fields = numFields(blackcard);

            whitecard1 = player.hand[0];
            whitecard2 = player.hand[1];
            whitecard3 = player.hand[2];
            whitecard4 = player.hand[3];
            whitecard5 = player.hand[4];
            whitecard6 = player.hand[5];
            whitecard7 = player.hand[6];
            whitecard8 = player.hand[7];
            whitecard9 = player.hand[8];
            whitecard10 = player.hand[9];

            player.IsPlayer = true;

            if (fields == 1)
            {

                message = ("Click the card you wish to play");
                playerTimer.Start();

                chosenCard = -1;

                while (chosenCard == -1)
                {
                    Yield(100000);

                    if (playerTimer.ElapsedMilliseconds == 60000)
                    {
                        TimeoutScreen();
                        return;
                    }
                }
                playerTimer.Stop();
                playerTimer.Reset();

                Connection.Connect("!player.playCard|" + player.hand[chosenCard] + "`" + player.Name);
                player.hand.RemoveAt(chosenCard);
            }
            else
            {

                string temp = "";
                List<int> toRemove = new List<int>();

                for (int i = 0; i < fields; i++)
                {
                    message ="Click the card you wish to go in field " + (i + 1);

                    playerTimer.Start();

                    chosenCard = -1;

                    while (chosenCard == -1)
                    {
                        Yield(100000);

                        if (playerTimer.ElapsedMilliseconds == 60000)
                        {
                            TimeoutScreen();
                            return;
                        }
                    }

                    temp += player.hand[chosenCard] + "`";
                    toRemove.Add(chosenCard);

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

            message = "waiting for czar to choose";
            progressBar.Visibility = Visibility.Visible;

            while (Connection.Connect("!game.roundWinner") == "wait")
            {
                Yield(10000000);
            }

            c1.Visibility = Visibility.Hidden;
            c2.Visibility = Visibility.Hidden;
            c3.Visibility = Visibility.Hidden;
            c4.Visibility = Visibility.Hidden;
            c5.Visibility = Visibility.Hidden;
            c6.Visibility = Visibility.Hidden;
            c7.Visibility = Visibility.Hidden;
            c8.Visibility = Visibility.Hidden;
            c9.Visibility = Visibility.Hidden;
            c10.Visibility = Visibility.Hidden;

            progressBar.Visibility = Visibility.Collapsed;

            blackcard = Connection.Connect("!game.getWinCards|" + blackcard);

            message = Connection.Connect("!game.roundWinner") + " has won the round!";

            Yield(30000000);

            whitecard1 = "";
            whitecard2 = "";
            whitecard3 = "";
            whitecard4 = "";
            whitecard5 = "";
            whitecard6 = "";
            whitecard7 = "";
            whitecard8 = "";
            whitecard9 = "";
            whitecard10 = "";
            blackcard = "";

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

            message = "waiting for players to choose";
            Message.Visibility = Visibility.Visible;
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

            progressBar.Visibility = Visibility.Collapsed;


            string parse = Connection.Connect("!game.roundEntries");
            string[] cards = parse.Split('`');

            message = "pick the winner";

            int offset = 0;

            if (cards.Length/fields > 0)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard1 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c1.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 1)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard2 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c2.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 2)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard3 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c3.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 3)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard4 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c4.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 4)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard5 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c5.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 5)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard6 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c6.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 6)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard7 += cards[offset + j] + "\n\n\n";
                }
                offset += fields;
                c7.Visibility = Visibility.Visible;
            }

            if (cards.Length/fields > 7)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard8 += cards[offset + j] + "\n\n\n";
                }
                c8.Visibility = Visibility.Visible;
                offset += fields;
            }

            if (cards.Length/fields > 8)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard9 += cards[offset + j] + "\n\n\n";
                }
                c9.Visibility = Visibility.Visible;
                offset += fields;
            }

            if (cards.Length/fields > 9)
            {
                for (int j = 0; j < fields; j++)
                {
                    whitecard10 += cards[offset + j] + "\n\n\n";
                }
                c10.Visibility = Visibility.Visible;
                offset += fields;
            }

            player.IsCzar = true;

            chosenWinner = -1;

            while (chosenWinner == -1)
            {
                Yield(100000);
            }

            c1.Visibility = Visibility.Hidden;
            c2.Visibility = Visibility.Hidden;
            c3.Visibility = Visibility.Hidden;
            c4.Visibility = Visibility.Hidden;
            c5.Visibility = Visibility.Hidden;
            c6.Visibility = Visibility.Hidden;
            c7.Visibility = Visibility.Hidden;
            c8.Visibility = Visibility.Hidden;
            c9.Visibility = Visibility.Hidden;
            c10.Visibility = Visibility.Hidden;


            StringBuilder sb = new StringBuilder(blackcard);

            List<string> winningCards = new List<string>();

            int cardOffset = 0;

            if (fields == 2)
            {
                cardOffset++;
            }
            else if (fields == 3)
            {
                cardOffset += 2;
            }

            cardOffset += chosenWinner-1;

            for (int i = 0; i < fields; i++)
            {
                winningCards.Add(cards[cardOffset + i]);
            }

            for (int i = 0; i < fields; i++)
            {
                int index = sb.ToString().IndexOf("___");

                sb.Remove(index, "___".Length);
                sb.Insert(index, winningCards[i]);

            }

            blackcard = sb.ToString();

            Connection.Connect("!game.setWinCards|" + blackcard);

            player.IsCzar = false;

            Connection.Connect("!game.setWinner|" + chosenWinner);

            message = Connection.Connect("!game.roundWinner") + " has won the round!";

            Connection.Connect("!game.setNextCzar");

            Yield(30000000);

            whitecard1 = "";
            whitecard2 = "";
            whitecard3 = "";
            whitecard4 = "";
            whitecard5 = "";
            whitecard6 = "";
            whitecard7 = "";
            whitecard8 = "";
            whitecard9 = "";
            whitecard10 = "";
            blackcard = "";

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

        private int numFields(string qhuest)
        {
            int count = 0;
            foreach (char c in qhuest)
            {
                if (c == '_') count++;
            }

            return count / 3;
        }

        private void chatUpdate()
        {
            while (true)
            {
               string messages = Connection.Connect("!chat.getMessages|" + player.Name);

               if (messages != "0")
               {
                   string[] parsedMessages = messages.Split('|');

                   foreach (string s in parsedMessages)
                   {
                       string[] messageComponents = s.Split('`');

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            try
                            {
                                chatBox.AppendText(messageComponents[1] + " | " + messageComponents[2] + "\n");
                                chatBox.AppendText(messageComponents[0] + "\n\n");
                                chatBox.ScrollToEnd();
                            }
                            catch (Exception)
                            {
                                //this accommodates for if the program crashes mid cycle, stopping it from reading non chat information in.
                            }

                        }));

                   }
               }

               Yield(1000000);
            }
        }

    }
}
