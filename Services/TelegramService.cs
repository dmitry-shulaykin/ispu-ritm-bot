using GradesNotification.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GradesNotification.Services
{
    
    public class TelegramService
    {
        private readonly BotService _botService;
        private readonly ILogger<TelegramService> _logger;
        private readonly StudentsRepository _studentsRepository;

        public TelegramService(ILogger<TelegramService> logger, BotService botService, StudentsRepository studentsRepository) {
            _botService = botService;
            _logger = logger;
            _studentsRepository = studentsRepository;
        }

        public async void NotifyNewMark(MarkChangedModel change)
        {
            var student = await _studentsRepository.GetByRitmLogin(change.Student);
            if (student.ChatId != 0)
            {
                await _botService.Client.SendTextMessageAsync(student.ChatId, 
                    $"You recieved new mark(${change.Type}) *{change.PrevValue}->{change.Value}* for subject {change.SubjectName}({change.Semester})", ParseMode.Markdown);
            }
        }

        public void NotifyJobRun(Student student)
        {
            _botService.Client.SendTextMessageAsync(student.ChatId, "No updates for today.(");
        }

        public void NotifyError(Student student, Exception e)
        {
            _botService.Client.SendTextMessageAsync(student.ChatId, $"Today's job failed. Error: {e.Message}");
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
                                if (segments.Length != 3)
                                {
                                    await _botService.Client.SendTextMessageAsync(chatId, "usage /login {ritm_login} {ritm_password}");
                                    throw new Exception("segments don't match");
                                }

                                var ritmLogin = segments[1];
                                var ritmPassword = segments[2];

                                var student = await _studentsRepository.GetByRitmLogin(ritmLogin);
                                if (student != null)
                                {
                                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Your login *{ritmLogin}* is already on the list", ParseMode.Markdown);
                                } else
                                {
                                        await _studentsRepository.CreateStudent(new Student { ChatId = chatId, RitmLogin = ritmLogin, Password = ritmPassword });
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogInformation($"Couldn't create login. Exception: {e}, ChatId: {message.Chat.Id}, Message: { message.Text}.");
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't create login. {e.Message}");
                            }
                            break;
                        }
                        case Constants.UpdateAction:
                            {
                                try
                                {
                                    if (segments.Length != 4)
                                    {
                                        await _botService.Client.SendTextMessageAsync(chatId, "usage /update {ritm_login} {old_ritm_password} {new_password}");
                                        throw new Exception("segments don't match");
                                    }

                                    var ritmLogin = segments[1];
                                    var oldRitmPassword = segments[2];
                                    var ritmPassword = segments[3];

                                    var student = await _studentsRepository.GetByRitmLogin(ritmLogin);
                                    if (student.Password != oldRitmPassword)
                                    {
                                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Old password doesn't match", ParseMode.Markdown);
                                    }
                                    else
                                    {
                                        student.Password = ritmPassword;
                                        await _studentsRepository.UpdateStudent(student);
                                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Password has been updated", ParseMode.Markdown);
                                    }
                                }
                                catch (Exception e)
                                {
                                    _logger.LogInformation($"Couldn't create login. Exception: {e}, ChatId: {message.Chat.Id}, Message: { message.Text}.");
                                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, $"Couldn't update password. {e.Message}");
                                }
                                break;
                            }
                        default: {
                                _logger.LogInformation($"Unsupported sequence. ChatId: {message.Chat.Id}, Message: { message.Text}.");
                                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "usage /login or /update");
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Message type is text, couldnt sent back {e}");
                }
            }
        }
    }
}
