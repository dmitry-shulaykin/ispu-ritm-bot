using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GradesNotification.Services;
using Telegram.Bot.Types;
using System.IO;

namespace GradesNotification.Controllers
{
    [Route("api/update")]
    public class UpdateController : Controller
    {
        private readonly TelegramService _updateService;

        public UpdateController(TelegramService updateService)
        {
            _updateService = updateService;
        }

        // POST api/update
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await _updateService.HandleUpdate(update);
            return Ok();
        }
    }
}