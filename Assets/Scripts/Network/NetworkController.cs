using UI.Menu.Menus;
using UnityEngine;

namespace Sabotris.Network
{
    public class NetworkController : MonoBehaviour
    {
        public World world;

        public Client Client { get; private set; }
        public Server Server { get; private set; }

        private void Start()
        {
            Client = new Client(this);
            Server = new Server(this, world);
        }

        private void Update()
        {
            Client?.PollMessages();
            Server?.PollMessages();
        }
    }
}