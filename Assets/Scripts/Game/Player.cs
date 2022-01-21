
namespace Sabotris
{
    public class Player
    {
        public long Id { get; set; }
        public string Name { get; private set; }

        public Player(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}