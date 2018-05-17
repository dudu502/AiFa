using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

public class ByteBuffer
{
    //MemoryStream ms = null;
    //BinaryReader br = null;
    //BinaryWriter bw = null;
    //public ByteBuffer(byte[] source)
    //{
    //    ms = new MemoryStream(source);
    //}
    //public ByteBuffer()
    //{
    //    ms = new MemoryStream();
    //}
    //public byte[] Getbuffer() { return ms.GetBuffer(); }

    //public ByteBuffer WriteBool(bool value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public bool ReadBool()
    //{
    //    return Reader.ReadBoolean();
    //}

    //public ByteBuffer WriteByte(byte value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public byte ReadByte() { return Reader.ReadByte(); }

    //public ByteBuffer WriteSByte(sbyte value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public sbyte ReadSByte() { return Reader.ReadSByte(); }

    //public ByteBuffer WriteShort(short value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public short ReadShort() { return Reader.ReadInt16(); }
    //public ByteBuffer WriteInt32(int value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public int ReadInt32()
    //{
    //    return Reader.ReadInt32();
    //}

    //public ByteBuffer WriteFloat(float value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public float ReadFloat()
    //{
    //    return Reader.ReadSingle();
    //}

    //public ByteBuffer WriteLong(long value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}
    //public long ReadLong() { return Reader.ReadInt64(); }
    //public ByteBuffer WriteString(string value)
    //{
    //    Writer.Write(value);
    //    return this;
    //}

    //public string ReadString()
    //{
    //    return Reader.ReadString();
    //}


    //public ByteBuffer WriteBytes(byte[] value)
    //{
    //    WriteInt32(value.Length);
    //    Writer.Write(value);
    //    return this;
    //}

    //public byte[] ReadBytes() { return Reader.ReadBytes(ReadInt32()); }

    //BinaryWriter Writer
    //{
    //    get
    //    {
    //        bw = bw ?? new BinaryWriter(ms);
    //        return bw;
    //    }
    //}
    //BinaryReader Reader
    //{
    //    get
    //    {
    //        br = br ?? new BinaryReader(ms);
    //        return br;
    //    }
    //}



    private byte[] bytes = new byte[0];

    private int position = 0;
    public ByteBuffer(byte[] value)
    {
        source = value;

    }
    public ByteBuffer()
    {


    }

    public byte[] Getbuffer() { return source; }
    public void ResetPosition()
    {
        position = 0;
    }
    public byte[] source
    {
        get { return bytes; }
        set
        {
            bytes = value;
            ResetPosition();
        }
    }
    public void WriteInt32(int value)
    {
        copy(BitConverter.GetBytes(value));
    }

    public int ReadInt32()
    {
        return BitConverter.ToInt32(get(4), 0);
    }

    public void WriteShort(short value)
    {
        copy(BitConverter.GetBytes(value));
    }
    public short ReadShort()
    {
        return BitConverter.ToInt16(get(2), 0);
    }

    public void WriteString(string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        WriteInt32(data.Length);
        copy(data);
    }
    public string ReadString()
    {
        return Encoding.UTF8.GetString(get(ReadInt32()));
    }

    public void WriteBytes(byte[] value)
    {
        WriteInt32(value.Length);
        copy(value);        
    }
    public byte[] ReadBytes()
    {
        return get(ReadInt32());
    }


    public void WriteByte(byte value)
    {
        copy(new byte[] { value });
    }
    public byte ReadByte()
    {
        return get(1)[0];
    }
    public void WriteBool(bool value)
    {
        WriteByte(Convert.ToByte(value));
    }
    public bool ReadBool()
    {
        return Convert.ToBoolean(ReadByte());
    }
    public void WriteLong(long value)
    {
        copy(BitConverter.GetBytes(value));
    }
    public long ReadLong()
    {
        return BitConverter.ToInt32(get(8), 0);
    }

    public void WriteFloat(float value)
    {
        copy(BitConverter.GetBytes(value));
    }
    public float ReadFloat()
    {
        return BitConverter.ToSingle(get(4), 0);
    }

    private void copy(byte[] value)
    {
        byte[] temps = new byte[bytes.Length + value.Length];
        Array.Copy(bytes, 0, temps, 0, bytes.Length);
        Array.Copy(value, 0, temps, position, value.Length);
        position += value.Length;
        bytes = temps;
        temps = null;
    }
    private byte[] get(int length)
    {
        byte[] data = new byte[length];
        Array.Copy(bytes, position, data, 0, length);
        position += length;
        return data;
    }

}

