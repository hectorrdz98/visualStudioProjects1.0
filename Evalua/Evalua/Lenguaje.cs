using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Lenguaje : Sintaxis
    {
        List<Variable> variables;
        List<string> funciones;
        Stack<float> evalua;

        public Lenguaje() : base()
        {
            variables = new List<Variable>();
            funciones = new List<string>();
            evalua = new Stack<float>();
        }
        public Lenguaje(string filePath) : base(filePath)
        {
            variables = new List<Variable>();
            funciones = new List<string>();
            evalua = new Stack<float>();
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
            string subLibreria = getContenido();
            Match(c.Identificador);
            if (getContenido() == ".")
            {
                subLibreria += SubLibrerias();
                if (subLibreria == "System.Math")
                {
                    funciones.Add("abs");
                    funciones.Add("sqrt");
                    funciones.Add("pow2");
                    funciones.Add("round");
                    funciones.Add("trunc");

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

        private void BloqueDeInstrucciones(bool ejecutar = true, int rep = 1)
        {
            long position = archivo.Position;
            int saveActRow = actRow;
            int saveActCol = actColumn;
            Match(c.InicioBloque);
            {
                if (getClasificacion() != c.FinBloque)
                {
                    if (rep == 0) ejecutar = false;

                    for (int i = 0; i < rep; i++)
                    {
                        Instrucciones(ejecutar);
                        if (i < rep - 1)
                        {
                            archivo.Seek(position, SeekOrigin.Begin);
                            actRow = saveActRow;
                            actColumn = saveActCol;
                            nextToken();
                        }
                    }
                }
            }
            Match(c.FinBloque);
        }

        private void Instruccion(bool ejecutar)
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
                                if (ejecutar)
                                {
                                    string output = getContenido();
                                    if (typeOutput == "Write") Console.Write(output.Substring(1, output.Length - 2));
                                    else Console.WriteLine(output.Substring(1, output.Length - 2));
                                }
                                Match(c.Cadena);
                            }
                            else
                            {
                                if (getContenido() != ")")
                                {
                                    if (existeVariable(getContenido()))
                                    {
                                        if (ejecutar)
                                        {
                                            Variable v = getVariable(getContenido());
                                            if (typeOutput == "Write")
                                            {
                                                if (v.getTipo() == Variable.t.String)
                                                {
                                                    Console.Write(v.getValor().Substring(1, v.getValor().Length - 2));
                                                }
                                                else Console.Write(v.getValor());
                                            }
                                            else
                                            {
                                                if (v.getTipo() == Variable.t.String)
                                                {
                                                    Console.WriteLine(v.getValor().Substring(1, v.getValor().Length - 2));
                                                }
                                                else Console.WriteLine(v.getValor());
                                            }
                                        }
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
                            if (ejecutar)
                            {
                                if (typeInput == "ReadLine") Console.ReadLine();
                                else if (typeInput == "Read") Console.Read();
                                else Console.ReadKey();
                            }
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
                ListaDeIdentificadores(tipoVariable, ejecutar);
                Match(c.FinSentencia);
            }
            else if (getClasificacion() == c.Constante)
            {
                Match(c.Constante);
                Variable.t tipoVariable = Variable.stringToT(getContenido());
                Match(c.TipoDato);
                ListaDeConstantes(tipoVariable, ejecutar);
                Match(c.FinSentencia);
            }
            else if (getClasificacion() == c.If)
            {
                IF(ejecutar);
            }
            else if (getClasificacion() == c.ForEach)
            {
                ForEach(ejecutar);
            }
            else // Modificacion de variables
            {
                string variable = getContenido();
                Match(c.Identificador);
                if (existeVariable(variable))
                {
                    Variable v = getVariable(variable);
                    Match(c.Asignacion);
                    string valor = Asignacion(variable);

                    validarTipoDato(v, valor);

                    if (ejecutar) setValor(variable, valor);
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

        private void Instrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (getClasificacion() != c.FinBloque)
                Instrucciones(ejecutar);
        }

        private void IF(bool ejecutarPre)
        {
            log.Write("IF: ");
            Match(c.If);
            Match("(");

            bool ejecutar = Condicion() && ejecutarPre;
            Match(")");

            if (getClasificacion() == c.InicioBloque)
            {
                BloqueDeInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }

            if (getContenido() == "else")
            {
                log.Write("Else: ");
                Match("else");
                if (getClasificacion() == c.InicioBloque)
                {
                    BloqueDeInstrucciones(!ejecutar);
                }
                else
                {
                    Instruccion(!ejecutar);
                }
            }

            log.WriteLine();
        }

        private void ForEach(bool ejecutar)
        {
            log.Write("IF: ");
            Match(c.ForEach);
            Match("(");
            Expresion();

            int nRep = (int) Math.Truncate(evalua.Pop());
            long position = archivo.Position;
            int saveActRow = actRow;
            int saveActCol = actColumn;

            Match(")");

            if (getClasificacion() == c.InicioBloque)
            {
                BloqueDeInstrucciones(ejecutar, nRep);
            }
            else
            {
                if (nRep == 0)
                    Instruccion(false);

                for (int i = 0; i < nRep; i++)
                {
                    Instruccion(ejecutar);
                    if (i < nRep - 1)
                    {
                        archivo.Seek(position, SeekOrigin.Begin);
                        actRow = saveActRow;
                        actColumn = saveActCol;
                        nextToken();
                    }
                }
            }

            log.WriteLine();
        }

        private bool Condicion()
        {
            bool not = false;
            if (getContenido() == "!") { Match(getContenido()); not = true; }

            Expresion();
            float elem1 = evalua.Pop();
            string operador = getContenido();
            Match(c.OperadorRelacional);
            Expresion();
            float elem2 = evalua.Pop();

            bool preRes;
            switch (operador)
            {
                case "==": preRes = elem1 == elem2; break;
                case "!=": preRes = elem1 != elem2; break;
                case ">": preRes = elem1 > elem2; break;
                case ">=": preRes = elem1 >= elem2; break;
                case "<": preRes = elem1 < elem2; break;
                default: preRes = elem1 <= elem2; break;
            }

            if (not) preRes = !preRes;

            if (getClasificacion() == c.OperadorLogico)
            {
                switch (getContenido())
                {
                    case "||":
                        {
                            Match(c.OperadorLogico);
                            preRes = Condicion() || preRes;
                        } break;
                    default:
                        {
                            Match(c.OperadorLogico);
                            preRes = Condicion() && preRes;
                        }
                        break;
                }
            }

            return preRes;
        }

        private void ListaDeIdentificadores(Variable.t tipoVariable, bool ejecutar)
        {
            string variable = getContenido();
            Match(c.Identificador);
            if (!existeVariable(variable))
            {
                string valor = "";
                if (getClasificacion() == c.Asignacion)
                {
                    Variable v = new Variable(variable, tipoVariable, "", false);
                    Match(c.Asignacion);
                    valor = Asignacion(variable);

                    validarTipoDato(v, valor);
                }

                if (ejecutar) variables.Add(new Variable(variable, tipoVariable, valor, false));

                if (getContenido() == ",")
                {
                    Match(",");
                    ListaDeIdentificadores(tipoVariable, ejecutar);
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

        private void ListaDeConstantes(Variable.t tipoVariable, bool ejecutar)
        {
            string constante = getContenido();
            Match(c.Identificador);
            if (!existeVariable(constante))
            {
                Variable v = new Variable(constante, tipoVariable, "", true);
                Match(c.Asignacion);
                string valor = Asignacion(constante);

                validarTipoDato(v, valor);

                if (ejecutar) variables.Add(new Variable(constante, tipoVariable, valor, true));

                if (getContenido() == ",")
                {
                    Match(",");
                    ListaDeConstantes(tipoVariable, ejecutar);
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
                    valor = "\"" + Console.ReadLine() + "\"";
                }
                else if (getContenido() == "Read")
                {
                    Match("Read");
                    valor = "\"" + Convert.ToChar(Console.Read()).ToString() + "\"";
                }
                else
                {
                    Match("ReadKey");
                    valor = "\"" + Console.ReadKey().KeyChar.ToString() + "\"";
                }
                Match("(");
                Match(")");
            }
            else if (getClasificacion() == c.Cadena)
            {
                valor = getContenido();
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
                //if (existeFuncion(getContenido()))
                if (funciones.Contains(getContenido()))
                {
                    string funcion = getContenido();
                    Match(c.Funcion);
                    Match("(");
                    Expresion();
                    Match(")");
                    log.Write(funcion + " ");
                    //Funcion f = getFuncion(funcion);
                    //evalua.Push(f.func(evalua.Pop()));
                    evalua.Push(Funcion(funcion, evalua.Pop()));
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

        private float Funcion(string nombre, float n)
        {
            switch (nombre)
            {
                case "abs": return Math.Abs(n);
                case "sqrt": return (float) Math.Sqrt(n);
                case "pow2": return n * n;
                case "round": return (float) Math.Round(n);
                case "trunc": return (float) Math.Truncate(n);
            }
            return n;
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

        private void validarTipoDato(Variable v, string valor)
        {
            if (v.getTipo() == Variable.t.String)
            {
                if (!v.esCadena(valor))
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de semántica en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": La valor asignado a la variable " + v.getNombre() + " no es una cadena");
                        throw new MyException("Error de semántica en la linea " + this.actRow + " en la columna " +
                            this.actColumn + ": La valor asignado a la variable ### no es una cadena", v.getNombre());
                    }
                    finally { closeFiles(); }
                }
            }

            Variable.t tDatoValor = tipoDatoValor(valor);

            if (v.getTipo() < Variable.t.String)
            {

            }
        }

        private Variable.t tipoDatoValor(string valor)
        {
            if (valor.Contains(".")) return Variable.t.Float;
            return Variable.t.String;
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
    }
}
