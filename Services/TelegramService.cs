using GradesNotification.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MihaZupan;

namespace GradesNotification.Services
{
    
    public class TelegramService
    {
        private readonly TelegramBotClient _client;
        private readonly BotConfiguration _config;
        private readonly ILogger<TelegramService> _logger;
        public TelegramService(ILogger<TelegramService> logger, IOptions<BotConfiguration> config) {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            _client = string.IsNullOrEmpty(_config.Socks5Host)
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port));
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }

            var message = update.Message;

            _logger.LogInformation("Received Message from {0}", message.Chat.Id);

            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                await _client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var fileId = message.Photo.LastOrDefault()?.FileId;
                var file = await _client.GetFileAsync(fileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await _client.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await _client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }
    }
}
