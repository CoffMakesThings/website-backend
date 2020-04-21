﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using W3ChampionsStatisticService.Ports;

namespace W3ChampionsStatisticService.Matches
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;

        public MatchesController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMatches(
            int offset = 0,
            int pageSize = 50,
            GameMode gameMode = GameMode.Undefined,
            int gateWay = 10)
        {
            var matches = await _matchRepository.Load(gameMode, offset, pageSize, gateWay);
            var count = await _matchRepository.Count();
            return Ok(new { matches, count });
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetMatchesPerPlayer(
            string playerId,
            string opponentId = null,
            GameMode gameMode = GameMode.Undefined,
            int offset = 0,
            int pageSize = 100)
        {
            var matches = await _matchRepository.LoadFor(playerId, opponentId, gameMode, pageSize, offset);
            var count = await _matchRepository.CountFor(playerId, opponentId, gameMode);
            return Ok(new { matches, count });
        }
    }
}