using System;
using System.Collections.Generic;

public class {0}BinaryConfig 
{{
	public List<{0}> items;
	public Dictionary<int,{0}> dict;



	#region read write
	public static {0}BinaryConfig Read(byte[] source)
	{{
		{0}BinaryConfig info = new {0}BinaryConfig();
		ByteBuffer bfs = new ByteBuffer();
		bfs.source = source;

		int len = bfs.ReadInt32();
		info.items = new List<{0}>();
		info.dict = new Dictionary<int,{0}>();
		for(int i=0;i<len;++i)
		{{
			info.items.Add({0}.Read(bfs.ReadBytes()));
			info.dict.Add(info.items[i].ID,info.items[i]);
		}}

		return info;
	}}

	public static byte[] Write({0}BinaryConfig source)
	{{
		ByteBuffer bfs = new ByteBuffer();

		int len = source.items.Count;
		bfs.WriteInt32(len);
		for (int i=0;i<len;++i)
		{{
			bfs.WriteBytes({0}.Write(source.items[i]));
		}}
		

		return bfs.source;
	}}
	#endregion

	public class {0}
	{{
		{1}
		
		#region read write
		public static {0} Read(byte[] source)
		{{
			{0} info = new {0}();
			ByteBuffer bfs = new ByteBuffer();
			bfs.source = source;

			{2}

			return info;
		}}

		public static byte[] Write({0} source)
		{{
			ByteBuffer bfs = new ByteBuffer();

			{3}

			return bfs.source;
		}}
		#endregion
	}}
	
}}

