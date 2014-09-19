using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class CreatureSay : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			Type = (Lineage.Channel)reader.ReadInt32();
			Author = reader.ReadString(Encoding.Unicode);
			Message = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x4a; } }
		public Int32 ObjectId { get; set; }
		public Lineage.Channel Type { get; set; }
		public string Author { get; set; }
		public string Message { get; set; }
	}
}
