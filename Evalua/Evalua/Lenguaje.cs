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
        Stack<double> evalua;
        Variable.t tipoDataExpresion;

        int contIf = 0;
        int contFor = 0;

        public Lenguaje() : base()
        {
            variables = new List<Variable>();
            funciones = new List<string>();
            evalua = new Stack<double>();
        }
        public Lenguaje(string filePath) : base(filePath)
        {
            variables = new List<Variable>();
            funciones = new List<string>();
            evalua = new Stack<double>();
        }

        public void Programa()
        {
            Librerias();

            asm.WriteLine("\ninclude \"emu8086.inc\"\n");

            Main();

            asm.WriteLine("\nint 21h");
            asm.WriteLine("ret\n");

            imprimeVariables();

            asm.WriteLine("\ndefine_scan_num");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            asm.WriteLine("end");
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
            asm.WriteLine(".code");
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
                                if (typeOutput == "Write")
                                {
                                    if (ejecutar)
                                        Console.Write(getContenido().Substring(1, getContenido().Length - 2));
                                    asm.WriteLine("print " + getContenido());
                                }
                                else
                                {
                                    if (ejecutar)
                                        Console.WriteLine(getContenido().Substring(1, getContenido().Length - 2));
                                    asm.WriteLine("printn " + getContenido());
                                }
                                Match(c.Cadena);
                            }
                            else
                            {
                                if (getContenido() != ")")
                                {
                                    if (existeVariable(getContenido()))
                                    {
                                        Variable v = getVariable(getContenido());
                                        if (typeOutput == "Write")
                                        {
                                            if (v.getTipo() == Variable.t.String)
                                            {
                                                if (ejecutar)
                                                    Console.Write(v.getValor().Substring(1, v.getValor().Length - 2));
                                                asm.WriteLine("print " + v.getValor());
                                            }
                                            else
                                            {
                                                if (ejecutar)
                                                    Console.Write(v.getValor());
                                                asm.WriteLine("mov ax," + v.getNombre());
                                                asm.WriteLine("call print_num");
                                            }
                                        }
                                        else
                                        {
                                            if (v.getTipo() == Variable.t.String)
                                            {
                                                if (ejecutar)
                                                    Console.WriteLine(v.getValor().Substring(1, v.getValor().Length - 2));
                                                asm.WriteLine("printn " + v.getValor());
                                            }
                                            else
                                            {
                                                if (ejecutar)
                                                    Console.WriteLine(v.getValor());
                                                asm.WriteLine("mov ax," + v.getNombre());
                                                asm.WriteLine("call print_num");
                                                asm.WriteLine("printn ''");
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
                            asm.WriteLine("call scan_num");
                            if (typeInput == "ReadLine")
                            {
                                if (ejecutar)
                                    Console.ReadLine();
                                asm.WriteLine("printn ''");
                            }
                            else if (typeInput == "Read")
                            {
                                if (ejecutar)
                                    Console.Read();
                            }
                            else
                            {
                                if (ejecutar)
                                    Console.ReadKey();
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

                    if (v.getTipo() == Variable.t.String && !v.esCadena(valor) && tipoDataExpresion == Variable.t.String)
                    {
                        valor = "\"" + valor + "\"";
                    }

                    validarTipoDato(v, valor);

                    if (v.esConst)
                    {
                        try
                        {
                            log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de semántica en la linea " + this.actRow + " en la columna " +
                                this.actColumn + ": La constante " + v.getNombre() + " no se puede modificar");
                            throw new MyException("Error de semántica en la linea " + this.actRow + " en la columna " +
                                this.actColumn + ": La constante ### no se puede modificar", v.getNombre());
                        }
                        finally { closeFiles(); }
                    }

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

            int contIfTmp = contIf;
            contIf++;

            if (getClasificacion() == c.InicioBloque)
            {
                BloqueDeInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }

            asm.WriteLine("jmp endif" + contIfTmp);
            asm.WriteLine("else" + contIfTmp + ":");
            

            if (getContenido() == "else")
            {
                log.Write("Else: ");
                Match("else");
                if (getClasificacion() == c.InicioBloque)
                {
                    BloqueDeInstrucciones(!ejecutar && ejecutarPre);
                }
                else
                {
                    Instruccion(!ejecutar && ejecutarPre);
                }
            }

            asm.WriteLine("endif" + contIfTmp + ":");

            log.WriteLine();
        }

        private void ForEach(bool ejecutar)
        {
            log.Write("Foreach: ");
            Match(c.ForEach);
            Match("(");
            Expresion();

            int nRep = 0;
            asm.WriteLine("pop ax");
            double nRepTemp = evalua.Pop();
            Variable.t tipoDatoNRep = tipoDatoValor(nRepTemp.ToString());
            if (tipoDatoNRep == Variable.t.Int || tipoDatoNRep == Variable.t.Char) nRep = (int)nRepTemp;
            else
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de semántica en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": El número de repeticiones en el ciclo no es válido");
                    throw new MyException("Error de semántica en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": El número de repeticiones en el ciclo no es válido");
                }
                finally { closeFiles(); }
            }
            long position = archivo.Position;
            int saveActRow = actRow;
            int saveActCol = actColumn;

            Match(")");

            int contForTmp = contFor;
            asm.WriteLine("for" + contForTmp + ":");
            contFor++;

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

            asm.WriteLine(";regresar a for" + contForTmp);

            log.WriteLine();
        }

        private bool Condicion()
        {
            bool not = false;
            if (getContenido() == "!") { Match(getContenido()); not = true; }

            Expresion();

            asm.WriteLine("pop cx");
            double elem1 = evalua.Pop();

            string operador = getContenido();
            Match(c.OperadorRelacional);

            Expresion();

            asm.WriteLine("pop dx");
            double elem2 = evalua.Pop();

            asm.WriteLine("\n;if" + contIf + " " + operador);
            asm.WriteLine("cmp cx,dx");

            bool preRes;
            switch (operador)
            {
                case "==":
                    asm.WriteLine("jne else" + contIf);
                    preRes = elem1 == elem2;
                    break;
                case "!=":
                    asm.WriteLine("je else" + contIf);
                    preRes = elem1 != elem2;
                    break;
                case ">":
                    asm.WriteLine("jle else" + contIf);
                    preRes = elem1 > elem2;
                    break;
                case ">=":
                    asm.WriteLine("jl else" + contIf);
                    preRes = elem1 >= elem2;
                    break;
                case "<":
                    asm.WriteLine("jge else" + contIf);
                    preRes = elem1 < elem2;
                    break;
                default:
                    asm.WriteLine("jg else" + contIf);
                    preRes = elem1 <= elem2;
                    break;
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

        private void ListaDeIdentificadores(Variable.t tipoVariable)
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

                    if (v.getTipo() == Variable.t.String && !v.esCadena(valor) && tipoDataExpresion == Variable.t.String)
                    {
                        valor = "\"" + valor + "\"";
                    }

                    validarTipoDato(v, valor);
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
                Variable v = new Variable(constante, tipoVariable, "", true);
                Match(c.Asignacion);
                string valor = Asignacion(constante);

                if (v.getTipo() == Variable.t.String && !v.esCadena(valor) && tipoDataExpresion == Variable.t.String)
                {
                    valor = "\"" + valor + "\"";
                }

                validarTipoDato(v, valor);

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
                tipoDataExpresion = Variable.t.String;
                Match("Console");
                Match(".");
                asm.WriteLine("call scan_num");
                if (getContenido() == "ReadLine")
                {
                    Match("ReadLine");
                    valor = "\"" + Console.ReadLine() + "\"";
                    asm.WriteLine("printn ''");
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
                asm.WriteLine("mov " + variable + ",cx");
                Match("(");
                Match(")");
            }
            else if (getClasificacion() == c.Cadena)
            {
                tipoDataExpresion = Variable.t.String;
                valor = getContenido();
                Match(c.Cadena);
            }
            else // Expresión matemática
            {
                log.Write(variable + " = ");
                asm.Write("\n;" + variable + " = \n");
                tipoDataExpresion = Variable.t.Char;
                Expresion();
                asm.WriteLine("pop ax");
                asm.WriteLine("mov " + variable + ",ax");
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

                double t1 = evalua.Pop();
                asm.WriteLine("pop bx");
                double t2 = evalua.Pop();
                asm.WriteLine("pop ax");

                switch (operador)
                {
                    case "+":
                        asm.WriteLine("add ax,bx");
                        evalua.Push(t2 + t1);
                        asm.WriteLine("push ax");
                        break;
                    case "-":
                        asm.WriteLine("sub ax,bx");
                        evalua.Push(t2 - t1);
                        asm.WriteLine("push ax");
                        break;
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

                double t1 = evalua.Pop();
                asm.WriteLine("pop bx");
                double t2 = evalua.Pop();
                asm.WriteLine("pop ax");

                switch (operador)
                {
                    case "*":
                        asm.WriteLine("mul bx");
                        evalua.Push(t2 * t1);
                        asm.WriteLine("push ax");
                        break;
                    case "/":
                        asm.WriteLine("div bx");
                        evalua.Push(t2 / t1);
                        asm.WriteLine("push ax");
                        break;
                    case "%":
                        asm.WriteLine("div bx");
                        evalua.Push(t2 % t1);
                        asm.WriteLine("push dx");
                        break;
                }

            }
        }

        private void Factor()
        {
            if (getClasificacion() == c.Numero)
            {
                log.Write(getContenido() + " ");
                tipoDataExpresion = tipoDataExpresion < tipoDatoValor(getContenido()) ? 
                        tipoDatoValor(getContenido()) : tipoDataExpresion;
                evalua.Push(double.Parse(getContenido()));
                asm.WriteLine("mov ax," + getContenido());
                asm.WriteLine("push ax");
                Match(c.Numero);
            }
            else if (getClasificacion() == c.Funcion)
            {
                if (funciones.Contains(getContenido()))
                {
                    string funcion = getContenido();
                    Match(c.Funcion);
                    Match("(");
                    Expresion();
                    Match(")");
                    log.Write(funcion + " ");

                    asm.WriteLine("pop ax");

                    double value = Funcion(funcion, evalua.Pop());

                    Variable.t tipoDatoFunction = tipoDatoValor(value.ToString());
                    tipoDataExpresion = tipoDatoFunction;

                    evalua.Push(value);
                    asm.WriteLine("mov ax," + value);
                    asm.WriteLine("push ax");
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
                    Variable v = getVariable(getContenido());
                    tipoDataExpresion = tipoDataExpresion < v.getTipo() ?
                        v.getTipo() : tipoDataExpresion;

                    Console.WriteLine("tipoDataExpresion: " + tipoDataExpresion.ToString());

                    log.Write(getContenido() + " ");
                    string variableValue = getValor(getContenido());
                    if (v.esCadena(variableValue)) evalua.Push(double.Parse(variableValue.Substring(1, variableValue.Length - 2)));
                    else evalua.Push(double.Parse(variableValue));

                    asm.WriteLine("mov ax," + getContenido());
                    asm.WriteLine("push ax");

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
                bool huboCasteo = false;
                Variable.t casteo = Variable.t.String;
                if (getClasificacion() == c.TipoDato)
                {
                    casteo = Variable.stringToT(getContenido());
                    if (casteo == Variable.t.String) tipoDataExpresion = casteo;
                    huboCasteo = true;
                    Match(c.TipoDato);
                    Match(")");
                    Match("(");
                }
                Expresion();
                Match(")");
                if (huboCasteo)
                {
                    string valor = evalua.Pop().ToString();
                    string valorCasteado = evaluaCasteo(valor, casteo);
                    evalua.Push(double.Parse(valorCasteado));
                    tipoDataExpresion = casteo;
                }
            }
        }

        private double Funcion(string nombre, double n)
        {
            switch (nombre)
            {
                case "abs": return Math.Abs(n);
                case "sqrt": return (double) Math.Sqrt(n);
                case "pow2": return n * n;
                case "round": return (double) Math.Round(n);
                case "trunc": return (double) Math.Truncate(n);
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

            else if (tipoDataExpresion > v.getTipo())
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de semántica en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": No se puede asignar un " + tipoDataExpresion + " a la variable " + v.getNombre() + " que es de tipo " + v.getTipo());
                    throw new MyException("Error de semántica en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": No se puede asignar un " + tipoDataExpresion + " a la variable ### que es de tipo " + v.getTipo(), v.getNombre());
                }
                finally { closeFiles(); }
            }
        }

        private Variable.t tipoDatoValor(string valor)
        {
            if (valor[0] == '\"') return Variable.t.String;
            else if (valor.Contains("."))
            {
                try
                {
                    float.Parse(valor);
                    return Variable.t.Float;
                }
                catch { return Variable.t.Double; }
                
            }
            else if (double.Parse(valor) < 256) return Variable.t.Char;
            else if (double.Parse(valor) < 65536) return Variable.t.Int;
            else return Variable.t.String;
        }

        private string evaluaCasteo(string valor, Variable.t tipoD)
        {
            int val;
            switch (tipoD)
            {
                case Variable.t.Char:
                    val = (int) Math.Truncate(double.Parse(valor));
                    if (val < 256) return val.ToString();
                    return (val % 256).ToString();
                case Variable.t.Int:
                    val = (int) Math.Truncate(double.Parse(valor));
                    if (val < 65536) return val.ToString();
                    return (val % 65536).ToString();
                case Variable.t.Float:
                    double val2 = double.Parse(valor);
                    if (val2 > 3.4E38)
                    {
                        return (val2 % 3.4E38).ToString();
                    }
                    return val2.ToString();
                case Variable.t.Double:
                default:
                    return valor;
            }
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

                asm.WriteLine(v.getNombre() + " dw ?");
            }
        }
    }
}
