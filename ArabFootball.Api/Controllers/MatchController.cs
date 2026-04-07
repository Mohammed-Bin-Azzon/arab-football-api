using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Features.Matchs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_training.Controllers;
using ArabFootball.Api.Shared.Entity;
using static ArabFootball.Shared.Helpers.Routing;

namespace ArabFootball.Api.Controllers
{
    [Route(Matches.Prefix)]
    [Authorize(Roles = "Admin")]
    public class MatchController : AppControllerBase
    {
        private readonly IMatchService _service;

        public MatchController(IMatchService service)
        {
            _service = service;
        }

        
        [HttpGet(Matches.GetAll)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return  Response(await _service.GetAllMatchesAsync(pageNumber, pageSize, search));
        }

        [HttpGet(Matches.GetById)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Response(await _service.GetMatchByIdAsync(id));
        }

        [HttpPut(Matches.Add)]
        public async Task<IActionResult> Create([FromBody]CreateMatchDto dto)
        {
            int adminId = int.Parse(User.FindFirst("UserId").Value);

            return Response(await _service.CreateMatchAsync(dto, adminId));
        }

        [HttpPut(Matches.Update)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]UpdateMatchDto dto)
        {
            return Response(await _service.UpdateMatchAsync(id, dto));
        }

        [HttpDelete(Matches.Delete)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
           return Response(await _service.DeleteMatchAsync(id));
        }

        [HttpPatch(Matches.ChangeStatus)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody]MatchStatus status)
        {
            return Response(await _service.ChangeStatusAsync(id, status));
        }

        [HttpPatch(Matches.OpenPredictions)]
        public async Task<IActionResult> OpenPredictions([FromRoute]int id)
        {
            return Response(await _service.OpenPredictionsAsync(id));
        }

        [HttpPatch(Matches.ClosePredictions)]
        public async Task<IActionResult> ClosePredictions([FromRoute]int id)
        {
            return Response(await _service.ClosePredictionsAsync(id));
        }

        [HttpPatch(Matches.LinkChat)]
        public async Task<IActionResult> LinkChat([FromRoute]int id, [FromBody] string chatUrl)
        {
            return Response(await _service.LinkChatAsync(id, chatUrl));
        }

        [HttpPatch(Matches.UnlinkChat)]
        public async Task<IActionResult> UnlinkChat([FromRoute] int id)
        {
            return Response(await (_service.UnlinkChatAsync(id)));
        }
    }
}
