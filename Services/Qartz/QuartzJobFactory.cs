using System;

namespace GradesNotification.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Quartz.Spi;

    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var ritmService = serviceProvider.GetService<RitmService>();
            var studentsRepository = serviceProvider.GetService<StudentsRepository>();
            var logger = serviceProvider.GetService<ILogger<CrawlStudentJob>>();
            var telegramService = serviceProvider.GetService<TelegramService>();

            return new CrawlStudentJob(ritmService, studentsRepository, logger, telegramService);
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
