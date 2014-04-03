using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AI : Player
    {
        public List<string> hand;

        public AI()
        {
            hand = new List<string>();

        }

        public void SeperateHand(string handString)
        {
            hand.AddRange(handString.Split('`').ToList());
        }

    }
}
