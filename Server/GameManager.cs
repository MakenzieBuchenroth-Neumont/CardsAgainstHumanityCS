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

        public List<PlayInfo> currentPlayerCards;

        public int CzarCounter;

        public bool gameStarted;

        public int maxCards;

        public Card currentBlackCard;

        public string roundWinner;

        public GameManager()
        {
            this.whiteDeck = new Stack<Card>();
            this.blackDeck = new Stack<Card>();
            this.players = new List<Player>();
            this.currentPlayerCards = new List<PlayInfo>();
            this.CzarCounter = 0;
            this.gameStarted = false;
            this.maxCards = 10;
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
            this.currentBlackCard = blackDeck.pop();
            currentPlayerCards.Clear();
            roundWinner = "wait";
        }

        public bool played()
        {
            if (currentPlayerCards.Count == players.Count-1)
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
