using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.IO;
using GradesNotification.Models;
using GradesNotification.Services;

namespace GradesNotification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private ILogger<StudentsController> _logger;
        private readonly StudentsRepository _studentsRepository;
        public StudentsController(ILogger<StudentsController> logger, StudentsRepository studentsRepository)
        {
            _logger = logger;
            _studentsRepository = studentsRepository;
        }


        [HttpGet("{ritm_login}")]
        public IActionResult Get(string ritm_login)
        {
            try
            {
                var student = _studentsRepository.GetByRitmLogin(ritm_login);
                if (student == null)
                {
                    _logger.LogWarning($"Can't find student with login {ritm_login}.");
                }

                var result = Json(new { ok = student != null, student });
                result.StatusCode = (int)(student == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Can't get student with login {ritm_login}. Exception: {e.ToString()}");
                var result = Json(new { ok = false, student = (Student)null });
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpPut("{ritm_login}")]
        public IActionResult Update(string ritm_login, [FromBody] Student student)
        {
            try
            {
                _studentsRepository.Update(ritm_login, student);
                return Json(new { ok = true });
            }
            catch (Exception e)
            {
                _logger.LogError($"Update failed: {e}");
                var result = Json(new { ok = false });
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpDelete("{ritm_login}")]
        public IActionResult Delete(string ritm_login)
        {
            try
            {
                _studentsRepository.Remove(ritm_login);
                return Json(new { ok = true });
            }
            catch (Exception e)
            {
                _logger.LogError($"Update failed: {e}");
                var result = Json(new { ok = false });
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

        [HttpGet("{ritm_login}/semestr/{semestr_number}")]
        public IActionResult Get(string ritm_login, int semestr_number)
        {
            try
            {
                var student = _studentsRepository.GetByRitmLogin(ritm_login);
                var semestr = student.Semesters.FirstOrDefault(s.Numeber == semestr_number);
                var result = Json(new { ok = true, semestr);
                if (semestr == null)
                {
                    result.StatusCode = (int) HttpStatusCode.NotFound;
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"Can't get student with login {ritm_login}. Exception: {e.ToString()}");
                var result = Json(new { ok = false });
                result.StatusCode = (int)HttpStatusCode.BadRequest;
                return result;
            }
        }

    }
}