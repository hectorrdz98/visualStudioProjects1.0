using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Funcion
    {
        private string nombre;
        public Func<float, float> func;

        public Funcion(string nombre)
        {
            this.nombre = nombre;
        }

        public string getNombre()
        {
            return this.nombre;
        }
    }
}
