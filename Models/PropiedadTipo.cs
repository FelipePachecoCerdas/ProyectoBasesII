using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoBasesII.Models
{
    public partial class PropiedadTipo
    {
        public PropiedadTipo()
        {
        }

        public decimal IdPais { get; set; }
        public string NbrPais { get; set; }
        public string TipoPropiedad { get; set; }
        public decimal Cantidad { get; set; }

    }
}
