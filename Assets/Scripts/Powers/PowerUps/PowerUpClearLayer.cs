using System.Collections;
using Sabotris.Network.Packets.Layer;
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
            activatingContainer.networkController.Client?.SendPacket(packet);
            yield return new WaitForSeconds(1);
        }
    }
}