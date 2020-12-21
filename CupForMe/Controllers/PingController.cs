using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CupForMe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CupForMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Ping Get()
        {
            Ping ping = new Ping
            {
                Date = DateTime.UtcNow,
                Message = "Pinged " + this.Request.GetDisplayUrl()
            };
            return ping;

        }
    }
}
