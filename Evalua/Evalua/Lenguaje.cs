using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Lenguaje : Sintaxis
    {
        List<Variable> variables;
        public Lenguaje() : base()
        {
            variables = new List<Variable>();
        }
        public Lenguaje(string filePath) : base(filePath)
        {
            variables = new List<Variable>();
        }

        public void Programa()
        {
            Librerias();
            Main();
            imprimeVariables();
        }

        private void Librerias()
        {
            Match("using");
            Match(c.Identificador);
            if (getContenido() == ".")
                SubLibrerias();
            Match(c.FinSentencia);
            if (getContenido() == "using")
                Librerias();
        }

        private void SubLibrerias()
        {
            Match(".");
            Match(c.Identificador);
            if (getContenido() == ".")
                SubLibrerias();
        }

        private void Main()
        {
            Match("namespace");
            Match(c.Identificador);
            Match(c.InicioBloque);
            {
                Match("class");
                Match("Program");
                Match(c.InicioBloque);
                {
                    Match("static");
                    Match("void");
                    Match("Main");
                    Match("(");
                    Match("string");
                    Match("[");
                    Match("]");
                    Match("args");
                    Match(")");
                    BloqueDeInstrucciones();
                }
                Match(c.FinBloque);
            }
            Match(c.FinBloque);
        }

        private void BloqueDeInstrucciones()
        {
            Match(c.InicioBloque);
            {
                Instrucciones();
            }
            Match(c.FinBloque);
        }

        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                Match(getContenido());
                Match(".");

                switch(getContenido())
                {
                    case "WriteLine":
                    case "Write":
                        {
                            string typeOutput = getContenido();
                            Match(getContenido());
                            Match("(");
                            if (getClasificacion() == c.Cadena)
                            {
                                string output = getContenido();
                                if (typeOutput == "Write") Console.Write(output.Substring(1, output.Length - 2));
                                else Console.WriteLine(output.Substring(1, output.Length - 2));
                                Match(c.Cadena);
                            }
                            else
                            {
                                if (getContenido() != ")")
                                {
                                    if (existeVariable(getContenido()))
                                    {
                                        Variable v = getVariable(getContenido());
                                        if (typeOutput == "Write") Console.Write(v.getValor());
                                        else Console.WriteLine(v.getValor());
                                        Match(c.Identificador);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis: Variable referenciada no existente - " + getContenido());
                                            throw new MyException("Error de sintaxis: Variable referenciada no existente - ", getContenido());
                                        }
                                        finally { closeFiles(); }
                                    }
                                }
                                else
                                {
                                    if (typeOutput != "Write") Console.WriteLine();
                                }
                            }
                            Match(")");
                        } break;
                    case "ReadLine":
                    case "Read":
                    case "ReadKey":
                        {
                            string typeInput = getContenido();
                            Match(getContenido());
                            if (typeInput == "ReadLine") Console.ReadLine();
                            else if (typeInput == "Read") Console.Read();
                            else Console.ReadKey();
                            Match("(");
                            Match(")");
                        }
                        break;
                    default:
                        {
                            try
                            {
                                log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis: Se espera una entrada o salida");
                                throw new MyException("Error de sintaxis: Se espera", "una entrada o salida");
                            }
                            finally { closeFiles(); }
                        }
                }
                Match(c.FinSentencia);
            }
            else if (getClasificacion() == c.TipoDato)
            {
                Variable.t tipoVariable = Variable.stringToT(getContenido());
                Match(c.TipoDato);
                ListaDeIdentificadores(tipoVariable);
                Match(c.FinSentencia);
            }
            else if (getClasificacion() == c.Constante)
            {
                Match(c.Constante);
                Variable.t tipoVariable = Variable.stringToT(getContenido());
                Match(c.TipoDato);
                ListaDeConstantes(tipoVariable);
                Match(c.FinSentencia);
            }

            // id = Expresion;
            // id = Console.ReadLine(); Requerimiento #7
            // id = Console.Read(); Requerimiento #7
            // id = Console.ReadKey(); Requerimiento #7
            // if
        }

        private void Instrucciones()
        {
            Instruccion();
            if (getClasificacion() != c.FinBloque)
                Instrucciones();
        }

        private void ListaDeIdentificadores(Variable.t tipoVariable)
        {
            string variable = getContenido();
            Match(c.Identificador);
            if (!existeVariable(variable))
            {
                string valor = "";
                if (getClasificacion() == c.Asignacion)
                {
                    Match(c.Asignacion);
                    if (getContenido() == "Console")
                    {
                        Match("Console");
                        Match(".");
                        if (getContenido() == "ReadLine")
                        {
                            Match("ReadLine");
                            valor = Console.ReadLine();
                        }
                        else if (getContenido() == "Read")
                        {
                            Match("Read");
                            valor = Convert.ToChar(Console.Read()).ToString();
                        }
                        else
                        {
                            Match("ReadKey");
                            valor = Console.ReadKey().KeyChar.ToString();
                        }
                        Match("(");
                        Match(")");

                    }
                    else if (getClasificacion() == c.Cadena)
                    {
                        string output = getContenido();
                        valor = output.Substring(1, output.Length - 2);
                        Match(c.Cadena);
                    }
                    else if (getClasificacion() == c.Numero)
                    {
                        valor = getContenido();
                        Match(c.Numero);
                    }
                    /* 
                    else 
                    {
                        valor = ExpresionMatematica();
                    }
                    */
                }

                variables.Add(new Variable(variable, tipoVariable, valor, false));

                if (getContenido() == ",")
                {
                    Match(",");
                    ListaDeIdentificadores(tipoVariable);
                }
            }
            else
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis: La variable " + variable + " está duplicada");
                    throw new MyException("Error de sintaxis: La variable " + variable + " está duplicada");
                }
                finally { closeFiles(); }
            }
            
        }

        private void ListaDeConstantes(Variable.t tipoVariable)
        {
            string constante = getContenido();
            Match(c.Identificador);
            if (!existeVariable(constante))
            {
                string valor = "";
                Match(c.Asignacion);
                if (getContenido() == "Console")
                {
                    Match("Console");
                    Match(".");
                    if (getContenido() == "ReadLine")
                    {
                        Match("ReadLine");
                        valor = Console.ReadLine();
                    }
                    else if (getContenido() == "Read")
                    {
                        Match("Read");
                        valor = Convert.ToChar(Console.Read()).ToString();
                    }
                    else
                    {
                        Match("ReadKey");
                        valor = Console.ReadKey().KeyChar.ToString();
                    }
                    Match("(");
                    Match(")");

                }
                else if (getClasificacion() == c.Numero)
                {
                    valor = getContenido();
                    Match(c.Numero);
                }
                /* 
                else 
                {
                    valor = ExpresionMatematica();
                }
                */

                variables.Add(new Variable(constante, tipoVariable, valor, true));

                if (getContenido() == ",")
                {
                    Match(",");
                    ListaDeConstantes(tipoVariable);
                }
            }
            else
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis: La constante " + constante + " está duplicada");
                    throw new MyException("Error de sintaxis: La constante " + constante + " está duplicada");
                }
                finally { closeFiles(); }
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach(Variable v in variables)
            {
                if (v.getNombre() == nombre) return true;
            }
            return false;
        }

        private Variable getVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombre) return v;
            }
            return null;
        }

        private void setValor(string nombreV, string valorV)
        {
            Variable v = getVariable(nombreV);
            if (v != null) v.setValor(valorV);
        }

        private void imprimeVariables()
        {
            foreach (Variable v in variables)
            {
                log.WriteLine("Nombre: " + v.getNombre());
                log.WriteLine("Tipo: " + v.getTipo());
                log.WriteLine("Valor: " + v.getValor());
                log.WriteLine(v.esVariable() ? "Es variable" : "Es constante");
            }
        }

    }
}
