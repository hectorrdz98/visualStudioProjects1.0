using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
{
    class Sintaxis : Lexico
    {
        public Sintaxis() : base()
        {
            nextToken();
        }

        public Sintaxis(string filePath) : base(filePath)
        {
            nextToken();
        }

        public void Match(string contenido)
        {
            if (contenido == getContenido())
            {
                nextToken();
            }
            else
            {
                try
                {
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un ###", contenido);
                }
                finally { closeFiles(); }
            }
        }

        public void Match(c clasificacion)
        {
            if (clasificacion == getClasificacion())
            {
                nextToken();
            }
            else
            {
                try
                {
                    throw new MyException("Error de sintaxis en la linea " + this.actRow + " en la columna " +
                        this.actColumn + ": Se espera un ###", clasificacion.ToString());
                }
                finally { closeFiles(); }
            }
        }
    }
}
