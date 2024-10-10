using IO.Swagger.Model;
using Microsoft.AspNetCore.Mvc;
using SmartCloud.Server.Services.Interfaces;

namespace SmartCloud.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KravOgBetalingerController : ControllerBase
    {
        private readonly IKravOgBetalinger _kravOgBetalingerService;    

        public KravOgBetalingerController(IKravOgBetalinger kravOgBetalingerService)
        {
            _kravOgBetalingerService = kravOgBetalingerService;
        }

        [HttpGet("aapnekrav")]
        public async Task<IActionResult> GetAapneKrav([FromQuery] string orgno, [FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? correlation)
        {

            AapneKrav? kravOgBetalinger = await _kravOgBetalingerService.GetAapneKrav(orgno, from, to, correlation);
            return Ok(kravOgBetalinger);
        }

        [HttpGet("utbetalinger")]
        public async Task<IActionResult> GetUtbetalinger([FromQuery] string orgno, [FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? correlation)
        {

            Utbetalinger? kravOgBetalinger = await _kravOgBetalingerService.GetUtbetalinger(orgno, from, to, correlation);
            return Ok(kravOgBetalinger);
        }

        [HttpGet("innbetalinger")]
        public async Task<IActionResult> GetInnbetalinger([FromQuery] string orgno, [FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? correlation)
        {

            Innbetalinger? innbetalinger = await _kravOgBetalingerService.GetInnbetalinger(orgno, from, to, correlation);
            return Ok(innbetalinger);
        }

        [HttpGet("krav")]
        public async Task<IActionResult> GetKravOgBetalinger([FromQuery] string orgno, [FromQuery] string from, [FromQuery] string? to, [FromQuery] string? correlation)
        {
            Krav? krav = await _kravOgBetalingerService.GetKrav(orgno, from, to, correlation);
            return Ok(krav);
        }
    }
}
