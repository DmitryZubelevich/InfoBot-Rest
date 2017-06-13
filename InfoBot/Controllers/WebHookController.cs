using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Newtonsoft.Json;

namespace InfoBot.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WebHookController : Controller
    {
        private readonly ITelegramBotClient _botClient;

        public WebHookController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        // POST api/webhook
        [HttpPost]
        public async Task<IActionResult> Post(Update update)
        {
            var messageString = JsonConvert.SerializeObject(update);
            Console.WriteLine(messageString);

            var message = update?.Message;

            if(message == null)
            {
                return Ok();
            }

            if (message.Type == MessageType.TextMessage)
            {
                // Echo each Message
                await _botClient.SendTextMessageAsync(message.Chat.Id, message.Text);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var result = await _botClient.TestApiAsync();
            var me = await _botClient.GetMeAsync();
            return $"Me - {me.Username}. Test result: {result}";
        }
    }
}