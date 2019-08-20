using System;
using System.Collections.Generic;

namespace ProyectoBasesII.Models
{
    public partial class FotoPropiedad
    {
        public decimal NumeroPlanoPropiedad { get; set; }
        public string RutaFotoPropiedad { get; set; }

        public Propiedad NumeroPlanoPropiedadNavigation { get; set; }
    }
}
