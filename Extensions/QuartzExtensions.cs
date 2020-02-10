namespace GradesNotification.Extensions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GradesNotification.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;

    public static class QuartzExtensions
    {

        public static void AddQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, QuartzJobFactory>();

            services.AddSingleton(provider =>
            {
                var schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start();
                return scheduler;
            });
        }

        /// <summary>
        /// app builder extension for starting all jobs.
        /// </summary>
        /// <param name="app">AppBuilder.</param>
        /// <param name="jobTypes">type of job, every job should implement IBaseJob and have an empty constructor.</param>
        public static void UseQuartz(this IApplicationBuilder app)
        {
            var scheduler = app.ApplicationServices.GetService<IScheduler>();

            var job = JobBuilder.Create<CrawlStudentJob>()
                   .WithIdentity(CrawlStudentJob.JobName)
                   .Build();

            var trigger = CrawlStudentJob.CreateTrigger();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}
