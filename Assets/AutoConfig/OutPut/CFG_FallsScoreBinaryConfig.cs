using System;
using System.Collections.Generic;

public class CFG_FallsScoreBinaryConfig 
{
	public List<CFG_FallsScore> items;
	public Dictionary<int,CFG_FallsScore> dict;



	#region read write
	public static CFG_FallsScoreBinaryConfig Read(byte[] source)
	{
		CFG_FallsScoreBinaryConfig info = new CFG_FallsScoreBinaryConfig();
		ByteBuffer bfs = new ByteBuffer();
		bfs.source = source;

		int len = bfs.ReadInt32();
		info.items = new List<CFG_FallsScore>();
		info.dict = new Dictionary<int,CFG_FallsScore>();
		for(int i=0;i<len;++i)
		{
			info.items.Add(CFG_FallsScore.Read(bfs.ReadBytes()));
			info.dict.Add(info.items[i].ID,info.items[i]);
		}

		return info;
	}

	public static byte[] Write(CFG_FallsScoreBinaryConfig source)
	{
		ByteBuffer bfs = new ByteBuffer();

		int len = source.items.Count;
		bfs.WriteInt32(len);
		for (int i=0;i<len;++i)
		{
			bfs.WriteBytes(CFG_FallsScore.Write(source.items[i]));
		}
		

		return bfs.source;
	}
	#endregion

	public class CFG_FallsScore
	{
		public int ID;
		public int Score;
		public string UpStep;
		public int Hurt;
		public string Name;
		
		
		#region read write
		public static CFG_FallsScore Read(byte[] source)
		{
			CFG_FallsScore info = new CFG_FallsScore();
			ByteBuffer bfs = new ByteBuffer();
			bfs.source = source;

			info.ID = bfs.ReadInt32();
			info.Score = bfs.ReadInt32();
			info.UpStep = bfs.ReadString();
			info.Hurt = bfs.ReadInt32();
			info.Name = bfs.ReadString();

			return info;
		}

		public static byte[] Write(CFG_FallsScore source)
		{
			ByteBuffer bfs = new ByteBuffer();

			bfs.WriteInt32(source.ID);
			bfs.WriteInt32(source.Score);
			bfs.WriteString(source.UpStep);
			bfs.WriteInt32(source.Hurt);
			bfs.WriteString(source.Name);

			return bfs.source;
		}
		#endregion
	}
	
}

