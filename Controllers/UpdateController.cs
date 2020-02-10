using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GradesNotification.Services;
using Telegram.Bot.Types;
using GradesNotification.Models;

namespace GradesNotification.Controllers
{
    [Route("api/update")]
    public class UpdateController : Controller
    {
        private readonly TelegramService _updateService;
        private readonly RitmService _ritmService;
        private readonly StudentsRepository _repository;
        public UpdateController(TelegramService updateService, RitmService ritmService, StudentsRepository repository)
        {
            _ritmService = ritmService;
            _updateService = updateService;
            _repository = repository;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateService.HandleUpdate(update);
            return OkJson<object>();
        }

        [HttpPost("new_grades")] 
        public async Task<IActionResult> NewGrades([FromBody] UpdateMarksRequest request)
        {
            var student = await _repository.GetByRitmLogin(request.RitmLogin);
            var updates = await _ritmService.CheckUpdatesAsync(student);

            return OkJson(updates);
        }

        [HttpPost("ritm_semesters")]
        public async Task<IActionResult> RitmSemesters([FromBody] UpdateMarksRequest request)
        {
            var student = await _repository.GetByRitmLogin(request.RitmLogin);
            var semesters = await _ritmService.ParseAllSemesters(student);
            student.Semesters = semesters;
            await _repository.UpdateStudent(student);
            return OkJson(semesters);
        }

        private IActionResult OkJson<T>(T payload = default)
        {
            return new OkObjectResult(new { ok = true, payload, error = "" });
        }
    }
}