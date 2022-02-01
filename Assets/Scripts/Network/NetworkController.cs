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
            Client = new Client();
            Server = new Server(world);
        }
        
        private void Update()
        {
            Server?.Update();
        }
    }
}