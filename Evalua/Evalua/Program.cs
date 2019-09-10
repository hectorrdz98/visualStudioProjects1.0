using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Lenguaje L = new Lenguaje("C:\\archivos\\prueba3.cs");
                /*while (!L.archivo.EndOfStream)
                {
                    L.nextToken();
                    Console.WriteLine(L.getContenido() + " " + L.getClasificacion());
                }*/
                // Checar esta secuencia: # include <iostream.h>
                L.Programa();
                Console.ReadKey();
                Console.WriteLine();
                L.closeFiles();
            }
            catch (MyException) { }
        }
    }
}
