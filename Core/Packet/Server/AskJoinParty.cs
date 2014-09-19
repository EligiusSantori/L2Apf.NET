using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class AskJoinParty : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			LeaderName = reader.ReadString(Encoding.Unicode);
			PartyLoot = (Lineage.PartyLoot)reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x39; } }
		public string LeaderName { get; set; }
		public Lineage.PartyLoot PartyLoot { get; set; }
	}
}
