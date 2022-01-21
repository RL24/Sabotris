﻿using System;
using System.Linq;
using Sabotris.Network.Packets.Game;

namespace Sabotris.Network.Packets
{
    public enum PacketTypeId
    {
        GameStart = 0x00,
        GameEnd = 0x01,
        
        ConnectingHail = 0x10,
        
        ShapeCreate = 0x20,
        ShapeMove = 0x21,
        ShapeRotate = 0x22,
        ShapeLock = 0x23,
        
        BlockBulkMove = 0x30,
        BlockBulkRemove = 0x31,
        
        PlayerConnected = 0x90,
        PlayerDisconnected = 0x91,
        PlayerList = 0x92,
        PlayerDead = 0x93
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

    public class PacketTypes
    {
        public static readonly PacketType GameStart = new PacketType(PacketTypeId.GameStart, () => new PacketGameStart());
        public static readonly PacketType GameEnd = new PacketType(PacketTypeId.GameEnd, () => new PacketGameEnd());
        
        public static readonly PacketType ConnectingHail = new PacketType(PacketTypeId.ConnectingHail, () => new PacketConnectingHail());

        public static readonly PacketType ShapeCreate = new PacketType(PacketTypeId.ShapeCreate, () => new PacketShapeCreate());
        public static readonly PacketType ShapeMove = new PacketType(PacketTypeId.ShapeMove, () => new PacketShapeMove());
        public static readonly PacketType ShapeRotate = new PacketType(PacketTypeId.ShapeRotate, () => new PacketShapeRotate());
        public static readonly PacketType ShapeLock = new PacketType(PacketTypeId.ShapeLock, () => new PacketShapeLock());
        
        public static readonly PacketType BlockBulkMove = new PacketType(PacketTypeId.BlockBulkMove, () => new PacketBlockBulkMove());
        public static readonly PacketType BlockBulkRemove = new PacketType(PacketTypeId.BlockBulkRemove, () => new PacketBlockBulkRemove());

        public static readonly PacketType PlayerConnected = new PacketType(PacketTypeId.PlayerConnected, () => new PacketPlayerConnected());
        public static readonly PacketType PlayerDisconnected = new PacketType(PacketTypeId.PlayerDisconnected, () => new PacketPlayerDisconnected());
        public static readonly PacketType PlayerList = new PacketType(PacketTypeId.PlayerList, () => new PacketPlayerList());
        public static readonly PacketType PlayerDead = new PacketType(PacketTypeId.PlayerDead, () => new PacketPlayerDead());

        public static PacketType GetPacketType(PacketTypeId packetTypeId)
        {
            return new [] {GameStart, GameEnd, ConnectingHail, ShapeCreate, ShapeMove, ShapeRotate, ShapeLock, BlockBulkMove, BlockBulkRemove, PlayerConnected, PlayerDisconnected, PlayerList, PlayerDead}
                .First((packetType) => packetType.Id == packetTypeId);
        }
    }
}