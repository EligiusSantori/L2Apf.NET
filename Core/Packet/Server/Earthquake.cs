using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class Earthquake : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Position = new Library.Point(
				reader.ReadInt32(),	
				reader.ReadInt32(),
				reader.ReadInt32()
			);
			Intensity = reader.ReadInt32();
			Duration = reader.ReadInt32();
			reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xc4; } }
		public Library.Point Position { get; set; }
		public Int32 Intensity { get; set; }
		public Int32 Duration { get; set; }
	}
}
