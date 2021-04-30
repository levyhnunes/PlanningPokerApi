namespace PlanningPoker.Api.Models
{
    public class Player
    {
        protected Player()
        {

        }

        public Player(string name, string connectionId, int roomId)
        {
            Name = name;
            ConnectionId = connectionId;
            RoomId = roomId;
        }

        public int Id { get; set; }

        public string Name { get; private set; }

        public string ConnectionId { get; private set; }

        public int? Card { get; set; }

        public bool IsAdmin { get; set; }

        public int RoomId { get; private set; }

        public Room Room { get; private set; }

        public void ChangeConnectionId(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
