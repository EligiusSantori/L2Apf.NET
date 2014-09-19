using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class ValidateLocation : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			PositionX = reader.ReadInt32();
			PositionY = reader.ReadInt32();
			PositionZ = reader.ReadInt32();
			Heading = reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x61; } }
		public Int32 ObjectId { get; set; }
		public Int32 PositionX { get; set; }
		public Int32 PositionY { get; set; }
		public Int32 PositionZ { get; set; }
		public Int32 Heading { get; set; }
	}
}
