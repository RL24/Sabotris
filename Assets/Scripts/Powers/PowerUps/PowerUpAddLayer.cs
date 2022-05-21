using System.Collections;
using Sabotris.Network.Packets.Game;
using Sabotris.Worlds;
using UnityEngine;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpAddLayer : PowerUp
    {
        public override Power GetPower()
        {
            return Power.AddLayer;
        }

        public override void Use(Container activatingContainer)
        {
            activatingContainer.cameraController.SetSelectingContainer(OnSelectedContainer, activatingContainer, new[] {activatingContainer});
        }

        private IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            var packet = new PacketLayerAdd
            {
                ContainerId = selectedContainer.Id,
                Layer = 1
            };
            yield return selectedContainer.AddLayer(packet.Layer);
            activatingContainer.networkController.Client?.SendPacket(packet);
            yield return new WaitForSeconds(1);
        }
    }
}