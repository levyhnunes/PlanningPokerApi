using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Controllers;
using System;
using System.Threading.Tasks;

namespace PlanningPoker.Api.Hubs
{
    public class PlanningPokerHub : Hub
    {
        public override Task OnDisconnectedAsync(Exception exception)
        {
            string clientconnecetionID = Context.ConnectionId;
            PlanningPokerController.users.Remove(clientconnecetionID);

            Clients.All.SendAsync("DisconnectedUser", Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
