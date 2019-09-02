using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Evalua
{
    class Lexico : Token, IDisposable
    {
        public StreamReader archivo;
        private StreamWriter log;

        private const int f = -1;
        private const int e = -2;

        private int[,] Trnd = {
            {  0,  0,  1,  2, 32,  1,  8,  9, 10, 11, 12, 14, 16, 18, 19, 21, 23, 23, 25, 27,  f, 32 }, 
            {  f,  f,  1,  1,  f,  1,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  2,  3,  5,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  e,  e,  e,  4,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e }, 
            {  f,  f,  f,  4,  f,  5,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  e,  e,  e,  7,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  6,  6,  e,  e,  e,  e,  e,  e }, 
            {  e,  e,  e,  7,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e,  e }, 
            {  f,  f,  f,  7,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f, 13,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f, 15,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 17,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 20,  f,  f,  f,  f,  f,  f, 20,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 22,  f,  f,  f,  f,  f,  f,  f, 22,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 24,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            { 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 25,  e, 25 }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f }, 
            {  f,  f,  f,  f,  f,  f,  f, 24,  f,  f,  f,  f,  f,  f,  f,  f, 29,  f,  f, 28,  f,  f }, 
            { 28,  0, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,  f, 28 }, 
            { 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 30, 29, 29, 29,  f, 29 }, 
            { 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 30, 29, 29, 31,  f, 29 }, 
            { 0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  f,  0 }, 
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f },
        };

        public Lexico()
        {
            archivo = new StreamReader("C:\\archivos\\prueba.cs");
            log = new StreamWriter("C:\\archivos\\prueba.log", true);
        }

        public Lexico(string filePath)
        {
            log = new StreamWriter(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".log", true);
            if (Path.GetExtension(filePath) != ".cs")
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - The file extension isn't .cs");
                    throw new LexicoExcepcion("The file extension isn't .cs");
                }
                finally { closeFiles(); }
            }
                
            if (!File.Exists(filePath))
            {
                try
                {
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - The file " + Path.GetFileName(filePath) + " doesn't exist");
                    throw new LexicoExcepcion("The file " + Path.GetFileName(filePath) + " doesn't exist");
                }
                finally { closeFiles(); }
            }
            archivo = new StreamReader(filePath);
        }

        public void Dispose()
        {
            closeFiles();
        }

        private void closeFiles()
        {
            if (archivo != null) archivo.Close();
            log.Close();
        }

        private int getColumna(char transicion)
        {
            if (transicion == 10)
            {
                return 1;
            }
            else if (char.IsWhiteSpace(transicion))
            {
                return 0;
            }
            else if (char.ToUpper(transicion) == 'E')
            {
                return 5;
            }
            else if (char.IsLetter(transicion))
            {
                return 2;
            }
            else if (char.IsDigit(transicion))
            {
                return 3;
            }
            else if (transicion == '.')
            {
                return 4;
            }
            else if (transicion == ';')
            {
                return 6;
            }
            else if (transicion == '=')
            {
                return 7;
            }
            else if (transicion == '{')
            {
                return 8;
            }
            else if (transicion == '}')
            {
                return 9;
            }
            else if (transicion == '&')
            {
                return 10;
            }
            else if (transicion == '|')
            {
                return 11;
            }
            else if (transicion == '!')
            {
                return 12;
            }
            else if (transicion == '>' || transicion == '<')
            {
                return 13;
            }
            else if (transicion == '+')
            {
                return 14;
            }
            else if (transicion == '-')
            {
                return 15;
            }
            else if (transicion == '*')
            {
                return 16;
            }
            else if (transicion == '%')
            {
                return 17;
            }
            else if (transicion == '"')
            {
                return 18;
            }
            else if (transicion == '/')
            {
                return 19;
            }
            else if (archivo.EndOfStream)
            {
                return 20;
            }

            return 21;
        }

        public void nextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;

            while(estado >= 0)
            {
                transicion = (char) archivo.Peek();
                int preEstado = estado;
                estado = Trnd[estado, this.getColumna(transicion)];
                if (estado >= 0)
                {
                    archivo.Read();
                    if (estado == 0) buffer = "";
                    if (estado > 0)
                    {
                        buffer += transicion;
                        switch (estado)
                        {
                            case 1: setClasificacion(c.Identificador); break;
                            case 2: setClasificacion(c.Numero); break;
                            case 4: setClasificacion(c.Numero); break;
                            case 7: setClasificacion(c.Numero); break;
                            case 8: setClasificacion(c.FinSentencia); break;
                            case 9: setClasificacion(c.Asignacion); break;
                            case 10: setClasificacion(c.InicioBloque); break;
                            case 11: setClasificacion(c.FinBloque); break;
                            case 12: setClasificacion(c.Caracter); break;
                            case 13: setClasificacion(c.OperadorLogico); break;
                            case 14: setClasificacion(c.Caracter); break;
                            case 15: setClasificacion(c.OperadorLogico); break;
                            case 16: setClasificacion(c.OperadorLogico); break;
                            case 17: setClasificacion(c.OperadorRelacional); break;
                            case 18: setClasificacion(c.OperadorRelacional); break;
                            case 19: setClasificacion(c.OperadorTermino); break;
                            case 20: setClasificacion(c.IncrementoTermino); break;
                            case 21: setClasificacion(c.OperadorTermino); break;
                            case 22: setClasificacion(c.IncrementoTermino); break;
                            case 23: setClasificacion(c.OperadorFactor); break;
                            case 24: setClasificacion(c.IncrementoFactor); break;
                            case 26: setClasificacion(c.Cadena); break;
                            case 27: setClasificacion(c.OperadorFactor); break;
                            default: setClasificacion(c.Caracter); break;
                        }
                    }
                } else if (estado == e)
                {
                    try
                    {
                        log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Lexic error after " + buffer);
                        throw new LexicoExcepcion("Lexic error after", buffer);
                    }
                    finally { closeFiles(); }
                }
            }

            this.setContenido(buffer);
        }
    }
}
