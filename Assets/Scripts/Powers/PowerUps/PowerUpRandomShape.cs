using System;
using System.Collections;
using Sabotris.Network.Packets.Shape;
using Sabotris.Util;
using Sabotris.Worlds;
using UnityEngine;
using Random = Sabotris.Util.Random;

namespace Sabotris.Powers.PowerUps
{
    public class PowerUpRandomShape : PowerUp
    {
        public override Power GetPower()
        {
            return Power.RandomShape;
        }

        protected override IEnumerator OnSelectedContainer(Container activatingContainer, Container selectedContainer)
        {
            var blocksPerShape = (selectedContainer.networkController ? selectedContainer.networkController.Client?.LobbyData.BlocksPerShape : null) ?? 4;
            var generateVerticalBlocks = (selectedContainer.networkController ? selectedContainer.networkController.Client?.LobbyData.GenerateVerticalBlocks : null) ?? false;
            var offsets = ShapeUtil.Generate(blocksPerShape, generateVerticalBlocks, selectedContainer.GenerateBottomLeft, selectedContainer.GenerateTopRight);
            var packet = new PacketFallingShapeCreate
            {
                Id = Guid.NewGuid(),
                ContainerId = selectedContainer.Id,
                Position = selectedContainer.GetRandomStartingPosition(offsets),
                Offsets = offsets,
                Color = Random.RandomColor()
            };
            var fallingBlock = selectedContainer.CreateFallingShape(packet.Id, packet.Position, packet.Offsets, packet.Color);
            activatingContainer.networkController.Client?.SendPacket(packet);
            yield return new WaitUntil(() => fallingBlock.removed);
            yield return new WaitForSeconds(1);
        }
    }
}