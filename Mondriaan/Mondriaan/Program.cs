using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mondriaan
{
    class Program
    {

        static int[] block = { 1, 2 };
        static int nSols = 0;

        static void Main(string[] args)
        {
            int[][] game = new int[][]
            {
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[]{ 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            Console.WriteLine("The game is {0} x {1} with {2} x {3} blocks:\n", game.Length, game[0].Length, block[0], block[1]);

            DateTime init = DateTime.Now;
            runDetection(game, 1);
            DateTime end = DateTime.Now;

            Console.WriteLine("\nThe # of solutions was: {0}\n", nSols);
            Console.WriteLine("Time: {0} seg\n", (end - init));
        }

        static void runDetection(int[][] preGame, int design)
        {
            bool flag = false;
            for (int x = 0; x < preGame.Length; x++)
            {
                for (int y = 0; y < preGame[x].Length; y++)
                    if (preGame[x][y] == 0)
                    {
                        runMovements(preGame, x, y, design);
                        flag = true;
                        break;
                    }
                if (flag) break;
            }
                
            if (finished(preGame)) nSols++;
        }

        static void runMovements(int [][] preGame, int x, int y, int design)
        {
            for (int i = 0; i<2; i++)
            {
                if (isValidPlace(preGame, x, y, i))
                {
                    int[][] game = copyGame(preGame);
                    fillGame(game, x, y, i, design);
                    runDetection(game, design + 1);
                }
            }
        }

        static bool isValidPlace(int [][] preGame, int x, int y, int moveId)
        {
            try
            {
                for (int i = 0; i < block[0]; i++)
                    for (int j = 0; j < block[1]; j++)
                    {
                        int valueSpace = 1;
                        switch (moveId)
                        {
                            case 0: valueSpace = preGame[x + i][y + j]; break;
                            case 1: valueSpace = preGame[x + j][y - i]; break;
                        }
                        if (valueSpace != 0) return false;
                    }   
                return true;
            }
            catch (Exception) { }

            return false;
        }

        static bool finished(int[][] game)
        {
            foreach (int[] row in game)
                foreach (int value in row)
                    if (value == 0) return false;
            return true;
        }

        static int[][] copyGame(int[][] preGame)
        {
            return new int[][]
            {
                new int[]{ preGame[0][0], preGame[0][1], preGame[0][2], preGame[0][3], preGame[0][4], preGame[0][5], preGame[0][6], preGame[0][7] },
                new int[]{ preGame[1][0], preGame[1][1], preGame[1][2], preGame[1][3], preGame[1][4], preGame[1][5], preGame[1][6], preGame[1][7] },
                new int[]{ preGame[2][0], preGame[2][1], preGame[2][2], preGame[2][3], preGame[2][4], preGame[2][5], preGame[2][6], preGame[2][7] },
                new int[]{ preGame[3][0], preGame[3][1], preGame[3][2], preGame[3][3], preGame[3][4], preGame[3][5], preGame[3][6], preGame[3][7] },
                new int[]{ preGame[4][0], preGame[4][1], preGame[4][2], preGame[4][3], preGame[4][4], preGame[4][5], preGame[4][6], preGame[4][7] },
                new int[]{ preGame[5][0], preGame[5][1], preGame[5][2], preGame[5][3], preGame[5][4], preGame[5][5], preGame[5][6], preGame[5][7] },
                new int[]{ preGame[6][0], preGame[6][1], preGame[6][2], preGame[6][3], preGame[6][4], preGame[6][5], preGame[6][6], preGame[6][7] },
            };
        }

        static void fillGame(int[][] game, int x, int y, int moveId, int design)
        {
            for (int i = 0; i < block[0]; i++)
                for (int j = 0; j < block[1]; j++)
                    switch (moveId)
                    {
                        case 0: game[x + i][y + j] = design; break;
                        case 1: game[x + j][y - i] = design; break;
                    }
        }
    }
}
