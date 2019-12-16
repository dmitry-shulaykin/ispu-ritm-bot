using GradesNotification.Extensions;
using GradesNotification.Models;
using GradesNotification.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification
{
    public class CrawlStudentJob : BaseJob
    {
        private readonly RitmService _ritmService;
        private readonly StudentsRepository _studentsRepository;
        private readonly string _ritmLogin;
        private readonly ILogger<CrawlStudentJob> _logger;
        private readonly TelegramService _telegramService;

        public CrawlStudentJob()
        {
        }


        public CrawlStudentJob(string ritmLogin, RitmService ritmService, StudentsRepository studentsRepository, ILogger<CrawlStudentJob> logger, TelegramService telegramService)
        {
            _ritmLogin = ritmLogin;
            _ritmService = ritmService;
            _studentsRepository = studentsRepository;
            _telegramService = telegramService;
            _logger = logger;
        }

        public override string JobName => nameof(CrawlStudentJob);

        public override ITrigger CreateTrigger()
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{JobName}.Trigger")
                .StartNow()
                .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever())
                .Build();

            return trigger;
        }

        protected override async Task Execute()
        {
            _logger.LogInformation($"Execute ");
            Console.WriteLine($"Execute ");
            await Task.Run(async () =>
            {
                try
                {
                    var students = _studentsRepository.GetAll();
                    foreach (var student in students)
                    {
                        try
                        {
                            var changes = await _ritmService.CheckUpdatesAsync(student);
                            _logger.LogInformation($"Updating marks for {student.RitmLogin}");
                            foreach (var change in changes)
                            {
                                _logger.LogInformation($"Have change for {student.RitmLogin} {change.SubjectName}");
                                _telegramService.NotifyNewMark(change);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Couldn't update marks for {student.RitmLogin}. Error: {e}");
                            _telegramService.NotifyError(student.ChatId, e.Message);
                        }
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError($"Error when crawling student {_ritmLogin}.  Exception: {e.ToString()}");
                }
            }).ConfigureAwait(false);
        }
    }
}
