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

        public static void AddQuartz(this IServiceCollection services, params Type[] jobs)
        {
            services.AddSingleton<IJobFactory, QuartzJobFactory>();
            services.Add(jobs.Select(jobType => new ServiceDescriptor(jobType, jobType, ServiceLifetime.Singleton)));

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
        public static void UseQuartz(this IApplicationBuilder app, params Type[] jobTypes)
        {
            var scheduler = app.ApplicationServices.GetService<IScheduler>();
            foreach (var job in jobTypes)
            {
                QuartzServicesUtilities.StartJob(scheduler, job);
            }
        }
    }

    public static class QuartzServicesUtilities
    {
        /// <summary>
        /// Create instance of job by type, and Start Job.
        /// </summary>
        /// <param name="scheduler">Common scheduler.</param>
        /// <param name="jobType">Job type, should inherite BaseJob</param>
        /// <exception cref="NullReferenceException">jobType.FullName isn't a IBaseJob</exception>
        public static void StartJob(IScheduler scheduler, Type jobType)
        {
            var instance = (BaseJob)Activator.CreateInstance(jobType);

            if (instance == null)
            {
                throw new NullReferenceException($"jobType.FullName isn't a IBaseJob");
            }

            var job = JobBuilder.Create(jobType)
                .WithIdentity(instance.JobName)
                .Build();

            var trigger = instance.CreateTrigger();
            scheduler.ScheduleJob(job, trigger);
        }
    }

    public abstract class BaseJob : IJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseJob"/> class.
        /// </summary>
        protected BaseJob()
        {
        }

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        public abstract string JobName { get; }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            return this.Execute();
        }

        /// <summary>
        /// Creates the trigger.
        /// </summary>
        /// <returns></returns>
        public abstract ITrigger CreateTrigger();

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        protected abstract Task Execute();
    }


}

