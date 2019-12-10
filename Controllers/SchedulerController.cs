using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradesNotification.Models;
using GradesNotification.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz.Spi;

namespace GradesNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly ILogger<SchedulerController> _logger;
        private readonly IJobFactory _jobFactory;
        public SchedulerController(ILogger<SchedulerController> scheduleUpdateRequest, IJobFactory jobFactory)
        {
            _logger = scheduleUpdateRequest;
            _jobFactory = jobFactory;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ScheduleUpdateRequest scheduleUpdateRequest)
        {
            try
            {
                // _jobFactory.NewJob()
            } 
            catch
            {

            }
            return Ok("ok");
        }
    }
}