using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
	public struct PlayInfo {
		public string card;
		public string cardPlayer;

		public PlayInfo(string card, string cardPlayer) {
			this.card = card;
			this.cardPlayer = cardPlayer;
		}
	}

	public class GameManager {
		public Stack<Card> whiteDeck;
		public Stack<Card> blackDeck;
		public List<Player> players;
		public List<Player> afkPlayers;
		public List<AI> AIs;
		public List<PlayInfo> currentPlayerCards;
		public List<Player> waitingFor;
		public int CzarCounter;
		public bool gameStarted;
		public int maxCards;
		public Card currentBlackCard;
		public string roundWinner = "wait";
		public int numAIs;
		public int pointThreshold;
		public string winCards;

		public GameManager(int maxCards, int pointThreshold) {
			whiteDeck = new Stack<Card>();
			blackDeck = new Stack<Card>();
			players = new List<Player>();
			AIs = new List<AI>();
			currentPlayerCards = new List<PlayInfo>();
			waitingFor = new List<Player>();
			afkPlayers = new List<Player>();
			CzarCounter = 0;
			gameStarted = false;
			this.maxCards = maxCards;
			this.pointThreshold = pointThreshold;
		}

		public GameManager() {
			whiteDeck = new Stack<Card>();
			blackDeck = new Stack<Card>();
			players = new List<Player>();
			AIs = new List<AI>();
			currentPlayerCards = new List<PlayInfo>();
			waitingFor = new List<Player>();
			afkPlayers = new List<Player>();
			CzarCounter = 0;
			gameStarted = false;
			maxCards = 10;
			pointThreshold = 10;
		}

		public event Action<string> CzarChanged;

		public void incrementCzarCounter() {
			foreach (var player in players) {
				player.IsCzar = false;
			}

			if (CzarCounter >= players.Count - 1) {
				CzarCounter = 0;
			}
			else {
				CzarCounter++;
			}

			players[CzarCounter].IsCzar = true;
			CzarChanged?.Invoke(players[CzarCounter].Name);
		}

		public void NewRound() {
			waitingFor.AddRange(players);
			waitingFor.RemoveAt(CzarCounter);
			this.currentBlackCard = blackDeck.Pop();
			currentPlayerCards.Clear();
			roundWinner = "wait";
			winCards = "";
		}

		public int numFields(string qhuest) {
			int count = 0;
			foreach (char c in qhuest) {
				if (c == '_') count++;
			}

			return count / 3;
		}

		public bool played() {
			if (currentPlayerCards.Count == players.Count - 1 + AIs.Count) {
				incrementCzarCounter();
				return true;
			}
			else {
				return false;
			}
		}

	}
}
