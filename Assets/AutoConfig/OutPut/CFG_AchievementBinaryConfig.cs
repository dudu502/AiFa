using System;
using System.Collections.Generic;

public class CFG_AchievementBinaryConfig 
{
	public List<CFG_Achievement> items;
	public Dictionary<int,CFG_Achievement> dict;



	#region read write
	public static CFG_AchievementBinaryConfig Read(byte[] source)
	{
		CFG_AchievementBinaryConfig info = new CFG_AchievementBinaryConfig();
		ByteBuffer bfs = new ByteBuffer();
		bfs.source = source;

		int len = bfs.ReadInt32();
		info.items = new List<CFG_Achievement>();
		info.dict = new Dictionary<int,CFG_Achievement>();
		for(int i=0;i<len;++i)
		{
			info.items.Add(CFG_Achievement.Read(bfs.ReadBytes()));
			info.dict.Add(info.items[i].ID,info.items[i]);
		}

		return info;
	}

	public static byte[] Write(CFG_AchievementBinaryConfig source)
	{
		ByteBuffer bfs = new ByteBuffer();

		int len = source.items.Count;
		bfs.WriteInt32(len);
		for (int i=0;i<len;++i)
		{
			bfs.WriteBytes(CFG_Achievement.Write(source.items[i]));
		}
		

		return bfs.source;
	}
	#endregion

	public class CFG_Achievement
	{
		public int ID;
		public string Target;
		public string IconPath;
		
		
		#region read write
		public static CFG_Achievement Read(byte[] source)
		{
			CFG_Achievement info = new CFG_Achievement();
			ByteBuffer bfs = new ByteBuffer();
			bfs.source = source;

			info.ID = bfs.ReadInt32();
			info.Target = bfs.ReadString();
			info.IconPath = bfs.ReadString();

			return info;
		}

		public static byte[] Write(CFG_Achievement source)
		{
			ByteBuffer bfs = new ByteBuffer();

			bfs.WriteInt32(source.ID);
			bfs.WriteString(source.Target);
			bfs.WriteString(source.IconPath);

			return bfs.source;
		}
		#endregion
	}
	
}

