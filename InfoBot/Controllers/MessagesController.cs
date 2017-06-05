using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        // Get api/Weather
        [HttpGet("/api/messages/weather")]
        public string GetWeather()
        {
            return "Success";
        }
    }
}