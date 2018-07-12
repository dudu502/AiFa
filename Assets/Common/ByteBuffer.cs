using System;
using System.Text;

public class ByteBuffer
{
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

