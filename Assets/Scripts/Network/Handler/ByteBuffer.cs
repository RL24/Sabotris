using System;
using System.Collections.Generic;

namespace Sabotris.Network.Packets
{
    public class ByteBufferException : Exception
    {
        public ByteBufferException(string message) : base(message)
        {
        }
    }

    public class ByteBuffer
    {
        public PacketType PacketType;
        private int _offset;
        private readonly List<byte> _bytes = new List<byte>();

        public ByteBuffer()
        {
        }

        public ByteBuffer(byte[] bytes)
        {
            _bytes.AddRange(bytes);
        }

        private void CheckOffset(int readSize)
        {
            if (_offset > _bytes.Count)
                throw new ByteBufferException($"Error reading packet {PacketType.Id}: Read start index {_offset} exceeding written bytes {_bytes.Count}");
            if (_offset + readSize > _bytes.Count)
                throw new ByteBufferException($"Error reading packet {PacketType.Id}: Read end index {_offset + readSize} exceeding written bytes {_bytes.Count}");
        }

        public byte ReadByte()
        {
            const int size = 1;
            CheckOffset(size);
            var value = _bytes.GetRange(_offset, size)[0];
            _offset += size;
            return value;
        }

        public sbyte ReadSByte()
        {
            const int size = 1;
            CheckOffset(size);
            var value = (sbyte) _bytes.GetRange(_offset, size)[0];
            _offset += size;
            return value;
        }

        public bool ReadBoolean()
        {
            const int size = 1;
            CheckOffset(size);
            var value = BitConverter.ToBoolean(Bytes, _offset);
            _offset += size;
            return value;
        }

        public char ReadChar()
        {
            const int size = 2;
            CheckOffset(size);
            var value = BitConverter.ToChar(Bytes, _offset);
            _offset += size;
            return value;
        }

        public short ReadInt16()
        {
            const int size = 2;
            CheckOffset(size);
            var value = BitConverter.ToInt16(Bytes, _offset);
            _offset += size;
            return value;
        }

        public ushort ReadUInt16()
        {
            const int size = 2;
            CheckOffset(size);
            var value = BitConverter.ToUInt16(Bytes, _offset);
            _offset += size;
            return value;
        }

        public int ReadInt32()
        {
            const int size = 4;
            CheckOffset(size);
            var value = BitConverter.ToInt32(Bytes, _offset);
            _offset += size;
            return value;
        }

        public uint ReadUInt32()
        {
            const int size = 4;
            CheckOffset(size);
            var value = BitConverter.ToUInt32(Bytes, _offset);
            _offset += size;
            return value;
        }

        public long ReadInt64()
        {
            const int size = 8;
            CheckOffset(size);
            var value = BitConverter.ToInt64(Bytes, _offset);
            _offset += size;
            return value;
        }

        public ulong ReadUInt64()
        {
            const int size = 8;
            CheckOffset(size);
            var value = BitConverter.ToUInt64(Bytes, _offset);
            _offset += size;
            return value;
        }

        public double ReadDouble()
        {
            const int size = 8;
            CheckOffset(size);
            var value = BitConverter.ToDouble(Bytes, _offset);
            _offset += size;
            return value;
        }

        public float ReadFloat()
        {
            const int size = 4;
            CheckOffset(size);
            var value = BitConverter.ToSingle(Bytes, _offset);
            _offset += size;
            return value;
        }

        public string ReadString()
        {
            var length = ReadInt32();
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = ReadChar();
            var value = new string(chars);
            return value;
        }

        public void Write(byte value)
        {
            _bytes.Add(value);
        }

        public void Write(sbyte value)
        {
            _bytes.Add((byte) value);
        }

        public void Write(bool value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(char value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(short value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(ushort value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(ulong value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            _bytes.AddRange(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            Write(value.Length);
            var arr = value.ToCharArray();
            foreach (var c in arr)
                Write(c);
        }

        public byte[] Bytes => _bytes.ToArray();
    }
}