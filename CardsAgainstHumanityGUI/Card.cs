using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHumanityGUI
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
    }

    public class RootObject
    {
        public string cardSetName { get; set; }
        public string cardSetType { get; set; }
        public List<Card> cards { get; set; }
    }
}
