using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Card
    {
        public string text { get; set; }
        public string definitionProvider { get; set; }
        public string definitionText { get; set; }

        public override string ToString()
        {
            return text + "\n";// + "-" + definitionProvider + "-" + definitionText + "\n";
        }

        public string toHand()
        {
            return text;
        }
    }

    public class CardSet
    {
        public string cardSetName { get; set; }
        public string cardSetType { get; set; }
        public List<Card> cards { get; set; }
    }
}
