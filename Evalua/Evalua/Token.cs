using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Token
    {
        public enum c { Identificador, Numero, Asignacion, OperadorLogico, OperadorRelacional,
            OperadorTermino, OperadorFactor, IncrementoTermino, IncrementoFactor, Cadena,
            FinSentencia, InicioBloque, FinBloque, Caracter, TipoDato,
            Constante, Funcion, If };
        private c clasificacion;
        private string contenido;

        public void setClasificacion(c newClasificacion)
        {
            this.clasificacion = newClasificacion;
        }
        public c getClasificacion ()
        {
            return this.clasificacion;
        }
        

        public void setContenido(string newContenido)
        {
            this.contenido = newContenido;
        }
        public string getContenido ()
        {
            return this.contenido;
        }
    }
}
