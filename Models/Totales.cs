using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoBasesII.Models
{
    public partial class Totales
    {
        public Totales()
        {
        }

        public decimal codigoPais { get; set; }
        public decimal annoRegistro { get; set; }
        public decimal cantidad { get; set; }

    }
}