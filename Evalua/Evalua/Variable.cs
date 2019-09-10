using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evalua
{
    class Variable
    {
        public enum t { Char, Int, Float, Double, String };
        private string nombre;
        private t tipo;
        private string valor;
        private bool esConst;

        public Variable(string nombre, t tipo, string valor, bool esConst)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.valor = valor;
            this.esConst = esConst;
        }

        public string getValor()
        {
            return this.valor;
        }

        public string getNombre()
        {
            return this.nombre;
        }

        public t getTipo()
        {
            return this.tipo;
        }

        public void setValor(string valor)
        {
            this.valor = valor;
        }

        public static t stringToT(string tipo)
        {
            switch(tipo)
            {
                case "char": return t.Char;
                case "int": return t.Int;
                case "float": return t.Float;
                case "double": return t.Double;
                default: return t.String;
            }
        }

        public bool esVariable()
        {
            return this.esConst ? false : true;
        }
    }
}
