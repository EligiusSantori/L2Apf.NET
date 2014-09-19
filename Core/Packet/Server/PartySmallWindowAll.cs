using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class PartySmallWindowAll : Packet
	{
		public struct Member
		{
			public Int32 ObjectId { get; set; }
			public string Name { get; set; }
			public Int32 Cp { get; set; }
			public Int32 MaxCp { get; set; }
			public Int32 Hp { get; set; }
			public Int32 MaxHp { get; set; }
			public Int32 Mp { get; set; }
			public Int32 MaxMp { get; set; }
			public Int32 Level { get; set; }
			public Int32 Class { get; set; }
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			LeaderId = reader.ReadInt32();
			Loot = (Lineage.PartyLoot)reader.ReadInt32();
			Members = new Member[reader.ReadInt32()];
			for(int i = 0; i < Members.Length; i++)
			{
				Members[i] = new Member()
				{
					ObjectId = reader.ReadInt32(),
					Name = reader.ReadString(Encoding.Unicode),
					Cp = reader.ReadInt32(),
					MaxCp = reader.ReadInt32(),
					Hp = reader.ReadInt32(),
					MaxHp = reader.ReadInt32(),
					Mp = reader.ReadInt32(),
					MaxMp = reader.ReadInt32(),
					Level = reader.ReadInt32(),
					Class = reader.ReadInt32()
				};

				reader.ReadInt32();
				reader.ReadInt32();
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x4e; } }
		public Int32 LeaderId { get; set; }
		public Member[] Members { get; set; }
		public Lineage.PartyLoot Loot { get; set; }
	}
}
