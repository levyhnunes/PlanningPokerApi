namespace PlanningPoker.Api.Models
{
    public class Player
    {
        protected Player()
        {

        }

        public Player(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
        }

        public int Id { get; set; }

        public string Name { get; private set; }

        public string ConnectionId { get; private set; }

        public int Card { get; set; }

        public void ChangeConnectionId(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
