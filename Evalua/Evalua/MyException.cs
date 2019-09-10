using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class MyException : Exception
    {
        public MyException(string message) : base(message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public MyException(string message, string highlighted) : base(message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n"+message);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" " + highlighted + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
