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
        List<Funcion> funciones;
        Stack<float> evalua;

        public Lenguaje() : base()
        {
            variables = new List<Variable>();
            funciones = new List<Funcion>();
            evalua = new Stack<float>();
        }
        public Lenguaje(string filePath) : base(filePath)
        {
            variables = new List<Variable>();
            funciones = new List<Funcion>();
            evalua = new Stack<float>();
        }

        public void Programa()
        {
            Librerias();
            Main();
            imprimeVariables();
            imprimeFunciones();
        }

        private void Librerias()
        {
            Match("using");
            string subLibreria = getContenido();
            Match(c.Identificador);
            if (getContenido() == ".")
            {
                subLibreria += SubLibrerias();
                if (subLibreria == "System.Math")
                {
                    Funcion abs = new Funcion("abs");
                    abs.func = new Func<float, float>((n) => { return Math.Abs(n); });
                    funciones.Add(abs);

                    Funcion sqrt = new Funcion("sqrt");
                    sqrt.func = new Func<float, float>((n) => { return (float) Math.Sqrt((double) n); });
                    funciones.Add(sqrt);

                    Funcion pow2 = new Funcion("pow2");
                    pow2.func = new Func<float, float>((n) => { return n * n; });
                    funciones.Add(pow2);

                    Funcion round = new Funcion("round");
                    round.func = new Func<float, float>((n) => { return (float) Math.Round(n); });
                    funciones.Add(round);

                    Funcion trunc = new Funcion("trunc");
                    trunc.func = new Func<float, float>((n) => { return (float) Math.Truncate(n); });
                    funciones.Add(trunc);
                }
            }
            Match(c.FinSentencia);
            if (getContenido() == "using")
                Librerias();
        }

        private string SubLibrerias()
        {
            string subLibreria = "";
            Match(".");
            subLibreria += "." + getContenido();
            Match(c.Identificador);
            if (getContenido() == ".")
                SubLibrerias();
            return subLibreria;
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
                if (getClasificacion() != c.FinBloque)
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
                                            log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                                                this.actColumn + ": Variable referenciada no existente - " + getContenido());
                                            throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                                                this.actColumn + ": Variable referenciada no existente - ", getContenido());
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
                                log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                                    this.actColumn + ": Se espera una entrada o salida");
                                throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                                    this.actColumn + ": Se espera ###", "una entrada o salida");
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
            else if (getClasificacion() == c.If)
            {
                IF();
            }
            else // Modificacion de variables
            {
                string variable = getContenido();
                Match(c.Identificador);
                if (existeVariable(variable))
                {
                    Match(c.Asignacion);
                    string valor = Asignacion(variable);

                    setValor(variable, valor);
                    Match(c.FinSentencia);
                }
                else
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": La variable " + variable + " no está declarada");
                        throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": La variable ### no está declarada", variable);
                    }
                    finally { closeFiles(); }
                }
            }
        }

        private void Instrucciones()
        {
            Instruccion();
            if (getClasificacion() != c.FinBloque)
                Instrucciones();
        }

        private void IF()
        {
            Match(c.If);
            Match("(");
            Expresion();
            Match(c.OperadorRelacional);
            Expresion();
            Match(")");

            if (getClasificacion() == c.InicioBloque)
            {
                BloqueDeInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (getContenido() == "else")
            {
                Match("else");
                if (getClasificacion() == c.InicioBloque)
                {
                    BloqueDeInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
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
                    valor = Asignacion(variable);
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
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": La variable " + variable + " está duplicada");
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": La variable ### está duplicada", variable);
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
                Match(c.Asignacion);
                string valor = Asignacion(constante);

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
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": La constante " + constante + " está duplicada");
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": La constante ### está duplicada", constante);
                }
                finally { closeFiles(); }
            }
        }

        private string Asignacion(string variable)
        {
            string valor = "";
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
                valor = getContenido().Substring(1, getContenido().Length - 2);
                Match(c.Cadena);
            }
            else // Expresión matemática
            {
                log.Write(variable + " = ");
                Expresion();
                valor = evalua.Pop().ToString();
                log.WriteLine();
                log.WriteLine(variable + " = " + valor);
            }
            return valor;
        }

        private void Expresion()
        {
            Termino();
            MasTermino();
        }

        private void Termino()
        {
            Factor();
            PorFactor();
        }

        private void MasTermino()
        {
            if (getClasificacion() == c.OperadorTermino)
            {
                string operador = getContenido();
                Match(c.OperadorTermino);
                Termino();
                log.Write(operador + " ");

                float t1 = evalua.Pop();
                float t2 = evalua.Pop();

                switch (operador)
                {
                    case "+": evalua.Push(t2 + t1); break;
                    case "-": evalua.Push(t2 - t1); break;
                }
            }
        }

        private void PorFactor()
        {
            if (getClasificacion() == c.OperadorFactor)
            {
                string operador = getContenido();
                Match(c.OperadorFactor);
                Factor();
                log.Write(operador + " ");

                float t1 = evalua.Pop();
                float t2 = evalua.Pop();

                switch (operador)
                {
                    case "*": evalua.Push(t2 * t1); break;
                    case "/": evalua.Push(t2 / t1); break;
                    case "%": evalua.Push(t2 % t1); break;
                }

            }
        }

        private void Factor()
        {
            if (getClasificacion() == c.Numero)
            {
                log.Write(getContenido() + " ");
                evalua.Push(float.Parse(getContenido()));
                Match(c.Numero);
            }
            else if (getClasificacion() == c.Funcion)
            {
                if (existeFuncion(getContenido()))
                {
                    string funcion = getContenido();
                    Match(c.Funcion);
                    Match("(");
                    Expresion();
                    Match(")");
                    log.Write(funcion + " ");
                    Funcion f = getFuncion(funcion);
                    evalua.Push(f.func(evalua.Pop()));
                }
                else
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": Función " + getContenido() + " no declarada");
                        throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": Función ### no declarada", getContenido());
                    }
                    finally { closeFiles(); }
                }
                
            }
            else if (getClasificacion() == c.Identificador)
            {
                if (existeVariable(getContenido()))
                {
                    log.Write(getContenido() + " ");
                    evalua.Push(float.Parse(getValor(getContenido())));
                    Match(c.Identificador);
                }
                else
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": Variable " + getContenido() + " no declarada");
                        throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": Variable ### no declarada", getContenido());
                    }
                    finally { closeFiles(); }
                }
            }
            else
            {
                Match("(");
                Expresion();
                Match(")");
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

        private bool existeFuncion(string nombre)
        {
            foreach (Funcion f in funciones)
            {
                if (f.getNombre() == nombre) return true;
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

        private Funcion getFuncion(string nombre)
        {
            foreach (Funcion f in funciones)
            {
                if (f.getNombre() == nombre) return f;
            }
            return null;
        }

        private void setValor(string nombreV, string valorV)
        {
            Variable v = getVariable(nombreV);
            if (v != null) v.setValor(valorV);
        }

        private string getValor(string nombreV)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre() == nombreV) return v.getValor();
            }
            return "";
        }

        private void imprimeStack()
        {
            Console.Write("Valores del stack: ");
            foreach (float f in evalua)
            {
                Console.Write(f + " ");
            }
            Console.WriteLine();
        }

        private void imprimeVariables()
        {
            log.WriteLine();
            log.WriteLine("Lista de Variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine("Nombre: " + v.getNombre());
                log.WriteLine("Tipo: " + v.getTipo());
                log.WriteLine("Valor: " + v.getValor());
                log.WriteLine(v.esVariable() ? "Es variable" : "Es constante");
            }
        }

        private void imprimeFunciones()
        {
            log.WriteLine();
            log.WriteLine("Lista de Funciones: ");
            foreach (Funcion f in funciones)
            {
                log.WriteLine("Nombre: " + f.getNombre());
            }
        }

    }
}
