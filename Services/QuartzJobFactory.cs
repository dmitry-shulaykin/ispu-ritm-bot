using System;

namespace GradesNotification.Services
{
    using Microsoft.Extensions.DependencyInjection;

    using Quartz;
    using Quartz.Spi;

    public class QuartzJobFactory : IJobFactory
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzJobFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates new job.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;

            var job = (IJob)this.serviceProvider.GetRequiredService(jobDetail.JobType);
            return job;
        }

        /// <summary>
        /// Returns the job.
        /// </summary>
        /// <param name="job">The job.</param>
        public void ReturnJob(IJob job)
        {
        }

    }
}
