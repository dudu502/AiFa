using System;
using System.Collections.Generic;
//**************************************************************************
//  name:PtOutBinary.cs
//  desc:
//  ver:v1.1
//**************************************************************************
public class PtOutBinary 
{

    
    /// <summary>
    /// name
    /// </summary>
    private string _class_name;
    public string class_name
    {
        get { return _class_name; }
        set { _class_name = value; has_class_name = true; }
    }
    public bool has_class_name;

    /// <summary>
    /// 
    /// </summary>
    private int _field_count;
    public int field_count
    {
        get { return _field_count; }
        set { _field_count = value; has_field_count = true; }
    }
    public bool has_field_count;

    /// <summary>
    /// items
    /// </summary>
    private List<PtOutNode> _rows;
    public List<PtOutNode> rows
    {
        get { return _rows; }
        set { _rows = value; has_rows = true; }
    }
    public bool has_rows;


    #region 读写
    public static byte[] Write(PtOutBinary value)
    {
        //创建字节缓存
        ByteBuffer bfs = new ByteBuffer();

        
        //写入class_name
        bfs.WriteBool(value.has_class_name);
        if (value.has_class_name)
        {
            bfs.WriteString(value.class_name);
        }


        //写入field_count
        bfs.WriteBool(value.has_field_count);
        if (value.has_field_count)
        {
            bfs.WriteInt32(value.field_count);
        }


        //写入rows数组
        bfs.WriteBool(value.has_rows);
        if (value.has_rows)
        {
            int len = value.rows.Count;
            bfs.WriteInt32(len);
            for (int i = 0; i < len; ++i)
            {
                bfs.WriteBytes(PtOutNode.Write(value.rows[i]));
            }
        }



        //返回 字节数组
        return bfs.source;
    }
    
    public static PtOutBinary Read(byte[] value)
    {
        //创建结构体
        PtOutBinary info = new PtOutBinary();
        //创建字节缓存
        ByteBuffer bfs = new ByteBuffer();
        bfs.source = value;
        
        
        //读取class_name
        if (bfs.ReadBool())
        {
            info.class_name = bfs.ReadString();
        }


        //读取field_count
        if (bfs.ReadBool())
        {
            info.field_count = bfs.ReadInt32();
        }


        //读取rows数组
        if (bfs.ReadBool())
        {
            int len = bfs.ReadInt32();
            info.rows = new List<PtOutNode>();
            for (int i = 0; i < len; ++i)
            {
                info.rows.Add(PtOutNode.Read(bfs.ReadBytes()));
            }
        }



        //返回 PtOutBinary 实例
        return info;
    }
    #endregion

        
    

}

