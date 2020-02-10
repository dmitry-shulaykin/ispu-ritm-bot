using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GradesNotification.Models;
using GradesNotification.Services;
using GradesNotification.Extensions;

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

        [HttpGet]
        public IActionResult Get()
        {
            return OkJson("Hello world");
        }

        [HttpGet("{ritmLogin}")]
        public async Task<IActionResult> GetStudent(string ritmLogin)
        {
            try
            {
                var student = await _studentsRepository.GetByRitmLogin(ritmLogin);
                if (student == null)
                {
                    return NotFoundJson();
                }

                return OkJson(student);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't get student with login {ritmLogin}", e);
            }
        }

        [HttpGet("{ritmLogin}/semester/{semesterNumber}")]
        public async Task<IActionResult> GetSemester(string ritmLogin, int semesterNumber)
        {
            try
            {
                using var task = _logger.GetTelemetryEventDisposable("Get semester");
                var semester = await _studentsRepository.GetStudentSemester(ritmLogin, semesterNumber);
                if (semester == null)
                {
                    return NotFoundJson();
                }

                return OkJson(semester);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't get student with login {ritmLogin} semester {semesterNumber}.", e);
            }
        }

        [HttpGet("{ritmLogin}/semester/{semesterNumber}/subject/{subjectName}")]
        public async Task<IActionResult> GetSubject(string ritmLogin, int semesterNumber, string subjectName)
        {
            try
            {
                var subject = await _studentsRepository.GetStudentSubject(ritmLogin, semesterNumber, subjectName);
                if (subject == null)
                {
                    return NotFoundJson();
                }

                return OkJson(subject);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't get subject with login {ritmLogin} semester {semesterNumber} subject {subjectName}", e);
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] Student student)
        {
            try
            {
                var newStudent = await _studentsRepository.CreateStudent(student);

                return OkJson(newStudent);
            }
            catch (Exception e)
            {
                throw new Exception($"Exception when creating user {student.RitmLogin}", e);
            }
        }

        [HttpPost("{ritmLogin}/semester/")]
        public async Task<IActionResult> PostStudent(string ritmLogin, int semesterNumber, [FromBody] Semester semester)
        {
            try
            {
                var newSemester = await _studentsRepository.CreateSemester(ritmLogin, semester);

                return OkJson(newSemester);
            }
            catch (Exception e)
            {
                throw new Exception($"Exception when creating user {ritmLogin} semester {semesterNumber}.", e);
            }
        }


        [HttpPost("{ritmLogin}/semester/{semesterNumber}/subject")]
        public async Task<IActionResult> PostSubject(string ritmLogin, int semesterNumber, [FromBody] Subject subject)
        {
            try
            {
                var newSubject = await _studentsRepository.CreateSubject(ritmLogin, semesterNumber, subject);

                return OkJson(newSubject);
            }
            catch (Exception e)
            {
                throw new Exception($"Exception when creating semester {ritmLogin} {semesterNumber}.", e);
            }
        }

        [HttpPut("{ritmLogin}")]
        public async Task<IActionResult> PutStudent(string ritmLogin, [FromBody] Student student)
        {
            try
            {
                student.RitmLogin = ritmLogin;
                var newStudent = await _studentsRepository.UpdateStudent(student);
                return OkJson(newStudent);
            }
            catch (Exception e)
            {
                throw new Exception($"Update student {ritmLogin} failed.", e);
            }
        }

        [HttpPut("{ritmLogin}/semester/{semeterNumber}")]
        public async Task<IActionResult> PutSemester(string ritmLogin, int semesterNumber, [FromBody] Semester semester)
        {
            try
            {
                semester.Number = semesterNumber;
                var newSemester = await _studentsRepository.UpdateSemester(ritmLogin, semester);
                return OkJson(newSemester);
            }
            catch (Exception e)
            {
                throw new Exception($"Update student {ritmLogin} semester {semesterNumber} failed.", e);
            }
        }

        [HttpPut("{ritmLogin}/semester/{semesterNumber}/subject/{subjectName}")]
        public async Task<IActionResult> PutSubject(string ritmLogin, int semesterNumber, string subjectName, [FromBody] Subject subject)
        {
            try
            {
                subject.Name = subjectName;
                var newSubject = await _studentsRepository.UpdateSubject(ritmLogin, semesterNumber, subject);
                return OkJson(newSubject);
            }
            catch (Exception e)
            {
                throw new Exception($"Update student {ritmLogin} semester {semesterNumber} subject {subjectName} failed.", e);
            }
        }

        [HttpDelete("{ritmLogin}")]
        public async Task<IActionResult> DeleteStudent(string ritmLogin)
        {
            try
            {
                await _studentsRepository.DeleteStudent(ritmLogin);
                return OkJson<object>();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't delete student {ritmLogin}", e);
            }
        }

        [HttpDelete("{ritmLogin}/semester/{semesterNumber}/")]
        public async Task<IActionResult> DeleteSemester(string ritmLogin, int semesterNumber)
        {
            try
            {
                await _studentsRepository.DeleteSemester(ritmLogin, semesterNumber);
                return OkJson<object>();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't delete student {ritmLogin}", e);
            }
        }

        [HttpDelete("{ritmLogin}/semester/{semesterNumber}/subject/{subjectName}")]
        public async Task<IActionResult> DeleteSubject(string ritmLogin, int semesterNumber, string subjectName)
        {
            try
            {
                await _studentsRepository.DeleteSubject(ritmLogin, semesterNumber, subjectName);
                return OkJson<object>();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't delete student {ritmLogin}", e);
            }
        }

        private IActionResult OkJson<T>(T payload = default)
        {
            return new OkObjectResult(new { ok = true, payload, error = "" });
        }

        private IActionResult NotFoundJson()
        {
            object payload = null;
            return new NotFoundObjectResult(new { ok = false, payload, error = "" });
        }
    }
}