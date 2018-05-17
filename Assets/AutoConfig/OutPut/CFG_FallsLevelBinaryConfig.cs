using System;
using System.Collections.Generic;

public class CFG_FallsLevelBinaryConfig 
{
	public List<CFG_FallsLevel> items;
	public Dictionary<int,CFG_FallsLevel> dict;



	#region read write
	public static CFG_FallsLevelBinaryConfig Read(byte[] source)
	{
		CFG_FallsLevelBinaryConfig info = new CFG_FallsLevelBinaryConfig();
		ByteBuffer bfs = new ByteBuffer();
		bfs.source = source;

		int len = bfs.ReadInt32();
		info.items = new List<CFG_FallsLevel>();
		info.dict = new Dictionary<int,CFG_FallsLevel>();
		for(int i=0;i<len;++i)
		{
			info.items.Add(CFG_FallsLevel.Read(bfs.ReadBytes()));
			info.dict.Add(info.items[i].ID,info.items[i]);
		}

		return info;
	}

	public static byte[] Write(CFG_FallsLevelBinaryConfig source)
	{
		ByteBuffer bfs = new ByteBuffer();

		int len = source.items.Count;
		bfs.WriteInt32(len);
		for (int i=0;i<len;++i)
		{
			bfs.WriteBytes(CFG_FallsLevel.Write(source.items[i]));
		}
		

		return bfs.source;
	}
	#endregion

	public class CFG_FallsLevel
	{
		public int ID;
		public int Section;
		public int TotalCount;
		public string Values;
		public string Name;
		public float Interval_Second;
		public string missionTarget;
		public int Type;
		public int NeedGolden;
		
		
		#region read write
		public static CFG_FallsLevel Read(byte[] source)
		{
			CFG_FallsLevel info = new CFG_FallsLevel();
			ByteBuffer bfs = new ByteBuffer();
			bfs.source = source;

			info.ID = bfs.ReadInt32();
			info.Section = bfs.ReadInt32();
			info.TotalCount = bfs.ReadInt32();
			info.Values = bfs.ReadString();
			info.Name = bfs.ReadString();
			info.Interval_Second = bfs.ReadFloat();
			info.missionTarget = bfs.ReadString();
			info.Type = bfs.ReadInt32();
			info.NeedGolden = bfs.ReadInt32();

			return info;
		}

		public static byte[] Write(CFG_FallsLevel source)
		{
			ByteBuffer bfs = new ByteBuffer();

			bfs.WriteInt32(source.ID);
			bfs.WriteInt32(source.Section);
			bfs.WriteInt32(source.TotalCount);
			bfs.WriteString(source.Values);
			bfs.WriteString(source.Name);
			bfs.WriteFloat(source.Interval_Second);
			bfs.WriteString(source.missionTarget);
			bfs.WriteInt32(source.Type);
			bfs.WriteInt32(source.NeedGolden);

			return bfs.source;
		}
		#endregion
	}
	
}

