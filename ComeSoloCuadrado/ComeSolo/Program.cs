using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComeSolo
{
    class Program
    {

        static int[] victories = {
            0, 0, 0, 0, 0,
            0, 0, 0, 0, 0,
            0, 0, 0, 0, 0,
            0, 0, 0, 0, 0
        };
        static int totalVictories = 0;

        static void Main(string[] args)
        {
            char[][] game = new char[][]{
                new char[]{ ' ', '*', '*', '*', '*' },
                new char[]{ '*', '*', '*', '*', '*' },
                new char[]{ '*', '*', '*', '*', '*' },
                new char[]{ '*', '*', '*', '*', '*' }
            };

            Console.WriteLine("Program started...");

            printGame(game);

            naturalMovement(game);

            Console.WriteLine("\nTotal of victories: {0}\n", totalVictories);
            Console.WriteLine("  {0} {1} {2} {3} {4}", victories[0], victories[1], victories[2], victories[3], victories[4]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", victories[5], victories[6], victories[7], victories[8], victories[9]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", victories[10], victories[11], victories[12], victories[13], victories[14]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", victories[15], victories[16], victories[17], victories[18], victories[19]);
            Console.WriteLine();
        }

        static void naturalMovement(char[][] game)
        {
            for (int x = 0; x< game.Length; x++)
            {
                for (int y = 0; y < game[x].Length; y++)
                {
                    if (game[x][y] == '*')
                        posibleSteps(game, new int[] { x, y });
                }
            }
            if (getRemain(game) == 1)
            {
                updateVictories(game);
            }
        }

        static void posibleSteps(char[][] preGame, int[] actPos)
        {
            int[][] moves = {
                new int[]{   actPos[0], actPos[1]+1 },
                new int[]{ actPos[0]+1,   actPos[1] },
                new int[]{   actPos[0], actPos[1]-1 },
                new int[]{ actPos[0]-1,   actPos[1] }
            };
            int[][] movesExtra = {
                new int[]{   actPos[0], actPos[1]+2 },
                new int[]{ actPos[0]+2,   actPos[1] },
                new int[]{   actPos[0], actPos[1]-2 },
                new int[]{ actPos[0]-2,   actPos[1] }
            };
            int cont = 0;

            foreach(int [] move in moves)
            {
                if (isValidPos(preGame, move)) {
                    if (preGame[move[0]][move[1]] == '*')
                    {
                        if (isValidPos(preGame, movesExtra[cont]))
                        {
                            if (preGame[movesExtra[cont][0]][movesExtra[cont][1]] == ' ')
                            {
                                char[][] game = copyGame(preGame);
                                game[actPos[0]][actPos[1]] = ' ';
                                game[move[0]][move[1]] = ' ';
                                game[movesExtra[cont][0]][movesExtra[cont][1]] = '*';
                                naturalMovement(game);
                            }
                        }
                    }
                }
                cont++;
            }
        }

        static bool isValidPos(char[][] game, int[] move)
        {
            if (move[0] >= 0 && move[1] >= 0)
            {
                try
                {
                    char temp = game[move[0]][move[1]];
                    return true;
                } catch (Exception) { };
            }

            return false;
        }

        static int getRemain(char[][] game)
        {
            int cont = 0;
            foreach (char[] row in game)
                foreach (char val in row)
                    if (val == '*')
                        cont++;
            return cont;
        }

        static void updateVictories(char[][] game)
        {
            int cont = 0;
            for (int x = 0; x < game.Length; x++)
            {
                for (int y = 0; y < game[x].Length; y++)
                {
                    if (game[x][y] == '*')
                    {
                        victories[cont]++;
                        totalVictories++;
                        return;
                    }
                    cont++;
                }
            }
        }

        static char[][] copyGame(char[][] preGame)
        {
            char[][] game = new char[][]{
                new char[]{ preGame[0][0], preGame[0][1], preGame[0][2], preGame[0][3], preGame[0][4] },
                new char[]{ preGame[1][0], preGame[1][1], preGame[1][2], preGame[1][3], preGame[1][4] },
                new char[]{ preGame[2][0], preGame[2][1], preGame[2][2], preGame[2][3], preGame[2][4] },
                new char[]{ preGame[3][0], preGame[3][1], preGame[3][2], preGame[3][3], preGame[3][4] },
            };

            return game;
        }

        static void printGame(char[][] game)
        {
            Console.WriteLine("  {0} {1} {2} {3} {4}", game[0][0], game[0][1], game[0][2], game[0][3], game[0][4]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", game[1][0], game[1][1], game[1][2], game[1][3], game[1][4]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", game[2][0], game[2][1], game[2][2], game[2][3], game[2][4]);
            Console.WriteLine("  {0} {1} {2} {3} {4}", game[3][0], game[3][1], game[3][2], game[3][3], game[3][4]);
        }
    }
}
