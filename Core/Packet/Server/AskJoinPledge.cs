using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class AskJoinPledge : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			ClanName = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x32; } }
		public Int32 ObjectId { get; set; }
		public string ClanName { get; set; }
	}
}
