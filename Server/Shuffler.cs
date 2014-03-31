using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class Shuffler
    {
        private System.Random _rng;

        public Shuffler()
        {
            _rng = new Random();
        }

        public void Shuffle<T>(IList<T> array)
        {
            for (int n = array.Count; n > 1; )
            {
                int k = _rng.Next(n);
                --n;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
