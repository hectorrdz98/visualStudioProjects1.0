using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_Puzzle
{
    class OrderWeights : IComparer<MiniArea>
    {
        public int Compare(MiniArea x, MiniArea y)
        {
            if (x.weight > y.weight)
                return 1;
            else
                return 0;

        }
    }
}
