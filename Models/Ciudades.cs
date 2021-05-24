using System;
using System.Collections.Generic;

#nullable disable

namespace InfoClimaApi.Models
{
    public partial class Ciudades
    {
        public Ciudades()
        {
            Climas = new HashSet<Clima>();
        }

        public int IdCiudades { get; set; }
        public int IdPaises { get; set; }
        public string Descripcion { get; set; }

        public virtual Paise IdPaisesNavigation { get; set; }
        public virtual ICollection<Clima> Climas { get; set; }
    }
}
