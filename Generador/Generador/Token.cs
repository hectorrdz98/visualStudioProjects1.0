using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generador
{
    class Token
    {
        public enum c
        {
            ST, SNT, Flechita, Epsilon,
            Or, FinProduccion, ParentesisDer, ParentesisIzq
        };
        private c clasificacion;
        private string contenido;

        public void setClasificacion(c newClasificacion)
        {
            this.clasificacion = newClasificacion;
        }
        public c getClasificacion()
        {
            return this.clasificacion;
        }


        public void setContenido(string newContenido)
        {
            this.contenido = newContenido;
        }
        public string getContenido()
        {
            return this.contenido;
        }
    }
    
}
