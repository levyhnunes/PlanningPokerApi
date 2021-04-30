using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Controllers;
using PlanningPoker.Api.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Api.Hubs
{
    public class PlanningPokerHub : Hub
    {
        private readonly PlanningPokerContext _context;

        public PlanningPokerHub(PlanningPokerContext context)
        {
            _context = context;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientconnecetionId = Context.ConnectionId;
            ConnectedPlayer.ConnectedIds.Remove(clientconnecetionId);
            var player = _context.Players.FirstOrDefault(u => u.ConnectionId.Equals(clientconnecetionId));

            if (player != null)
            {
                _context.Remove(player);
                _context.SaveChanges();
            }

            Clients.All.SendAsync("DisconnectedPlayer", clientconnecetionId);

            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
