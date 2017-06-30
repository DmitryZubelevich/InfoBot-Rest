using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Newtonsoft.Json;
using Serilog;
using System.Linq;
using InfoBot.MessageGenerators.WeatherMessageGenerator;

namespace InfoBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WebHookController : Controller
    {
        private readonly ITelegramBotClient _botClient;
        private IWeatherMessageGenerator _weatherMessageGenerator;
        private static ILogger Logger = Log.ForContext<WebHookController>();

        public WebHookController(ITelegramBotClient botClient, IWeatherMessageGenerator weatherMessageGenerator)
        {
            _botClient = botClient;
            _weatherMessageGenerator = weatherMessageGenerator;
        }

        // POST api/webhook
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var message = update?.Message;
            if (message == null)
            {
                var messageString = JsonConvert.SerializeObject(update);
                Log.Warning($"POST api/webhook: update event with empty message: '{messageString}'");
                return Ok();
            }

            if (message.Type == MessageType.TextMessage)
            {
                try
                {
                    var parts = message.Text.Split(' ');
                    var commandPart = parts.FirstOrDefault()?.Trim();

                    var responseMessage = message.Text;
                    if(commandPart.Equals("погода", StringComparison.OrdinalIgnoreCase))
                    {
                        var city = parts.LastOrDefault()?.Trim();
                        if (string.IsNullOrEmpty(city))
                        {
                            responseMessage = "Укажите пожалуйста город!";
                        }
                        else
                        {
                            responseMessage = await _weatherMessageGenerator.GetMessageAsync(city);
                        }
                    }

                    await _botClient.SendTextMessageAsync(message.Chat.Id, responseMessage);
                }catch(Exception e)
                {
                    Log.Warning($"POST api/webhook: exception on sending text message: '{e.Message}'");
                }
            }

            return Ok();
        }

        // GET api/webhook
        [HttpGet]
        public async Task<string> GetAsync()
        {
            var result = await _botClient.TestApiAsync();
            var me = await _botClient.GetMeAsync();
            var response = $"Me - {me.Username}. Test result: {result}";
            return response;
        }
    }
}