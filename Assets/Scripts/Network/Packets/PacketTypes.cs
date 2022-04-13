using System;
using System.Linq;
using Sabotris.Network.Packets.Game;

namespace Sabotris.Network.Packets
{
    public enum PacketTypeId
    {
        GameStart = 0x00,
        GameEnd = 0x01,
        
        ChatMessage = 0x10,

        ShapeCreate = 0x20,
        ShapeMove = 0x21,
        ShapeRotate = 0x22,
        ShapeLock = 0x23,

        BlockBulkRemove = 0x31,
        FallingBlockCreate = 0x32,

        LayerMove = 0x40,
        LayerClear = 0x41,
        LayerAdd = 0x42,

        PlayerConnected = 0x90,
        PlayerDisconnected = 0x91,
        PlayerList = 0x92,
        RetrievePlayerList = 0x93,
        PlayerDead = 0x94,
        PlayerScore = 0x95,

        ServerShutdown = 0x1000
    }

    public class PacketType
    {
        public PacketTypeId Id { get; }
        public Func<Packet> NewPacket { get; }

        public PacketType(PacketTypeId id, Func<Packet> newPacket)
        {
            Id = id;
            NewPacket = newPacket;
        }
    }

    public static class PacketTypes
    {
        public static readonly PacketType GameStart = new PacketType(PacketTypeId.GameStart, () => new PacketGameStart());
        public static readonly PacketType GameEnd = new PacketType(PacketTypeId.GameEnd, () => new PacketGameEnd());
        
        public static readonly PacketType ChatMessage = new PacketType(PacketTypeId.ChatMessage, () => new PacketChatMessage());

        public static readonly PacketType ShapeCreate = new PacketType(PacketTypeId.ShapeCreate, () => new PacketShapeCreate());
        public static readonly PacketType ShapeMove = new PacketType(PacketTypeId.ShapeMove, () => new PacketShapeMove());
        public static readonly PacketType ShapeRotate = new PacketType(PacketTypeId.ShapeRotate, () => new PacketShapeRotate());
        public static readonly PacketType ShapeLock = new PacketType(PacketTypeId.ShapeLock, () => new PacketShapeLock());

        public static readonly PacketType BlockBulkRemove = new PacketType(PacketTypeId.BlockBulkRemove, () => new PacketBlockBulkRemove());
        public static readonly PacketType FallingBlockCreate = new PacketType(PacketTypeId.FallingBlockCreate, () => new PacketFallingBlockCreate());

        public static readonly PacketType LayerMove = new PacketType(PacketTypeId.LayerMove, () => new PacketLayerMove());
        public static readonly PacketType LayerClear  = new PacketType(PacketTypeId.LayerClear, () => new PacketLayerClear());
        public static readonly PacketType LayerAdd  = new PacketType(PacketTypeId.LayerAdd, () => new PacketLayerClear());

        public static readonly PacketType PlayerConnected = new PacketType(PacketTypeId.PlayerConnected, () => new PacketPlayerConnected());
        public static readonly PacketType PlayerDisconnected = new PacketType(PacketTypeId.PlayerDisconnected, () => new PacketPlayerDisconnected());
        public static readonly PacketType PlayerList = new PacketType(PacketTypeId.PlayerList, () => new PacketPlayerList());
        public static readonly PacketType RetrievePlayerList = new PacketType(PacketTypeId.RetrievePlayerList, () => new PacketRetrievePlayerList());
        public static readonly PacketType PlayerDead = new PacketType(PacketTypeId.PlayerDead, () => new PacketPlayerDead());
        public static readonly PacketType PlayerScore = new PacketType(PacketTypeId.PlayerScore, () => new PacketPlayerScore());

        public static readonly PacketType ServerShutdown = new PacketType(PacketTypeId.ServerShutdown, () => new PacketServerShutdown());

        public static PacketType GetPacketType(PacketTypeId packetTypeId)
        {
            return new[] {GameStart, GameEnd, ChatMessage, ShapeCreate, ShapeMove, ShapeRotate, ShapeLock, BlockBulkRemove, FallingBlockCreate, LayerMove, LayerClear, LayerAdd, PlayerConnected, PlayerDisconnected, PlayerList, RetrievePlayerList, PlayerDead, PlayerScore, ServerShutdown}
                .First((packetType) => packetType.Id == packetTypeId);
        }
    }
}