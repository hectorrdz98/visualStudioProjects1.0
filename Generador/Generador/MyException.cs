using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
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
            int init = message.IndexOf("###");
            bool flag = false;

            message = message.Replace("###", "");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            for (int i = 0; i < message.Length; i++)
            {
                if (i == init)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(highlighted);
                    Console.ForegroundColor = ConsoleColor.Red;
                    flag = true;
                }
                Console.Write(message[i]);
            }
            if (!flag)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(highlighted);
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
