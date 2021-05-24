using System;
using System.Collections.Generic;

#nullable disable

namespace InfoClimaApi.Models
{
    public partial class Paise
    {
        public Paise()
        {
            Ciudades = new HashSet<Ciudades>();
        }

        public int IdPaises { get; set; }
        public string Descripcion { get; set; }

        public virtual ICollection<Ciudades> Ciudades { get; set; }
    }
}
