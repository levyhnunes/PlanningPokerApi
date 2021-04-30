using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PlanningPoker.Api.Data;
using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Models;
using PlanningPoker.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanningPoker.Api.Controllers
{
    [Route("api/Rooms")]
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

        public IActionResult Get()
        {
            var room = _context.Rooms.Include(p => p.Players).FirstOrDefault();

            room.Players = room.Players.Where(p => ConnectedPlayer.ConnectedIds.Contains(p.ConnectionId)).ToList();

            return Ok(new
            {
                Id = room.Id,
                Reveal = room.Reveal,
                Players = room.Players.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Card,
                    p.ConnectionId,
                    p.IsAdmin
                })
            });
        }

        [HttpPut]
        public IActionResult Put([FromBody] RoomViewModel model)
        {
            var room = _context.Rooms.Include(p => p.Players).FirstOrDefault(p => p.Id == model.Id);

            room.Reveal = model.Reveal;

            if (!model.Reveal)
            {
                room.Players.ToList().ForEach(p => p.Card = null);
            }

            _context.SaveChanges();

            _hubContext.Clients.All.SendAsync($"UpdateRoom#{room.Id}", new { reveal = model.Reveal });

            return Ok();
        }


        [HttpPost]
        [Route("{roomId}/Players")]
        public IActionResult AddPlayer(int roomId, [FromBody] PlayerViewModel model)
        {
            var room = _context.Rooms.FirstOrDefault();
            var player = _context.Players.FirstOrDefault(u => u.Name.Equals(model.Name));

            if (player != null)
            {
                player.ChangeConnectionId(model.ConnectionId);
            }
            else
            {
                player = new Player(model.Name, model.ConnectionId, room.Id);
                _context.Players.Add(player);
            }

            player.IsAdmin = false;

            ConnectedPlayer.ConnectedIds.Add(model.ConnectionId);
            // no active user is an administrator
            if (!_context.Players.Any(p => ConnectedPlayer.ConnectedIds.Contains(p.ConnectionId) && p.IsAdmin))
            {
                player.IsAdmin = true;
            }

            _context.SaveChanges();

            _hubContext.Clients.All.SendAsync("ReceivePlayer", new
            {
                id = player.Id,
                connectionId = model.ConnectionId,
                name = model.Name,
                isAdmin = player.IsAdmin,
                card = player.Card
            });

            return Ok(model);
        }

        [HttpPut]
        [Route("Players")]
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

        [HttpPost]
        [Route("NewPlayerAdm")]
        public IActionResult AddCard([FromBody] NewPlayerAdmViewModel model)
        {
            var newAdm = _context.Players.FirstOrDefault(u => u.Id == model.NewAdmId);

            if (newAdm == null)
            {
                throw new ApplicationException("Usuário não encontrado");
            }

            newAdm.IsAdmin = true;

            var oldAdm = _context.Players.FirstOrDefault(u => u.Id == model.OldAdmId);

            if (oldAdm != null)
            {
                oldAdm.IsAdmin = false;
            }

            _context.SaveChanges();

            _hubContext.Clients.All.SendAsync("ReceivePlayer", new
            {
                id = newAdm.Id,
                connectionId = newAdm.ConnectionId,
                name = newAdm.Name,
                isAdmin = newAdm.IsAdmin,
                card = newAdm.Card
            });

            return Ok(model);
        }
    }
}
