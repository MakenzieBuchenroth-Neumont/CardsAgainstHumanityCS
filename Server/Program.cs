﻿using System;
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

namespace Server {
	public class Program {
		public static IPAddress ipaddr;
		public static int port = 1337;
		public static List<string> AInames;
		public static int bufferSize = 720;
		public static GameManager gameManager;
		public static bool AIstarted = false;
		public static string cardDirectory = Path.Combine(Environment.CurrentDirectory, "cards");
		public static string[] whiteCardFilePaths = Directory.GetFiles(cardDirectory + "\\white");
		public static string[] blackCardFilePaths = Directory.GetFiles(cardDirectory + "\\black");



		static void Main(string[] args) {

			Console.SetWindowSize(90, 25);

			DisplayLogo();

			ipaddr = SetIP();
			Shuffler shuffler = new Shuffler();

			Console.WriteLine("\nThis is the server window. \n");
			Console.WriteLine("Hit enter for a standard game, otherwise enter 'custom'");
			if (Console.ReadLine().ToLower() == "custom") {
				Console.WriteLine("Set the server port (enter d for default) :");
				string temp = Console.ReadLine();

				if (temp.ToLower().Trim() != "d") {
					port = int.Parse(temp);
				}

				Console.WriteLine("Max cards: (recommended 5-15)");
				int maxCards = int.Parse(Console.ReadLine());

				Console.WriteLine("Win threshold: (recommended 10-30) points");
				int pointThresh = int.Parse(Console.ReadLine());

				bool[] CardsToUse = new bool[whiteCardFilePaths.Length];

				for (int i = 1; i < CardsToUse.Length; i++) {
					string name = whiteCardFilePaths[i].Substring(whiteCardFilePaths[i].LastIndexOf('\\') + 1);
					Console.WriteLine("Would you like to use the " + name.Substring(0, name.IndexOf('.')) + " card set: (y or n)");
					if (Console.ReadLine().ToLower() == "y") {
						CardsToUse[i] = true;
					}
					else {
						CardsToUse[i] = false;
					}

				}

				gameManager = new GameManager(maxCards, pointThresh);

				List<CardSet> whiteCardSets = new List<CardSet>();
				List<CardSet> blackCardSets = new List<CardSet>();

				List<Card> AllWhiteCards = new List<Card>();
				List<Card> AllBlackCards = new List<Card>();

				for (int i = 0; i < CardsToUse.Length; i++) {
					if (CardsToUse[i] == true) {
						whiteCardSets.Add(DeserializeCards(whiteCardFilePaths[i]));
						blackCardSets.Add(DeserializeCards(blackCardFilePaths[i]));
					}
				}

				for (int i = 0; i < whiteCardSets.Count; i++) {
					AllWhiteCards.AddRange(whiteCardSets[i].cards);
					AllBlackCards.AddRange(blackCardSets[i].cards);
				}

				shuffler.Shuffle(AllWhiteCards);
				shuffler.Shuffle(AllBlackCards);

				foreach (Card c in AllWhiteCards) {
					gameManager.whiteDeck.Push(c);
				}

				foreach (Card c in AllBlackCards) {
					gameManager.blackDeck.Push(c);
				}


			}
			else {
				gameManager = new GameManager();
				CardSet whiteCardSet = DeserializeCards(Path.Combine(cardDirectory, "white/all.json"));
				CardSet blackCardSet = DeserializeCards(Path.Combine(cardDirectory, "black/all.json"));

				shuffler.Shuffle(whiteCardSet.cards);
				shuffler.Shuffle(blackCardSet.cards);

				foreach (Card c in whiteCardSet.cards) {
					gameManager.whiteDeck.Push(c);
				}

				foreach (Card c in blackCardSet.cards) {
					gameManager.blackDeck.Push(c);
				}
			}
			Console.WriteLine("All players will be asked to input the host IP, which is: " + ipaddr.ToString() + "\n");
			Console.WriteLine("This window will now keep a log of all connections and server actions.\n");
			Console.WriteLine("If anything unexpected happens ; check here. \n");
			Console.WriteLine("If you are unsure of any server commands, use !help to display a list of commands and their function \n");

			AInames = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "AInames.txt")).ToList();

			Thread tcplistener = new Thread(listener);
			tcplistener.Start();

			string responce = "";

			while (true) {

				if (gameManager.players.Count > 0) {
					responce = parseCommand(Console.ReadLine());
					Console.WriteLine(responce);
				}
			}

		}

		static CardSet DeserializeCards(string path) {
			using (StreamReader file = File.OpenText(path)) {
				JsonSerializer serializer = new JsonSerializer();
				CardSet cards = (CardSet)serializer.Deserialize(file, typeof(CardSet));
				return cards;
			}
		}

		static void listener() {
			TcpListener server = null;

			try {

				server = new TcpListener(ipaddr, port);
				server.Start();

				Byte[] bytes = new Byte[bufferSize];
				String data = null;

				// listening loop. 
				while (true) {
					//Console.Write("Waiting for a connection... ");
					if (Console.KeyAvailable) {
						Console.WriteLine("\n");
						Thread.Sleep(1000);
					}

					TcpClient client = server.AcceptTcpClient();
					//Console.WriteLine("Connected!");

					data = null;

					NetworkStream stream = client.GetStream();

					int i;

					while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
						data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
						//Console.WriteLine("Received: " + data);

						if (data.Substring(0, 1) == "!") {

							data = parseCommand(data);
							if (data == "False" || data == "wait" || data == "0" || data.Contains('`')) {
							}
							else {
								Console.WriteLine(data);
							}
						}
						else {
							data = "Message Received";
						}

						byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

						stream.Write(msg, 0, msg.Length);
						//Console.WriteLine("Sent: {0}", data);
					}

					stream.Close();
					client.Close();
				}
			}
			catch (SocketException e) {
				Console.WriteLine("SocketException: {0}", e);
			}
			catch (IOException e) {
				Console.WriteLine("IOException: {0}", e);
				Console.ReadLine();
			}
			finally {
				server.Stop();
			}


			Console.Read();
		}

		static void AIloop() {
			Thread.Sleep(100);
			Random rand = new Random();
			bool played = false;

			while (parseCommand("!player.hasWon") == "no") {
				while (gameManager.roundWinner == "wait" && !gameManager.played()) {
					played = false;

					foreach (AI ai in gameManager.AIs) {
						foreach (PlayInfo p in gameManager.currentPlayerCards) {
							if (p.cardPlayer == ai.Name) {
								played = true;
							}
						}
					}
					if (!played) {
						foreach (AI ai in gameManager.AIs) {
							int fields = gameManager.numFields(gameManager.currentBlackCard.text);
							if (fields == 1) {
								int cardToPlay = rand.Next(0, ai.hand.Count - 1);

								parseCommand("!player.playCard|" + ai.hand[cardToPlay] + "`" + ai.Name);

								ai.hand.RemoveAt(cardToPlay);
							}
							else {
								string temp = "";
								int cardToPlay = rand.Next(0, 9);
								int[] toRemove = new int[fields];

								for (int i = 0; i < fields; i++) {
									while (toRemove.Contains(cardToPlay)) {
										cardToPlay = rand.Next(0, ai.hand.Count - 1);
									}
									toRemove[i] = cardToPlay;
									temp += ai.hand[cardToPlay] + "`";
									ai.hand.RemoveAt(cardToPlay);

								}

								temp += ai.Name;
								parseCommand("!player.playCard|" + temp);
							}

							ai.SeperateHand(parseCommand("!player.draw|" + (gameManager.maxCards - ai.hand.Count)));
						}
					}
					else {
						Thread.Sleep(100);
					}
				}
				Thread.Sleep(100);
			}
		}

		static IPAddress SetIP() {
			UdpClient u = new UdpClient("8.8.8.8", 1);
			return ((IPEndPoint)u.Client.LocalEndPoint).Address;
		}

		static string parseCommand(string command) {
			if (command.StartsWith("!player.join")) {
				string[] playerinfo = parseFields(command);

				foreach (Player p in gameManager.players) {
					if (playerinfo[0] == p.Name) {
						return "nameTaken";
					}
				}

				gameManager.players.Add(new Player(playerinfo[0], playerinfo[1], playerinfo[2], playerinfo[3]));
				return "Added player: " + playerinfo[0];
			}
			else if (command.StartsWith("!player.draw")) {
				int numCards;
				if (command.Substring(command.IndexOf('|') + 1) == "max") {
					numCards = gameManager.maxCards;
				}
				else {
					numCards = int.Parse(command.Substring(command.IndexOf('|') + 1));
				}
				string hand = "";

				for (int i = 0; i < numCards; i++) {
					hand += gameManager.whiteDeck.Pop().toHand();
					hand += "`";
				}
				hand = hand.Substring(0, hand.Length - 1);
				return hand;
			}
			else if (command.StartsWith("!player.isCzar")) {
				return gameManager.players[gameManager.CzarCounter].Name;
			}
			else if (command.StartsWith("!player.SinglePoint")) {
				string[] playerinfo = parseFields(command);

				foreach (var player in gameManager.players) {
					if (player.Name == playerinfo[0]) {
						return player.Points.ToString();
					}
				}
				return "0";
			}
			else if (command.StartsWith("!player.Points")) {
				string points = ""; 
				foreach (var player in gameManager.players) {
					points += player.Points.ToString() + "|";
				}
				return points;
			}
			else if (command.StartsWith("!player.SingleName")) {
				string[] playerinfo = parseFields(command);

				foreach (var player in gameManager.players) {
					if (player.Name == playerinfo[0]) {
						return player.Name;
					}
				}
				return "0";
			}
			else if (command.StartsWith("!player.Names")) {
				string names = "";
				foreach (var player in gameManager.players) {
					names += player.Name + "|";
				}
				return names;
			}
			else if (command.StartsWith("!player.leave")) {
				string[] playerinfo = parseFields(command);

				for (int i = 0; i < gameManager.players.Count; i++) {
					if (gameManager.players[i].Name == playerinfo[0]) {
						gameManager.players.RemoveAt(i);
					}
				}

				for (int i = 0; i < gameManager.afkPlayers.Count; i++) {
					if (gameManager.afkPlayers[i].Name == playerinfo[0]) {
						gameManager.afkPlayers.RemoveAt(i);
					}
				}
				return "0";
			}
			else if (command.StartsWith("!player.rejoin")) {
				string[] playerinfo = parseFields(command);

				for (int i = 0; i < gameManager.afkPlayers.Count; i++) {
					if (gameManager.afkPlayers[i].Name == playerinfo[0]) {
						gameManager.players.Add(gameManager.afkPlayers[i]);
						gameManager.afkPlayers.RemoveAt(i);
					}
				}
				return "0";
			}
			else if (command.StartsWith("!player.playCard")) {
				int entries = gameManager.numFields(gameManager.currentBlackCard.text);

				string[] playerinfo = parseFields(command);

				if (entries == 1) {
					gameManager.currentPlayerCards.Add(new PlayInfo(playerinfo[0], playerinfo[1]));
				}
				else {
					string temp = "";

					for (int i = 0; i < entries; i++) {
						temp += playerinfo[i] + "`";
					}

					temp = temp.Substring(0, temp.Length - 1);

					gameManager.currentPlayerCards.Add(new PlayInfo(temp, playerinfo[playerinfo.Length - 1]));
				}

				for (int i = 0; i < gameManager.waitingFor.Count; i++) {
					if (gameManager.waitingFor[i].Name == playerinfo[1]) {
						gameManager.waitingFor.RemoveAt(i);
					}
				}

				return "0";
			}
			else if (command.StartsWith("!game.blackcard")) {
				if (!AIstarted) {
					Thread AiLoop = new Thread(AIloop);
					AiLoop.Start();
					AIstarted = true;
				}
				return gameManager.currentBlackCard.text;
			}
			else if (command.StartsWith("!player.hasWon")) {
				foreach (Player p in gameManager.players) {
					if (p.hasWon()) {
						return p.Name;
					}
				}
				foreach (AI ai in gameManager.AIs) {
					if (ai.hasWon()) {
						return ai.Name;
					}
				}

				return "no";
			}
			else if (command.StartsWith("!game.start")) {
				gameManager.gameStarted = true;

				gameManager.players = gameManager.players.OrderBy(o => o.TimeSinceDump()).ToList();
				gameManager.players[gameManager.CzarCounter].IsCzar = true;

				return gameManager.players[0].Name + " is the Card Czar";

			}
			else if (command.StartsWith("!game.hasStarted")) {
				return gameManager.gameStarted.ToString();
			}
			else if (command.StartsWith("!game.roundPlayed")) {
				return gameManager.played().ToString();
			}
			else if (command.StartsWith("!game.roundWinner")) {
				return gameManager.roundWinner;
			}
			else if (command.StartsWith("!game.newRound")) {
				gameManager.NewRound();
				return "0";
			}
			else if (command.StartsWith("!game.setNextCzar")) {
				gameManager.incrementCzarCounter();
				return "0";
			}
			else if (command.StartsWith("!game.numPlayers")) {
				return (gameManager.numAIs + gameManager.players.Count).ToString();
			}
			else if (command.StartsWith("!game.addAI")) {
				Random rand = new Random();

				AI newAi = new AI();

				int nameIndex = rand.Next(0, AInames.Count - 1);
				if (AInames.Count > 0) {
					newAi.Name = AInames[nameIndex];
					AInames.RemoveAt(nameIndex);
				}
				else {
					newAi.Name = "AI-" + rand.Next(0, int.MaxValue);
				}
				newAi.IpAddress = ipaddr.ToString();
				for (int i = 0; i < gameManager.maxCards; i++) {
					newAi.hand.Add(gameManager.whiteDeck.Pop().toHand());
				}

				gameManager.numAIs++;
				gameManager.AIs.Add(newAi);

				return "Added AI:" + newAi.Name;

			}
			else if (command.StartsWith("!game.setWinCards|")) {
				command = command.Substring(command.IndexOf('|') + 1);

				gameManager.winCards = command;

				return "0";
			}
			else if (command.StartsWith("!game.getWinCards|")) {
				return gameManager.winCards;
			}
			else if (command.StartsWith("!game.roundEntries")) {
				string temp = "";

				Shuffler shuffler = new Shuffler();
				shuffler.Shuffle(gameManager.currentPlayerCards);

				foreach (PlayInfo p in gameManager.currentPlayerCards) {
					temp += p.card + "`";
				}

				if (temp == "") {
					return "0";
				}

				temp = temp.Substring(0, temp.Length - 1);

				return temp;
			}
			else if (command.StartsWith("!game.setWinner")) {
				command = command.Substring(command.IndexOf('|') + 1);
				gameManager.roundWinner = gameManager.currentPlayerCards[int.Parse(command)].cardPlayer;
				foreach (Player p in gameManager.players) {
					if (p.Name == gameManager.roundWinner) {
						p.Points++;
					}
				}
				foreach (AI ai in gameManager.AIs) {
					if (ai.Name == gameManager.roundWinner) {
						ai.Points++;
					}
				}
				return "0";
			}
			else if (command.StartsWith("!game.numPlayers")) {
				return (gameManager.AIs.Count + gameManager.players.Count).ToString();
			}
			else if (command.StartsWith("!game.viewPoints")) {
				string tally = "";

				foreach (Player p in gameManager.players) {
					tally += p.Name + "\t\t" + p.Points + "\n";
				}
				foreach (AI ai in gameManager.AIs) {
					tally += ai.Name + "\t\t" + ai.Points + "\n";
				}
				return tally;
			}
			else if (command.StartsWith("!game.playerTimeout")) {
				foreach (Player p in gameManager.waitingFor) {
					gameManager.afkPlayers.Add(p);
					gameManager.players.Remove(p);
				}

				return "0";
			}
			else if (command.StartsWith("!game.quit")) {
				Environment.Exit(0);
				return "0";
			}
			else if (command.StartsWith("!help")) {
				return "|command| \t\t |function| \n" +
					   "!game.start \t\t starts the game (do so only after all players have joined) \n" +
					   "!game.addAI \t\t adds a rando cardrisian (a player bot that picks cards randomly) \n" +
					   "!game.quit \t\t ends the game (not gracefully for clients midgame) \n";
			}
			else if (command.StartsWith("!chat.sendMessage")) {
				foreach (Player p in gameManager.players) {
					p.messages.Push(command.Substring(command.IndexOf('|') + 1));
				}

				return "0";
			}
			else if (command.StartsWith("!chat.getMessages")) {
				string name = command.Substring(command.IndexOf('|') + 1);
				string messages = "";
				foreach (Player p in gameManager.players) {
					if (name == p.Name) {
						while (p.messages.Count != 0) {
							messages += p.messages.Pop() + "|";
						}
					}
				}
				if (messages != "") {
					messages = messages.Substring(0, messages.LastIndexOf('|'));
					return messages;
				}
				else {
					return "0";
				}

			}
			else if (command == "\n") {
				return "0";
			}
			else {
				return "Unknown Command";
			}
		}

		static string[] parseFields(string command) {
			command = command.Substring(command.IndexOf('|') + 1);
			string[] playerinfo = command.Split('`');

			return playerinfo;
		}

		public static int returnPointThresh() {
			return gameManager.pointThreshold;
		}

		static void DisplayLogo() {
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
