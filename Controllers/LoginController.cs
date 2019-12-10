using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GradesNotification.Models;
using GradesNotification.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GradesNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        ILogger<LoginController> _logger;
        StudentsRepository _studentsRepository;

        public LoginController(ILogger<LoginController> logger, StudentsRepository studentsRepository, RitmService ritmService)
        {
            _logger = logger;
            _studentsRepository = studentsRepository;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation($"New user {request.RitmLogin}");

                await _studentsRepository.Create(new Student()
                {
                    Id = request.RitmLogin,
                    RitmLogin = request.RitmLogin,
                    Password = request.RitmPassword,
                });

                _logger.LogInformation($"Created user successfully {request.RitmLogin}");

                var result = Json(new { ok = true });
                result.StatusCode = (int)HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Exception when creating user {request.RitmLogin}. Exception: {e.ToString()}");

                var result = Json(new { ok = false });
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }


    }
}