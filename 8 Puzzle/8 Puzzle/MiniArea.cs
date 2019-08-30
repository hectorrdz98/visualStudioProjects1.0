using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_Puzzle
{
    class MiniArea
    {
        public int[,] data;
        public MiniArea father;
        public int weight;
        public int steps;

        public MiniArea(int[,] data, MiniArea father, int weight, int steps)
        {
            this.data = data;
            this.father = father;
            this.weight = weight;
            this.steps = steps;
        }

        public List<MiniArea> getNeighbors()
        {
            List<MiniArea> nbs = new List<MiniArea>();
            int[] whiteSpace = this.findWS();
            // Console.WriteLine("WhiteSpace in [{0}][{1}]", whiteSpace[0], whiteSpace[1]);

            int[,] moves = {
                { whiteSpace[0], whiteSpace[1] + 1 },
                { whiteSpace[0] + 1,     whiteSpace[1] },
                {     whiteSpace[0], whiteSpace[1] - 1 },
                { whiteSpace[0] - 1,     whiteSpace[1] }
            };

            for (int i = 0; i<4; i++)
                if (moves[i,0] >= 0 && moves[i,0] < data.GetLength(0) && moves[i,1] >= 0 && moves[i,1] < data.GetLength(1))
                {
                    // Console.WriteLine("[{0}][{1}] es un movimiento valido", moves[i,0], moves[i,1]);
                    int[,] neighbor = this.shuffle(whiteSpace[0], whiteSpace[1], moves[i,0], moves[i,1]);
                    nbs.Add(new MiniArea(neighbor, this, 9999, this.steps + 1));
                } else { // Console.WriteLine("[{0}][{1}] no es un movimiento valido", moves[i,0], moves[i,1]); 
                }

            return nbs;
        }

        public string dataString()
        {
            string res = "";
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    res += data[i, j];
            return res;
        }

        private int[] findWS()
        {
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    if (this.data[i,j] == 0) return new int[] { i, j };
            return null;
        }

        private int[,] shuffle(int wx, int wy, int x, int y)
        {
            int[,] res = new int[this.data.GetLength(0), this.data.GetLength(1)];
            for (int i = 0; i<data.GetLength(0); i++)
                for (int j = 0; j<data.GetLength(1); j++)
                {
                    if (i == wx && j == wy) res[i, j] = this.data[x, y];
                    else if (i == x && j == y) res[i, j] = this.data[wx, wy];
                    else res[i, j] = this.data[i, j];
                }

            /* Console.WriteLine("Suffled neighbor: ");

            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                    Console.Write(res[i, j] + " ");
                Console.WriteLine();
            }*/

            return res;
        }
    }
}
