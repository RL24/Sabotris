using System.Collections;
using Sabotris.Network.Packets.Game;
using UnityEngine;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpClearLayer : PowerUp
    {
        public override Power GetPower()
        {
            return Power.ClearLayer;
        }

        public override void Use(Container activatingContainer)
        {
            activatingContainer.cameraController.SetSelectingContainer(OnSelectedContainer, activatingContainer, new[] {activatingContainer});
        }

        private IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            var packet = new PacketLayerClear()
            {
                ContainerId = selectedContainer.id,
                Layer = 0
            };
            // yield return selectedContainer.StartClearingLayers(new [] {new Vector3Int(0, packet.Layer, 0)}, true);
            activatingContainer.networkController.Client.SendPacket(packet);
            yield return new WaitForSeconds(2);
        }
    }
}