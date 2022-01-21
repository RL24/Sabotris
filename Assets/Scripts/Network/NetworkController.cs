using UnityEngine;

namespace Sabotris.Network
{
    public class NetworkController : MonoBehaviour
    {
        public Client Client { get; } = new Client();
        public Server Server { get; } = new Server();

        private void Update()
        {
            Server?.Update();
        }
    }
}