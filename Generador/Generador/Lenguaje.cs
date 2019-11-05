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
                // https://pastebin.com/VxN0hr2v
                lenguaje.WriteLine("using System;");
                lenguaje.WriteLine("using System.Collections.Generic;");
                lenguaje.WriteLine("using System.Linq;");
                lenguaje.WriteLine("using System.Text;");
                lenguaje.WriteLine("using System.Threading.Tasks;");
                lenguaje.WriteLine();
                lenguaje.WriteLine("namespace Generador");
                lenguaje.WriteLine("{");
                {
                    lenguaje.WriteLine("\tclass Lenguaje : Sintaxis");
                    lenguaje.WriteLine("\t{");
                    {
                        lenguaje.WriteLine("\t\tpublic Lenguaje() : base()");
                        lenguaje.WriteLine("\t\t{");
                        lenguaje.WriteLine();
                        lenguaje.WriteLine("\t\t}");
                        lenguaje.WriteLine("\t\tpublic Lenguaje(string filePath) : base(filePath)");
                        lenguaje.WriteLine("\t\t{");
                        lenguaje.WriteLine();
                        lenguaje.WriteLine("\t\t}");

                        Producciones();
                    }
                    lenguaje.WriteLine("\t}");
                }
                lenguaje.WriteLine("}");
            }
            Match("}");
        }

        private void Producciones()
        {
            lenguaje.WriteLine("\t\tpublic void " + getContenido() + "()");
            Match(c.SNT);
            Match(c.Flechita);

            lenguaje.WriteLine("\t\t{");
            LadoDerecho();
            lenguaje.WriteLine("\t\t}");
            lenguaje.WriteLine();

            Match(c.FinProduccion);
            if (getContenido() != "}")
                Producciones();
        }

        private void LadoDerecho()
        {
            if (getClasificacion() == c.ST)
            {
                lenguaje.WriteLine("\t\t\tMatch(\"" + getContenido() + "\");");
                Match(c.ST);

            }
            else if (getClasificacion() == c.SNT)
            {
                lenguaje.WriteLine("\t\t\t" + getContenido() + "();");
                Match(c.SNT);
            }
            else if (getClasificacion() == c.ParentesisIzq)
            {
                lenguaje.WriteLine("\t\t\t{");
                Match(c.ParentesisIzq);
                LadoDerecho();
                Match(c.ParentesisDer);
                lenguaje.WriteLine("\t\t\t}");
            }

            if (getClasificacion() != c.FinProduccion &&
                getClasificacion() != c.ParentesisDer)
                LadoDerecho();
        }
    }
}
