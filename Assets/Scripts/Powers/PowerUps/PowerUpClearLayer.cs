using System.Collections;
using Sabotris.Network.Packets.Game;
using Sabotris.Worlds;
using UnityEngine;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpClearLayer : PowerUp
    {
        public override Power GetPower()
        {
            return Power.ClearLayer;
        }

        protected override IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            var packet = new PacketLayerClear
            {
                ContainerId = selectedContainer.Id,
                Layer = 1
            };
            yield return selectedContainer.StartClearingLayers(new [] {new Vector3Int(0, packet.Layer, 0)}, true);
            activatingContainer.networkController.Client?.SendPacket(packet);
            yield return new WaitForSeconds(1);
        }
    }
}