using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class Shuffler
    {
        private Random rand;

        public Shuffler()
        {
            rand = new Random();
        }

        public void Shuffle<T>(List<T> list)
        {
            for (int n = list.Count-1; n > 0; n--)
            {
                int k = rand.Next(n);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
    }
}
