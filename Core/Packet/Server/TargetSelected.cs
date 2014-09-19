using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class TargetSelected : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			TargetId = reader.ReadInt32();
			X = reader.ReadInt32();
			Y = reader.ReadInt32();
			X = reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x29; } }
		public Int32 ObjectId { get; set; }
		public Int32 TargetId { get; set; }
		public Int32 X { get; set; }
		public Int32 Y { get; set; }
		public Int32 Z { get; set; }
	}
}
