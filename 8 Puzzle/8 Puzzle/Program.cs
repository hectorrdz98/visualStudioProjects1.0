using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _8_Puzzle
{
    class Program
    {
        static int[,] endGame = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        static List<MiniArea> posibles;
        static List<string> alreadyVisited;
        static OrderWeights orderWeights;

        static void Main(string[] args)
        {
            /*int[,] game = {
                { 1, 2, 3 },
                { 4, 0, 6 },
                { 7, 5, 8 }
            };*/
            /*int[,] game = {
                { 8, 3, 4 },
                { 1, 5, 6 },
                { 2, 7, 0 }
            };*/
            int[,] game = {
                { 1, 3, 6 },
                { 4, 2, 8 },
                { 7, 0, 5 }
            };

            posibles = new List<MiniArea>();
            alreadyVisited = new List<string>();
            orderWeights = new OrderWeights();

            Console.WriteLine("\n---------------- GAME -----------------\n");
            printGame(game);
            Console.WriteLine("\n");
            Console.WriteLine("Best steps: 0");
            Console.WriteLine("Attempted: 0");
            // Console.WriteLine("\n\n-------- ALREADY VISITED LIST ---------");

            process(game);
        }

        static void process(int[,] game)
        {

            MiniArea actual = new MiniArea(game, null, 0, 0);
            actual.weight = getWeight(actual);
            posibles.Add(actual);

            while (posibles.Any())
            {
                actual = posibles.First();
                // Console.WriteLine("\nActual: ");
                // printGame(actual.data);
                // Console.WriteLine("Weight: " + actual.weight);
                // Console.WriteLine("Steps: " + actual.steps + "\n");

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 3);
                Console.WriteLine("Best steps: " + actual.steps);
                Console.WriteLine("Attempted: " + alreadyVisited.Count());

                if (getWrong(actual.data) == 0) break;

                string dataString = actual.dataString();
                alreadyVisited.Add(dataString);
                
                // Console.WriteLine(dataString);

                // Console.WriteLine("Getting neighbors");

                foreach (MiniArea neighbor in actual.getNeighbors())
                {
                    if (!alreadyVisited.Contains(neighbor.dataString()))
                    {
                        // Console.WriteLine("The neighbor is not in the visited list");
                        neighbor.weight = getWeight(neighbor);
                        // Console.WriteLine("neighbor.weight: " + neighbor.weight);
                        posibles.Add(neighbor);
                    } else {
                        // Console.WriteLine("The neighbor is in the visited list");
                    }
                }

                posibles.Remove(actual);
                posibles.Sort(orderWeights);

                /*Console.WriteLine("\n-------- ALREADY VISITED LIST ---------");
                for (int i = 0; i < alreadyVisited.Count; i++)
                {
                    Console.WriteLine(alreadyVisited[i]);
                }
                Console.WriteLine("\n---------------------------------------");

                Console.WriteLine("\n------------ POSIBLES LIST ------------");
                for (int i = 0; i<posibles.Count; i++)
                {
                    printGame(posibles[i].data);
                    Console.WriteLine();
                }
                Console.WriteLine("\n---------------------------------------");*/
            }

            Console.WriteLine("\n\nPath: \n");

            recreatePath(actual);

            Console.WriteLine("\nVictory with {0} steps\n", actual.steps);
        }

        static int getWeight(MiniArea actual)
        {
            return actual.steps + getWrong(actual.data);
        }

        static int getWrong(int[,] game)
        {
            int cont = 0;
            for (int i = 0; i < game.GetLength(0); i++)
                for (int j = 0; j < game.GetLength(1); j++)
                    if (game[i,j] != 0)
                        if (game[i,j] != endGame[i,j]) cont++;
            return cont;
        }

        static void recreatePath(MiniArea actual)
        {
            List<MiniArea> paths = new List<MiniArea>();
            while (actual != null)
            {
                paths.Add(actual);
                actual = actual.father;
            }
            paths.Reverse();
            foreach(MiniArea path in paths)
            {
                printGame(path.data);
                Console.WriteLine();
            }
        }

        static void printGame(int[,] game)
        {
            for (int i = 0; i<game.GetLength(0); i++)
            {
                for (int j = 0; j<game.GetLength(1); j++)
                    Console.Write(game[i,j] + " ");
                Console.WriteLine();
            }
        }
    }
}
