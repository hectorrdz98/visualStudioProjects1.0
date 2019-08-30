using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class LexicoExcepcion : Exception
    {
        public LexicoExcepcion(string message) : base(message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public LexicoExcepcion(string message, string highlighted) : base(message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nError: " + message);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" " + highlighted + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
