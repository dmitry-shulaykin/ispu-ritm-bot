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
using Telegram.Bot.Types.ReplyMarkups;

namespace GradesNotification.Services
{
    
    public class TelegramService
    {
        private readonly BotService _botService;
        private readonly ILogger<TelegramService> _logger;
        private readonly StudentsRepository _studentsRepository;
        private readonly RitmService _ritmService;

        public TelegramService(ILogger<TelegramService> logger, BotService botService, StudentsRepository studentsRepository, RitmService ritmService) {
            _botService = botService;
            _logger = logger;
            _studentsRepository = studentsRepository;
            _ritmService = ritmService;
        }

        public void NotifyNewMark(MarkChangedModel change)
        {
            var student = _studentsRepository.GetByRitmLogin(change.Student);
            if (student.ChatId != 0)
            {
                _botService.Client.SendTextMessageAsync(student.ChatId, 
                    $"You recieved new mark *{change.PrevValue}->{change.Value}* for subject {change.SubjectName}({change.Semestr})", ParseMode.Markdown);
            }
        }

        public void NotifyError(long chatId, string error)
        {
            _botService.Client.SendTextMessageAsync(chatId, error);
        }

        public async Task HandleUpdate(Update update)
        {
            
            if (update == null || update.Type != UpdateType.Message)
            {
                _logger.LogWarning("update == null or update type is not message");
                return;
            }

            var message = update.Message;

            if (message == null) 
            {
                _logger.LogWarning("message == null");
                return;
            }

            var chatId = message.Chat.Id;
            var messId = message.MessageId;

            
            if (message.Type == MessageType.Text)
            {
                // Echo each Message
                _logger.LogInformation("Received Message from {0} {1}", message.Chat.Id, message.Text);
                try 
                {
                    var segments = message.Text.Split(' ');
                    if (segments.Length == 0)
                    {
                        return;
                    }

                    switch (segments[0])
                    {
                        case Constants.LoginAction:
                            {
                                try
                                {
                                    if (segments.Length != 2)
                                    {
                                        throw new Exception("segments don't match");
                                    }

                                    var ritmLogin = segments[1];
                                    var isNewUser = await _studentsRepository.UpdateOrRegisterStudentWithChatIdAsync(chatId, ritmLogin);
                                    if (isNewUser)
                                    {
                                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Your login *{ritmLogin}* successfully added to crawling list.", ParseMode.Markdown);
                                    } 
                                    else
                                    {
                                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Your login *{ritmLogin}* was already on the list.", ParseMode.Markdown);
                                    }
                                } 
                                catch(Exception e)
                                {
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't create login. Please use: {Constants.LoginAction} your_ritm_login");
                                    _logger.LogInformation($"Couldn't create login. Exception: {e}, ChatId: {message.Chat.Id}, Message: { message.Text}.");
                                }
                                break;
                            }
                        case Constants.PasswordAction:
                            {
                                try
                                {
                                    if (segments.Length != 2)
                                    {
                                        throw new Exception("segments don't match");
                                    }

                                    var ritmPassword = segments[1];
                                    await _studentsRepository.UpdateStudentPasswordASync(chatId, ritmPassword);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogInformation($"Couldn't create password. Exception: {e}, ChatId: {message.Chat.Id}, Message: { message.Text}.");
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't update password. {e.Message}");
                                }
                                break;
                            }
                        case Constants.UpdateAction:
                            {
                                try
                                {
                                    var student = _studentsRepository.GetByChatId(chatId);
                                    student.Semesters = await _ritmService.ParseAllSemesters(student);
                                    _studentsRepository.Update(student.Id, student);
                                } 
                                catch (Exception e)
                                {
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't update your marks.{e.Message}");
                                }
                                break;
                            }
                        case Constants.CheckAction:
                            {
                                try
                                {
                                    if (segments.Length != 2)
                                    {
                                        throw new Exception("segments don't match");
                                    }

                                    var semestr = int.Parse(segments[1]);
                                    var student = _studentsRepository.GetByChatId(chatId);

                                    var t = Newtonsoft.Json.JsonSerializer.Create();
                                    var w = new StringWriter();
                                    t.Serialize(w, student.Semesters[semestr]);

                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, w.ToString());

                                }
                                catch (Exception e)
                                {
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't get your marks.{e.Message}");
                                }


                                break;
                            }

                    }
                    // await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
                }  
                catch (Exception e)
                {
                    _logger.LogError($"Message type is text, couldnt sent back {e}");
                }
            }
            else if (message.Type == MessageType.Photo)
            {
               try 
                {
                    var fileId = message.Photo.LastOrDefault()?.FileId;
                    var file = await _botService.Client.GetFileAsync(fileId);

                    var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                    using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                    {
                        await _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                    }

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
                }  
                catch (Exception e)
                {
                    _logger.LogError($"Message type is photo, couldnt sent back. {e}");
                }
            }
            else {
                _logger.LogInformation($"Message type unknow {update} {message} {message.Type}");
            }
        }

        public void DisplayMenu(long chatId, int messageId)
        {
            _botService.Client.SendTextMessageAsync(
                chatId: chatId,
                replyToMessageId: messageId,
                text: "ISPU Ritm bot",
                replyMarkup: new InlineKeyboardMarkup(
                    new InlineKeyboardButton[] {
                        InlineKeyboardButton.WithCallbackData("Обновить данные", $"launchCrawler {chatId}"),
                    }
                )
            );
        }
    }
}
