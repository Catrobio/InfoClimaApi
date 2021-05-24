using System;
using System.Threading.Tasks;
using InfoClimaApi.Models;
using InfoClimaApi.Models.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace InfoClimaApi.Services
{
    public interface IClimasServices
    {

        Task<IEnumerable<InfoClimasDTO>> GetInfoClimas(string Ciudad, bool historico);
        Task<Clima> SetInfoClima(dynamic json);
    }

    public class ClimasServices : IClimasServices
    {
        private readonly InfoClimaContext _context;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        public ClimasServices(InfoClimaContext context, HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<InfoClimasDTO>> GetInfoClimas(string Ciudad, bool historico)
        {
            var infoClimaResult = new InfoClimasDTO();
            var ListinfoClimaResult = new List<InfoClimasDTO>();
            var uri = _configuration["openweatherApi"] + "q=" + Ciudad + "&appid=" + _configuration["openweatherKey"];
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync();
                dynamic jsonResult = JsonConvert.DeserializeObject(result.Result);
                Clima nuevoClima  = await SetInfoClima(jsonResult);
                infoClimaResult.IdClima = nuevoClima.IdClima;
                infoClimaResult.IdCiudades = nuevoClima.IdCiudades;
                infoClimaResult.Ciudad = nuevoClima.IdCiudadesNavigation.Descripcion;
                infoClimaResult.Clima = nuevoClima.Clima1;
                infoClimaResult.IdPais = nuevoClima.IdCiudadesNavigation.IdPaises;
                infoClimaResult.Pais =  nuevoClima.IdCiudadesNavigation.Descripcion;
                infoClimaResult.Termica = nuevoClima.Termica;                
                ListinfoClimaResult.Add(infoClimaResult);
            }
            if (historico)
            {
                var Climas = await _context.Climas
                    .Include(t => t.IdCiudadesNavigation)
                        .ThenInclude(p => p.IdPaisesNavigation)
                    .Where(c => c.IdCiudades == infoClimaResult.IdCiudades)
                    .ToListAsync();
                foreach (var _clima in Climas)
                {
                    var toInfoClima = new InfoClimasDTO
                    {
                        IdClima = _clima.IdClima,
                        IdCiudades = _clima.IdCiudades,
                        Ciudad = _clima.IdCiudadesNavigation.Descripcion,
                        Clima = _clima.Clima1,
                        IdPais = _clima.IdCiudadesNavigation.IdPaises,
                        Pais = _clima.IdCiudadesNavigation.IdPaisesNavigation.Descripcion,
                        Termica = _clima.Termica
                    };
                    ListinfoClimaResult.Add(toInfoClima);
                }

            }
            return ListinfoClimaResult;
        }

        public async Task<Clima> SetInfoClima(dynamic json)
        {
            var T = json["main"]["feels_like"].ToString();
            var c = json["main"]["temp"].ToString();
            string ciudad = json["name"].ToString();
            string Pais = json["sys"]["country"].ToString();
            var clima = new Clima();
            clima.Clima1 = setCelsiusTemp(T);
            clima.Termica = setCelsiusTemp(c);
            clima.Fecha = DateTime.Now;
            var ciudadConsulta = await _context.Ciudades
                  .Where(c => c.Descripcion.ToUpper() == ciudad.ToUpper())
                  .FirstOrDefaultAsync();
            if (ciudadConsulta == null)
            {
                var nuevaCiudad = new Ciudades();
                nuevaCiudad.Descripcion = ciudad;
                
                var pais = await _context.Paises
                    .Where(p => p.Descripcion.ToUpper() == Pais.ToUpper())
                    .FirstOrDefaultAsync();

                if (pais == null)
                {
                    var NuevoPais = new Paise
                    {
                        Descripcion = Pais
                    };
                    var paisGuardado = await _context.AddAsync(NuevoPais);
                    await _context.SaveChangesAsync();       
                    nuevaCiudad.IdPaises = paisGuardado.Entity.IdPaises;
                }
                else
                {
                    nuevaCiudad.IdPaises = pais.IdPaises;
                }
                var ciudadGuardada = await _context.AddAsync(nuevaCiudad);
                await _context.SaveChangesAsync();
                
                clima.IdCiudades = ciudadGuardada.Entity.IdCiudades;
            }
            else
            {
                clima.IdCiudades = ciudadConsulta.IdCiudades;
            }
            var climaGuardado = await _context.AddAsync(clima);
            await _context.SaveChangesAsync();
            return climaGuardado.Entity;

        }

        private string setCelsiusTemp(string Temp)
        {
            var KelvinTemp = Convert.ToDecimal(Temp);
            var CelsiusTemp = KelvinTemp - 273.15M;
            var CelsiusString = CelsiusTemp.ToString().Replace(".", ",");
            return CelsiusString;
        }
    }
}