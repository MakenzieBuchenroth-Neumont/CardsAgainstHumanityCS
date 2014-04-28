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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public List<String> listOfNumbers { get; set; }

        public Player player = new Player();

        public static Game game;

        public Login()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(MainWindow_PreviewKeyDown);

            List<int> nums = Enumerable.Range(0, 60).ToList();
            listOfNumbers = nums.ConvertAll<string>(delegate(int i) { return i.ToString(); });
            listOfNumbers.Add("you are too constipated to play this game, please leave");
            DataContext = this;

        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Connect_Click(Connect, null);
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            player.LastDump = hoursSinceDump.Text.ToString() + ":" + minsSinceDump.Text.ToString();
            player.Name = name.Text;
            Connection.ipaddr = serverIP.Text;

            if (name.Text == "")
            {
                MessageBox.Show("Please enter a name");
                return;
            }
            
            if (hoursSinceDump.Text.ToString() == "")
            {
                MessageBox.Show("Please enter hours since last dump");
                return;
            }
            
            if (minsSinceDump.Text.ToString() == "")
            {
                MessageBox.Show("Please enter minutes since last dump");
                return;
            }
            
            if (serverIP.Text ==  "")
            {
                MessageBox.Show("Please enter a server IP");
                return;          
            }

            if (hoursSinceDump.Text == "you are too constipated to play this game, please leave")
            {
                player.LastDump = "60:59";
            }

            if (minsSinceDump.Text == "you are too constipated to play this game, please leave")
            {
                player.LastDump = "60:59";
            }
            string connectionVal = Connection.Connect("!player.join|" + player);

            if (connectionVal == "nameTaken")
            {
                MessageBox.Show("This name is already taken, try another");
                return;

            }

            if (connectionVal == "TimedOut")
            {
                MessageBox.Show("Connection unresponsive, check the ip and try again");
                return;

            }
            
            if (connectionVal == "ConnectionRefused")
            {
                MessageBox.Show("Connection refused, make sure the server is running and port 1337 is accessible");
                return;
            }

            this.Hide();
            game = new Game();
            game.Show();
            game.Start(player);
        }
    }
}
