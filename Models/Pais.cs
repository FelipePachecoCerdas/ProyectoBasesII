using System;
using System.Collections.Generic;

namespace ProyectoBasesII.Models
{
    public partial class Pais
    {
        public Pais()
        {
            Propiedad = new HashSet<Propiedad>();
        }

        public decimal IdPais { get; set; }
        public string NbrPais { get; set; }
        public decimal AreaKm { get; set; }
        public decimal PoblacionActual { get; set; }
        public string NivelRiesgo { get; set; }

        public ICollection<Propiedad> Propiedad { get; set; }
    }
}
