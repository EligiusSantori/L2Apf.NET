using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class GetItem : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			PlayerId = reader.ReadInt32();
			ObjectId = reader.ReadInt32();
			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x0d; } }
		public Library.Point Position { get; set; }
		public Int32 PlayerId { get; set; }
		public Int32 ObjectId { get; set; }
	}
}
