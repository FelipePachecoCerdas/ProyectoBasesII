using System;
using System.Collections.Generic;

namespace ProyectoBasesII.Models
{
    public partial class Propiedad
    {
        public Propiedad()
        {
            FotoPropiedad = new HashSet<FotoPropiedad>();
        }

        public decimal NumeroPlano { get; set; }
        public decimal AnnoRegistro { get; set; }
        public string RutaImagenPlano { get; set; }
        public string RutaValoracion { get; set; }
        public string TipoPropiedad { get; set; }
        public decimal CodigoPais { get; set; }
        public string NbrContanto { get; set; }
        public string CorreoElectronico { get; set; }
        public string RutaVideo { get; set; }
        public decimal CostoDolares { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Pais CodigoPaisNavigation { get; set; }
        public ICollection<FotoPropiedad> FotoPropiedad { get; set; }
    }
}
