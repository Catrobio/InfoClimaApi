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

        //#region GET
        [HttpGet()]
        // [Authorize]
        public async Task<IActionResult> GetClimas(string Ciudad, bool historico)
        {
            return Ok(await _climaServices.GetInfoClimas(Ciudad, historico));
        }

        [HttpPost()]
        // [Authorize]
        public async Task<IActionResult> SetClimas(dynamic jsonClimaResult)
        {
            return Ok(await _climaServices.SetInfoClima(jsonClimaResult));
        }
    
    }
}
