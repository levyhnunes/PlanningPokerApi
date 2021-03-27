using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlanningPoker.Api.Controllers
{
    [Route("api/PlanningPoker")]
    [ApiController]
    public class PlanningPokerController : ControllerBase
    {
        private IHubContext<PlanningPokerHub> _hubContext;
        public static Dictionary<string, string> users = new Dictionary<string, string>();

        public PlanningPokerController(IHubContext<PlanningPokerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            users.Add(user.ConnectionId, user.Name);

            _hubContext.Clients.All.SendAsync("ReceiveUser", new { connectionId = user.ConnectionId, name = user.Name });

            return Ok(users.Select(x => new User
            {
                ConnectionId = x.Key,
                Name = x.Value
            }));
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            return Ok(users.Select(x => new User
            {
                ConnectionId = x.Key,
                Name = x.Value
            }));
        }
    }
}
