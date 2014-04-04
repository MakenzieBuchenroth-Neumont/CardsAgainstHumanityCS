using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public struct PlayInfo
    {
        public string card;
        public string cardPlayer;

        public PlayInfo(string card, string cardPlayer)
        {
            this.card = card;
            this.cardPlayer = cardPlayer;
        }
    }

    public class GameManager
    { 
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

        public string roundWinner;

        public int numAIs;

        public int pointThreshold;

        public GameManager(int maxCards, int pointThreshold)
        {
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

        public GameManager()
        {
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

        public void incrementCzarCounter()
        {
        
            if (this.CzarCounter != this.players.Count-1)
            {
                this.CzarCounter++;
            }
            else
            {
                this.CzarCounter = 0;
            }
            
        }

        public void NewRound()
        {
            waitingFor.AddRange(players);
            this.currentBlackCard = blackDeck.Pop();
            currentPlayerCards.Clear();
            incrementCzarCounter();
            roundWinner = "wait";
        }

        public int numFields(string qhuest)
        {
            int count = 0;
            foreach (char c in qhuest)
            {
                if (c == '_') count++;
            }

            return count / 3;
        }

        public bool played()
        {
            if (currentPlayerCards.Count == numFields(currentBlackCard.text) * (players.Count - 1 + AIs.Count))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
