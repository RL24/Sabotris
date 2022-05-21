using JetBrains.Annotations;
using Sabotris.UI.Menu.Menus;
using Sabotris.Worlds;
using UnityEngine;

namespace Sabotris.Network
{
    public class NetworkController : MonoBehaviour
    {
        public World world;

        [CanBeNull] public Client Client { get; private set; }
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