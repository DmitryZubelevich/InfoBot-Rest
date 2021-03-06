using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InfoBot.MessageGenerators.WeatherMessageGenerator;

namespace InfoBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private IWeatherMessageGenerator _weatherMessageGenerator;

        public MessagesController(IWeatherMessageGenerator weatherMessageGenerator)
        {
            _weatherMessageGenerator = weatherMessageGenerator;
        }

        // Get api/Weather/{city}
        [HttpGet("/api/messages/weather/{city}")]
        public async Task<string> GetWeatherAsync(string city)
        {
            var forecastMessage = await _weatherMessageGenerator.GetMessageAsync(city);
            return forecastMessage;
        }
    }
}