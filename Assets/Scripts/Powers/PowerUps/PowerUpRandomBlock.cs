using System;
using System.Collections;
using Sabotris.Network.Packets.Game;
using Sabotris.Worlds;
using UnityEngine;
using Random = Sabotris.Util.Random;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpRandomBlock : PowerUp
    {
        public override Power GetPower()
        {
            return Power.RandomBlock;
        }

        public override void Use(Container activatingContainer)
        {
            activatingContainer.cameraController.SetSelectingContainer(OnSelectedContainer, activatingContainer, new[] {activatingContainer});
        }

        private IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            var packet = new PacketFallingBlockCreate
            {
                Id = Guid.NewGuid(),
                ContainerId = selectedContainer.Id,
                Position = selectedContainer.GetRandomStartingPosition(),
                Color = Random.RandomColor()
            };
            var fallingBlock = selectedContainer.CreateFallingBlock(packet.Id, packet.Position, packet.Color);
            activatingContainer.networkController.Client?.SendPacket(packet);
            yield return new WaitUntil(() => fallingBlock.removed);
            yield return new WaitForSeconds(1);
        }
    }
}