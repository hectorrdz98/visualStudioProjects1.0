using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
{
    class Lenguaje : Sintaxis
    {
        bool firstMethod = false;
        int numTabs = 0;
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
                WriteLine("using System;");
                WriteLine("using System.Collections.Generic;");
                WriteLine("using System.Linq;");
                WriteLine("using System.Text;");
                WriteLine("using System.Threading.Tasks;");
                WriteLine("");
                WriteLine("namespace Generador");
                WriteLine("{");
                {
                    numTabs++;
                    WriteLine("class Lenguaje : Sintaxis");
                    WriteLine("{");
                    {
                        numTabs++;
                        WriteLine("public Lenguaje() : base()");
                        WriteLine("{");
                        WriteLine("");
                        WriteLine("}");
                        WriteLine("public Lenguaje(string filePath) : base(filePath)");
                        WriteLine("{");
                        WriteLine("");
                        WriteLine("}");
                        WriteLine("");

                        Producciones();

                        numTabs--;
                    }
                    WriteLine("}");
                    numTabs--;
                }
                WriteLine("}");
            }
            Match("}");
        }

        private void Producciones()
        {
            if (firstMethod)
            {
                WriteLine("private void " + getContenido() + "()");
            }
            else
            {
                WriteLine("public void " + getContenido() + "()");
                CreateProgramCS(getContenido());
                firstMethod = true;
            }
            Match(c.SNT);
            Match(c.Flechita);

            WriteLine("{");

            numTabs++;
            LadoDerecho();
            numTabs--;

            WriteLine("}");
            WriteLine("");

            Match(c.FinProduccion);
            if (getContenido() != "}")
                Producciones();
        }

        private void LadoDerecho()
        {
            if (getClasificacion() == c.ST)
            {
                if (getContenido()[0] == '\\')
                    WriteLine("Match(\"" + getContenido().Substring(1) + "\");");
                else
                    WriteLine("Match(\"" + getContenido() + "\");");
                Match(c.ST);
            }
            else if (getClasificacion() == c.SNT)
            {
                if (ClaseToken(getContenido()))
                    WriteLine("Match(c." + getContenido() + ");");
                else
                    WriteLine(getContenido() + "();");
                Match(c.SNT);
            }
            else if (getClasificacion() == c.ParentesisIzq)
            {
                WriteLine("{");
                Match(c.ParentesisIzq);

                numTabs++;
                LadoDerecho();
                numTabs--;

                Match(c.ParentesisDer);
                WriteLine("}");
            }

            if (getClasificacion() != c.FinProduccion &&
                getClasificacion() != c.ParentesisDer)
                LadoDerecho();
        }

        private bool ClaseToken(string contenido)
        {
            string[] nums = { "Identificador", "Constante", "Numero", "If", "ForEach", "TipoDato", "String" };
            return nums.Contains(contenido);
        }

        private void WriteLine(string contenido)
        {
            for (int i = 0; i < numTabs; i++) lenguaje.Write("\t");
            lenguaje.WriteLine(contenido);
        }

        private void CreateProgramCS(string firstProduction) {
            program.WriteLine("using System;");
            program.WriteLine("using System.Collections.Generic;");
            program.WriteLine("using System.Linq;");
            program.WriteLine("using System.Text;");
            program.WriteLine("using System.Threading.Tasks;");
            program.WriteLine("");

            program.WriteLine("namespace Generador");
            program.WriteLine("{");
            {
                program.WriteLine("\tclass Program");
                program.WriteLine("\t{");
                {
                    program.WriteLine("\t\tstatic void Main(string[] args)");
                    program.WriteLine("\t\t{");
                    {
                        program.WriteLine("\t\t\ttry");
                        program.WriteLine("\t\t\t{");
                        {
                            program.WriteLine("\t\t\t\tLenguaje L = new Lenguaje(\"C:\\\\archivos\\\\prueba.cs\\\\\");");
                            program.WriteLine("");

                            program.WriteLine("\t\t\t\tL." + firstProduction + "();");
                            program.WriteLine("");

                            program.WriteLine("\t\t\t\tConsole.ReadKey();");
                            program.WriteLine("\t\t\t\tConsole.WriteLine();");
                            program.WriteLine("\t\t\t\tL.closeFiles();");
                        }
                        program.WriteLine("\t\t\t}");
                        program.WriteLine("\t\t\tcatch (MyException) { }");
                    }
                    program.WriteLine("\t\t}");
                }
                program.WriteLine("\t}");
            }
            program.WriteLine("}");
        }
    }
}
