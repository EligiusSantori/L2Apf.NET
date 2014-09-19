using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class PledgeShowInfoUpdate : Packet
	{

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ClanId = reader.ReadInt32();
			CrestId = reader.ReadInt32();
			ClanLevel = reader.ReadInt32();
			HasCastle = reader.ReadInt32() != 0;
			HasClanhall = reader.ReadInt32() != 0;
			reader.ReadInt32();
			CharLevel = reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x88; } }

		public Int32 ClanId { get; set; }
		public Int32 CrestId { get; set; }
		public Int32 ClanLevel { get; set; }
		public Int32 CharLevel { get; set; }

		public bool HasCastle { get; set; }
		public bool HasClanhall { get; set; }
	}
}
