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
                Lexico L = new Lexico("C:\\archivos\\prueba3.cs");
                while (!L.archivo.EndOfStream)
                {
                    L.nextToken();
                    Console.WriteLine(L.getContenido() + " " + L.getClasificacion());
                }
                Console.ReadKey();
            }
            catch (LexicoExcepcion) { };
        }
    }
}
