using System;
using System.Collections.Generic;

public class CFG_ItemBinaryConfig 
{
	public List<CFG_Item> items;
	public Dictionary<int,CFG_Item> dict;



	#region read write
	public static CFG_ItemBinaryConfig Read(byte[] source)
	{
		CFG_ItemBinaryConfig info = new CFG_ItemBinaryConfig();
		ByteBuffer bfs = new ByteBuffer();
		bfs.source = source;

		int len = bfs.ReadInt32();
		info.items = new List<CFG_Item>();
		info.dict = new Dictionary<int,CFG_Item>();
		for(int i=0;i<len;++i)
		{
			info.items.Add(CFG_Item.Read(bfs.ReadBytes()));
			info.dict.Add(info.items[i].ID,info.items[i]);
		}

		return info;
	}

	public static byte[] Write(CFG_ItemBinaryConfig source)
	{
		ByteBuffer bfs = new ByteBuffer();

		int len = source.items.Count;
		bfs.WriteInt32(len);
		for (int i=0;i<len;++i)
		{
			bfs.WriteBytes(CFG_Item.Write(source.items[i]));
		}
		

		return bfs.source;
	}
	#endregion

	public class CFG_Item
	{
		public int ID;
		public string Name;
		public string Des;
		public int Cost_D;
		public int Cost_M;
		public string IconPath;
		public int ItemType;
		public string UseAudioPath;
		public string Values;
		public int Continue_Second;
		
		
		#region read write
		public static CFG_Item Read(byte[] source)
		{
			CFG_Item info = new CFG_Item();
			ByteBuffer bfs = new ByteBuffer();
			bfs.source = source;

			info.ID = bfs.ReadInt32();
			info.Name = bfs.ReadString();
			info.Des = bfs.ReadString();
			info.Cost_D = bfs.ReadInt32();
			info.Cost_M = bfs.ReadInt32();
			info.IconPath = bfs.ReadString();
			info.ItemType = bfs.ReadInt32();
			info.UseAudioPath = bfs.ReadString();
			info.Values = bfs.ReadString();
			info.Continue_Second = bfs.ReadInt32();

			return info;
		}

		public static byte[] Write(CFG_Item source)
		{
			ByteBuffer bfs = new ByteBuffer();

			bfs.WriteInt32(source.ID);
			bfs.WriteString(source.Name);
			bfs.WriteString(source.Des);
			bfs.WriteInt32(source.Cost_D);
			bfs.WriteInt32(source.Cost_M);
			bfs.WriteString(source.IconPath);
			bfs.WriteInt32(source.ItemType);
			bfs.WriteString(source.UseAudioPath);
			bfs.WriteString(source.Values);
			bfs.WriteInt32(source.Continue_Second);

			return bfs.source;
		}
		#endregion
	}
	
}

