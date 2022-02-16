using System;
using System.Collections.Generic;

namespace Sabotris.Network.Packets
{
    public class ByteBufferException : Exception
    {
    }
    
    public class ByteBuffer
    {
        private int _offset = 0;
        private readonly List<byte> _bytes = new List<byte>();

        public ByteBuffer()
        {
        }

        public ByteBuffer(byte[] bytes)
        {
            _bytes.AddRange(bytes);
        }

        public void ResetOffset()
        {
            _offset = 0;
        }

        public void CheckOffset()
        {
            if (_offset > _bytes.Count)
                throw new ByteBufferException();
        }
        
        public byte ReadByte()
        {
            CheckOffset();
            var value = _bytes.GetRange(_offset, 1)[0];
            _offset++;
            return value;
        }

        public bool ReadBoolean()
        {
            CheckOffset();
            var value = BitConverter.ToBoolean(Bytes, _offset);
            _offset++;
            return value;
        }

        public char ReadChar()
        {
            CheckOffset();
            var value = BitConverter.ToChar(Bytes, _offset);
            _offset += 2;
            return value;
        }

        public short ReadInt16()
        {
            CheckOffset();
            var value = BitConverter.ToInt16(Bytes, _offset);
            _offset += 2;
            return value;
        }

        public ushort ReadUInt16()
        {
            CheckOffset();
            var value = BitConverter.ToUInt16(Bytes, _offset);
            _offset += 2;
            return value;
        }

        public int ReadInt32()
        {
            CheckOffset();
            var value = BitConverter.ToInt32(Bytes, _offset);
            _offset += 4;
            return value;
        }

        public uint ReadUInt32()
        {
            CheckOffset();
            var value = BitConverter.ToUInt32(Bytes, _offset);
            _offset += 4;
            return value;
        }

        public long ReadInt64()
        {
            CheckOffset();
            var value = BitConverter.ToInt64(Bytes, _offset);
            _offset += 8;
            return value;
        }

        public ulong ReadUInt64()
        {
            CheckOffset();
            var value = BitConverter.ToUInt64(Bytes, _offset);
            _offset += 8;
            return value;
        }

        public double ReadDouble()
        {
            CheckOffset();
            var value = BitConverter.ToDouble(Bytes, _offset);
            _offset += 8;
            return value;
        }

        public float ReadFloat()
        {
            CheckOffset();
            var value = BitConverter.ToSingle(Bytes, _offset);
            _offset += 4;
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