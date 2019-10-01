using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Sintaxis : Lexico
    {
        public Sintaxis() : base()
        {
            //Console.WriteLine("Inicio de analisis sintáctico");
            nextToken();
        }

        public Sintaxis(string filePath) : base(filePath)
        {
            //Console.WriteLine("Inicio de analisis sintáctico");
            nextToken();
        }

        ~Sintaxis()
        {
            //Console.WriteLine("Terminó analisis sintáctico");
        }

        public void Match(string contenido)
        {
            //Console.WriteLine("Match contenido: " + contenido);
            //Console.WriteLine("Match getContenido: " + getContenido());
            if (contenido == getContenido())
            {
                nextToken();
            } else
            {
                try
                {
                    log.WriteLine();
                    log.WriteLine();
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un " + contenido);
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un ###", contenido);
                }
                finally { closeFiles(); }
            }
        }

        public void Match(c clasificacion)
        {
            //Console.WriteLine("Match clasificacion: " + clasificacion);
            //Console.WriteLine("Match contenido: " + getContenido());
            if (clasificacion == getClasificacion())
            {
                nextToken();
            } else
            {
                try
                {
                    log.WriteLine();
                    log.WriteLine();
                    log.WriteLine(DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un " + clasificacion);
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un ###", clasificacion.ToString());
                }
                finally { closeFiles(); }
            }
        }
    }
}
