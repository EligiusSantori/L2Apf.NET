using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class PledgeInfo : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ClanId = reader.ReadInt32();
			ClanName = reader.ReadString(Encoding.Unicode);
			AllyName = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x83; } }
		public Int32 ClanId { get; set; }
		public string ClanName { get; set; }
		public string AllyName { get; set; }
	}
}
