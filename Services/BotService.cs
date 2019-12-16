using Microsoft.Extensions.Options;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace GradesNotification.Services
{
    public class BotService
    {
        private readonly BotConfiguration _config;

        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = new TelegramBotClient(_config.BotToken);
        }

        public TelegramBotClient Client { get; }
    }
}
