using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// https://pastebin.com/EZAUyMZT

namespace Generador
{
    class Lexico : Token
    {
        public FileStream archivo;
        protected StreamWriter lenguaje;

        private const int f = -1;
        private const int e = -2;
        protected int actRow = 1;
        protected int actColumn = 1;

        // https://pastebin.com/fCVEbKcx
        private int[,] Trnd = {
            // WS  L   \   ;   -   >   (   )   ?   |   ᵝ
            {  0,  1,  2,  4,  5, 11,  7,  8,  9, 10, 11  },
            {  f,  1,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  6,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  },
            {  f,  f,  f,  f,  f,  f,  f,  f,  f,  f,  f  }
        };

        public Lexico()
        {
            string filePath = "C:\\archivos\\prueba.gram";
            if (!File.Exists(filePath))
            {
                try
                {
                    throw new MyException("Error de léxico: El archivo " + Path.GetFileName(filePath) + " no existe");
                }
                finally { closeFiles(); }
            }
            archivo = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            lenguaje = new StreamWriter("C:\\archivos\\Lenguaje.cs");
        }

        public Lexico(string filePath)
        {
            if (Path.GetExtension(filePath) != ".gram")
            {
                try
                {
                    throw new MyException("La extensión del archivo no es .gram");
                }
                finally { closeFiles(); }
            }

            if (!File.Exists(filePath))
            {
                try
                {
                    throw new MyException("El archivo " + Path.GetFileName(filePath) + " no existe");
                }
                finally { closeFiles(); }
            }
            archivo = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            lenguaje = new StreamWriter("C:\\archivos\\Lenguaje.cs");
        }

        public void closeFiles()
        {
            if (archivo != null) archivo.Close();
            if (lenguaje != null) lenguaje.Close();
        }

        private int getColumna(char transicion)
        {
            /*if (transicion == 10)    Para el req #1
            {
                return 1;
            }*/
            if (char.IsWhiteSpace(transicion))
            {
                return 0;
            }
            else if (char.IsLetter(transicion))
            {
                return 1;
            }
            else if (transicion == '\\')
            {
                return 2;
            }
            else if (transicion == ';')
            {
                return 3;
            }
            else if (transicion == '-')
            {
                return 4;
            }
            else if (transicion == '>')
            {
                return 5;
            }
            else if (transicion == '(')
            {
                return 6;
            }
            else if (transicion == ')')
            {
                return 7;
            }
            else if (transicion == '?')
            {
                return 8;
            }
            else if (transicion == '|')
            {
                return 9;
            }

            return 10;
        }

        public void nextToken()
        {
            char transicion;
            string buffer = "";
            int estado = 0;

            while (estado >= 0)
            {
                long position = archivo.Position;
                transicion = (char)archivo.ReadByte();
                archivo.Seek(position, SeekOrigin.Begin);

                if (transicion == '\n')
                {
                    this.actRow++;
                    this.actColumn = 1;
                }

                estado = Trnd[estado, this.getColumna(transicion)];
                if (estado >= 0)
                {
                    archivo.ReadByte();
                    this.actColumn++;
                    if (estado == 0) buffer = "";
                    if (estado > 0)
                    {
                        buffer += transicion;
                        switch (estado)
                        {
                            case 1: setClasificacion(c.SNT); break;
                            case 2: setClasificacion(c.ST); break;
                            case 4: setClasificacion(c.FinProduccion); break;
                            case 5: setClasificacion(c.ST); break;
                            case 6: setClasificacion(c.Flechita); break;
                            case 7: setClasificacion(c.ParentesisIzq); break;
                            case 8: setClasificacion(c.ParentesisDer); break;
                            case 9: setClasificacion(c.Epsilon); break;
                            case 10: setClasificacion(c.Or); break;
                            default: setClasificacion(c.ST); break;
                        }
                    }
                }
            }

            this.setContenido(buffer);

            if (getClasificacion() == c.SNT)
            {
                if (char.IsLower(getContenido()[0]))
                    setClasificacion(c.ST);
            }
        }
    }
}
