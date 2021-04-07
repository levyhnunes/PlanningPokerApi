using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Data;
using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace PlanningPoker.Api.Controllers
{
    [Route("api/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IHubContext<PlanningPokerHub> _hubContext;
        private readonly PlanningPokerContext _context;

        public RoomController(IHubContext<PlanningPokerHub> hubContext, PlanningPokerContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        [HttpGet]
        [Route("GetPlayers")]
        public IActionResult GetPlayers()
        {
            return Ok(_context.Players.ToList());
        }


        [HttpPost]
        [Route("AddPlayer")]
        public IActionResult AddPlayer([FromBody] PlayerViewModel model)
        {
            var player = _context.Players.FirstOrDefault(u => u.Name.Equals(model.Name));

            if (player != null)
            {
                player.ChangeConnectionId(model.ConnectionId);
            }
            else
            {
                player = new Player(model.Name, model.ConnectionId);
                _context.Players.Add(player);
            }

            _context.SaveChanges();

            _hubContext.Clients.All.SendAsync("ReceivePlayer", new { id = player.Id, connectionId = model.ConnectionId, name = model.Name });

            return Ok(model);
        }

        [HttpPost]
        [Route("AddCard")]
        public IActionResult AddCard([FromBody] PlayerCard model)
        {
            var player = _context.Players.FirstOrDefault(u => u.ConnectionId.Equals(model.ConnectionId));

            if (player != null)
            {
                player.Card = model.Card;

                _context.SaveChanges();

                _hubContext.Clients.All.SendAsync("ReceivePlayerCard", new { connectionId = model.ConnectionId, card = model.Card });
            }

            return Ok(model);
        }
    }
}
