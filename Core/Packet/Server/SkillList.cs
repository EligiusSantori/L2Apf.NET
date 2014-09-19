using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class SkillList : Packet
	{
		public class SkillItem
		{
			public int Id { get; set; }
			public int Level { get; set; }
			public bool Active { get; set; }
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id

			int Count = reader.ReadInt32();
			List = new List<SkillItem>(Count);
			for (int i = 0; i < Count; i++)
				List.Add(new SkillItem()
				{
					Active = !(reader.ReadInt32() != 0),
					Level = reader.ReadInt32(),
					Id = reader.ReadInt32()
				});
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}


		public override byte Id { get { return 0x58; } }
		public List<SkillItem> List { get; set; }
	}
}
