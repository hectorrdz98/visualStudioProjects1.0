using System;
using System.Collections.Generic;
using System.IO;
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

        private void LadoDerecho(bool ejecutar = true, int limit = -1)
        {
            if (getClasificacion() == c.ST)
            {
                string simbolo = getContenido();
                Match(c.ST);

                if (getClasificacion() == c.Epsilon)
                {
                    if (ejecutar)
                        GeneraIf(simbolo);
                    Match(c.Epsilon);
                }
                else if (getClasificacion() == c.Or)
                {
                    if (ejecutar)
                        GeneraIf(simbolo);
                    Match(c.Or);
                    if (ejecutar)
                        WriteLine("else");

                    bool vinoPar = getClasificacion() == c.ParentesisIzq;

                    if (!vinoPar)
                        numTabs++;
                    LadoDerecho(ejecutar, 1);
                    if (!vinoPar)
                        numTabs--;
                }
                else
                {
                    if (ejecutar)
                        GeneraMatch(simbolo);
                }
            }
            else if (getClasificacion() == c.SNT)
            {
                if (ejecutar)
                    WriteLine(getContenido() + "();");

                Match(c.SNT);
            }
            else if (getClasificacion() == c.ParentesisIzq)
            {
                long position = archivo.Position;
                Match(c.ParentesisIzq);
                string firstMatch = getContenido();
                bool wasEpsilon = false;
                LadoDerecho(false);
                Match(c.ParentesisDer);

                if (getClasificacion() == c.Epsilon)
                {
                    if (ejecutar)
                        GeneraIf(firstMatch, false);
                    Match(c.Epsilon);
                    wasEpsilon = true;
                }

                archivo.Seek(position, SeekOrigin.Begin);
                nextToken(); // Leaves us after (

                if (ejecutar)
                    WriteLine("{");

                numTabs++;
                LadoDerecho(ejecutar);
                numTabs--;

                Match(c.ParentesisDer);

                if (wasEpsilon)
                    Match(c.Epsilon);

                if (ejecutar)
                    WriteLine("}");
            }

            if (archivo.Position >= archivo.Length)
                return;

            // Max recursive limit
            limit--;
            if (limit == 0)
                return;

            if (getClasificacion() != c.FinProduccion &&
                getClasificacion() != c.ParentesisDer &&
                getClasificacion() != c.Flechita)
                LadoDerecho(ejecutar, limit);
        }

        private void GeneraMatch(string simbolo)
        {
            if (simbolo[0] == '\\')
                WriteLine("Match(\"" + simbolo.Substring(1) + "\");");
            else
            {
                if (EsClasificacion(simbolo))
                    WriteLine("Match(c." + simbolo + ");");
                else
                    WriteLine("Match(\"" + simbolo + "\");");
            }
        }

        private void GeneraIf(string simbolo, bool generaMatch = true)
        {
            if (simbolo[0] == '\\')
                WriteLine("if (getContenido() == \"" + simbolo.Substring(1) + "\")");
            else
            {
                if (EsClasificacion(simbolo))
                    WriteLine("if (getClasificacion() == c." + simbolo + ")");
                else
                    WriteLine("if (getContenido() == \"" + simbolo + "\")");
            }

            numTabs++;
            if (generaMatch)
                GeneraMatch(simbolo);
            numTabs--;
        }

        private bool EsClasificacion(string contenido)
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
