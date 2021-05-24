using System;

namespace InfoClimaApi.Models.DTOs
{
    public class InfoClimasDTO
    {
        public int IdClima { get; set; }                    
        public int IdCiudades { get; set; }
        public string Ciudad {get;set;}
        public int IdPais { get; set; }
        public string Pais {get;set;}
        public string Clima { get; set; }
        public string Termica { get; set; }
       
    }
}
