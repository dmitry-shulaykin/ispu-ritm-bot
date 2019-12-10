using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GradesNotification.Services;
using Telegram.Bot.Types;

namespace GradesNotification.Controllers
{
    [Route("api/[controller]")]
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
            await _updateService.EchoAsync(update);
            return Ok();
        }
    }
}