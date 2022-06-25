using System.Collections;
using Sabotris.Network.Packets.Layer;
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

        protected override IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
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