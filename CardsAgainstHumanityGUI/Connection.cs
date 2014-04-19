using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace CardsAgainstHumanityGUI
{
    public class Connection
    {
        public static string ipaddr;
        public static int port = 1337;
        public static int bufferSize = 720;


        public static string Connect(String message)
        {
            try
            {

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

                //Console.WriteLine(stopwatch.Elapsed);

                return responseData;
            }
            catch (ArgumentNullException e)
            {
                Login.game.createMessageBox("ArgumentNullException:" + e);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode.ToString() == "TimedOut")
                {
                    return "TimedOut";
                }
                else if (e.SocketErrorCode.ToString() == "ConnectionRefused")
                {
                    return "ConnectionRefused";
                }
                Login.game.createMessageBox("SocketException:" + e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Login.game.createMessageBox("Error occurred, please restart the client\n" + "Click ok to exit");
                Environment.Exit(0);
            }

            return "<<THE SHIT HAS HIT THE FAN>>";

        }
    }
}
