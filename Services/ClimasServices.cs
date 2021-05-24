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
using InfoClimaApi.Mappers;
using AutoMapper;

namespace InfoClimaApi.Services
{
    public interface IClimasServices
    {

        Task<IEnumerable<InfoClimasDTO>> GetInfoClimas(string Ciudad, bool historico);
        Task<IEnumerable<CiudadesDTO>> GetCiudades();
        Task<Clima> SetInfoClima(dynamic json);
    }

    public class ClimasServices : IClimasServices
    {
        private readonly InfoClimaContext _context;
        private readonly HttpClient _client;
        //Para optener las variables de entorno con la url de la api IinfoClima se inicializa y luego se inyecta la interfaz de configuracion 
        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        public ClimasServices(InfoClimaContext context, HttpClient client, IConfiguration configuration, IMapper mapper)
        {
            _client = client;
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InfoClimasDTO>> GetInfoClimas(string Ciudad, bool historico)
        {

            var infoClimaResult = new InfoClimasDTO();
            var ListinfoClimaResult = new List<InfoClimasDTO>();
            //URI de consulta de clima por la ciudad
            var uri = _configuration["openweatherApi"] + "q=" + Ciudad + "&appid=" + _configuration["openweatherKey"];
            //Consulto el clima
            var response = await _client.GetAsync(uri);
            // Si la respuesta es exitosa 
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync();
                //convierto en json el resultado
                dynamic jsonResult = JsonConvert.DeserializeObject(result.Result);
                //Guardo en base de datos el clima
                Clima nuevoClima = await SetInfoClima(jsonResult);
                //uso Automapper para convertir la clase a DTO
                infoClimaResult = _mapper.Map<InfoClimasDTO>(nuevoClima);
                //añado a la lista result
                ListinfoClimaResult.Add(infoClimaResult);
            }
            if (historico)
            {
                //Si solicito historico consulto todos los climas guardados con esta ciudad segun su id
                var Climas = await _context.Climas
                    .Include(t => t.IdCiudadesNavigation)
                        .ThenInclude(p => p.IdPaisesNavigation)
                    .Where(c => c.IdCiudades == infoClimaResult.IdCiudades)
                    .ToListAsync();
                ListinfoClimaResult = _mapper.Map<List<InfoClimasDTO>>(Climas);
            }
            return ListinfoClimaResult;
        }

        public async Task<IEnumerable<CiudadesDTO>> GetCiudades()
        {
            var ciudades = await _context.Ciudades.Include(p => p.IdPaisesNavigation).ToListAsync();
            var ListCiudadesResult = _mapper.Map<IEnumerable<CiudadesDTO>>(ciudades);
            return ListCiudadesResult;
        }
        public async Task<Clima> SetInfoClima(dynamic json)
        {
            var T = json["main"]["feels_like"].ToString();
            var c = json["main"]["temp"].ToString();
            string ciudad = json["name"].ToString();
            string Pais = json["sys"]["country"].ToString();
            var clima = new Clima();
            //convierto la temperatura de Kelvin a Celsius
            clima.Clima1 = setCelsiusTemp(T);
            clima.Termica = setCelsiusTemp(c);
            clima.Fecha = DateTime.Now;
            //Verifico si la ciudad existe, incluyo el pais.
            var ciudadConsulta = await _context.Ciudades
                .Include(p => p.IdPaisesNavigation)
                .Where(c => c.Descripcion.ToUpper() == ciudad.ToUpper())
                .FirstOrDefaultAsync();
            //Si no existe la ciudad
            if (ciudadConsulta == null)
            {
                var nuevaCiudad = new Ciudades();
                nuevaCiudad.Descripcion = ciudad;
                //Valido que exista el pais 
                var pais = await _context.Paises
                    .Where(p => p.Descripcion.ToUpper() == Pais.ToUpper())
                    .FirstOrDefaultAsync();
                //si no existe el pais
                if (pais == null)
                {
                    //Guardo el nuevo pais segun lo que devuelve la api de openweather
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
                    //Uso el pais que devuelve la base de datos
                    nuevaCiudad.IdPaises = pais.IdPaises;
                }
                //Guardo la ciudad
                var ciudadGuardada = await _context.AddAsync(nuevaCiudad);
                await _context.SaveChangesAsync();
                //uso el id de ciudad guardado para el nuevo clima
                clima.IdCiudades = ciudadGuardada.Entity.IdCiudades;
            }
            else
            {
                //uso el id de ciudad guardado para el nuevo clima
                clima.IdCiudades = ciudadConsulta.IdCiudades;
            }
            //Guardo los datos del clima
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