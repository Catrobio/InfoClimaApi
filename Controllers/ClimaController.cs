using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InfoClimaApi.Models.DTOs;
using InfoClimaApi.Services;


namespace ArbitApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClimaController : ControllerBase

    {
        private readonly IClimasServices _climaServices;
        public ClimaController(IClimasServices climaServices)
        {
            _climaServices = climaServices;
        }

        #region GET
        //Devuelve el registro de clima
        [HttpGet()]
        public async Task<IActionResult> GetClimas([FromQuery] string Ciudad, [FromQuery] bool historico)
        {
            return Ok(await _climaServices.GetInfoClimas(Ciudad, historico));
        }
        //Devuelve las ciudades
        [HttpGet("Ciudades")]
        public async Task<IActionResult> GetCiudades()
        {
            return Ok(await _climaServices.GetCiudades());
        }
        #endregion

        #region POST
        //Guarda un clima y ciudad nueva segun lo que devuelva la api de openweather
        [HttpPost()]        
        public async Task<IActionResult> SetClimas([FromBody] dynamic jsonClimaResult)
        {
            return Ok(await _climaServices.SetInfoClima(jsonClimaResult));
        }
        #endregion
    }
}
