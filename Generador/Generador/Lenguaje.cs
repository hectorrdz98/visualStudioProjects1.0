using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
{
    class Lenguaje : Sintaxis
    {
        public Lenguaje() : base()
        {

        }
        public Lenguaje(string filePath) : base(filePath)
        {

        }

        public void Gramatica()
        {
            Match("Lenguaje");
            Match(":");
            Match(c.SNT);
            Match(";");
            Match("{");
            {
                Producciones();
            }
            Match("}");
        }

        private void Producciones()
        {
            lenguaje.WriteLine("public void " + getContenido() + "()");
            Match(c.SNT);
            Match(c.Flechita);

            lenguaje.WriteLine("{");
            LadoDerecho();
            lenguaje.WriteLine("}");
            lenguaje.WriteLine();

            Match(c.FinProduccion);
            if (getContenido() != "}")
                Producciones();
        }

        private void LadoDerecho()
        {

        }
    }
}
