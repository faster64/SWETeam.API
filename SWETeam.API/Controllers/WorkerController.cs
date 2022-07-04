using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SWETeam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IServiceProvider _provider;

        public WorkerController(IServiceProvider provider)
        {
            _provider = provider;
        }

        [HttpGet("start-wokrer")]
        public IActionResult RunWokrer()
        {
            return Ok();
        }
    }
}
