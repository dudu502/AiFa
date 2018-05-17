using System;
using System.Collections.Generic;
//**************************************************************************
//  name:PtOutNode.cs
//  desc:Item
//  ver:v1.1
//**************************************************************************
public class PtOutNode 
{

    
    /// <summary>
    /// item data
    /// </summary>
    private List<string> _cols;
    public List<string> cols
    {
        get { return _cols; }
        set { _cols = value; has_cols = true; }
    }
    public bool has_cols;


    #region 读写
    public static byte[] Write(PtOutNode value)
    {
        //创建字节缓存
        ByteBuffer bfs = new ByteBuffer();

        
        //写入cols数组
        bfs.WriteBool(value.has_cols);
        if (value.has_cols)
        {
            int len = value.cols.Count;
            bfs.WriteInt32(len);
            for (int i = 0; i < len; ++i)
            {
                bfs.WriteString(value.cols[i]);
            }
        }



        //返回 字节数组
        return bfs.source;
    }
    
    public static PtOutNode Read(byte[] value)
    {
        //创建结构体
        PtOutNode info = new PtOutNode();
        //创建字节缓存
        ByteBuffer bfs = new ByteBuffer();
        bfs.source = value;
        
        
        //读取cols数组
        if (bfs.ReadBool())
        {
            int len = bfs.ReadInt32();
            info.cols = new List<string>();
            for (int i = 0; i < len; ++i)
            {
                info.cols.Add(bfs.ReadString());
            }
        }



        //返回 PtOutNode 实例
        return info;
    }
    #endregion

        
    

}

