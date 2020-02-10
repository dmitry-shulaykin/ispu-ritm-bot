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
    public class CrawlStudentJob : IJob
    {
        private readonly RitmService _ritmService;
        private readonly StudentsRepository _studentsRepository;
        private readonly ILogger<CrawlStudentJob> _logger;
        private readonly TelegramService _telegramService;

        public CrawlStudentJob(RitmService ritmService, StudentsRepository studentsRepository, ILogger<CrawlStudentJob> logger, TelegramService telegramService)
        {
            Console.WriteLine("CrawlStudentJob(string ritmLogin, RitmService ritmService, StudentsRepository studentsRepository, ILogger<CrawlStudentJob> logger, TelegramService telegramService)");
            _ritmService = ritmService;
            _studentsRepository = studentsRepository;
            _telegramService = telegramService;
            _logger = logger;
        }

        public static string JobName => nameof(CrawlStudentJob);

        public static ITrigger CreateTrigger()
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{JobName}.Trigger")
                  .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder
                        .WithInterval(TimeSpan.FromMinutes(60))
                        .RepeatForever())
                //.WithDailyTimeIntervalSchedule
                //  (s =>
                //     s.WithIntervalInHours(24)
                //    .OnEveryDay()
                //    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(6, 0))
                //  )
                .Build();

            return trigger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Execute crawling job");
            return Task.Run(async () =>
            {
                List<Student> students;
                try
                { 
                    using (var task = _logger.GetTelemetryEventDisposable("getting students for crawling"))
                    {
                        students = await _studentsRepository.GetAll();
                    }

                    using (var task = _logger.GetTelemetryEventDisposable("crawling all students"))
                    {
                        students = await _studentsRepository.GetAll();
                        _logger.LogInformation($"Crawl students(Total {students.Count})");
                        foreach (var student in students)
                        {
                            try
                            {
                                using (var subtask = _logger.GetTelemetryEventDisposable($"crawling student {student.RitmLogin}"))
                                {
                                    var (changes, semesters) = await _ritmService.CheckUpdatesAsync(student);
                                    foreach (var change in changes)
                                    {
                                        _logger.LogInformation($"Have change for {student.RitmLogin} {change.SubjectName}");
                                        _telegramService.NotifyNewMark(change);
                                    }

                                    if (changes.Count == 0)
                                    {
                                        _telegramService.NotifyJobRun(student);
                                    }


                                    student.Semesters = semesters;
                                    await _studentsRepository.UpdateStudent(student);
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError($"Couldn't update marks for {student.RitmLogin}. Error: {e}");
                                _telegramService.NotifyError(student, e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error when crawling students. Exception: {e.ToString()}");
                }
            });
        }
    }
}
