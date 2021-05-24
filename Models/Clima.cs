using System;
using System.Collections.Generic;

#nullable disable

namespace InfoClimaApi.Models
{
    public partial class Clima
    {
        public int IdClima { get; set; }
        public int IdCiudades { get; set; }
        public string Clima1 { get; set; }
        public string Termica { get; set; }
        public DateTime? Fecha { get; set; }

        public virtual Ciudades IdCiudadesNavigation { get; set; }
    }
}
