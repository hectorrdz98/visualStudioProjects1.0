using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Lenguaje L = new Lenguaje("C:\\archivos\\prueba.gram");

                L.Gramatica();

                Console.ReadKey();
                Console.WriteLine();
                L.closeFiles();
            }
            catch (MyException) { }
        }
    }
}
