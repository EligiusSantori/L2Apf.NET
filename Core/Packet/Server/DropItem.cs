using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class DropItem : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			DroppedBy = reader.ReadInt32();
			ObjectId = reader.ReadInt32();
			ItemId = reader.ReadInt32();
			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);
			IsStackable = reader.ReadInt32() != 0;
			Count = reader.ReadInt32();
			reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x0c; } }
		public Library.Point Position { get; set; }
		public Int32 DroppedBy { get; set; }
		public Int32 ObjectId { get; set; }
		public Int32 ItemId { get; set; }
		public Int32 Count { get; set; }
		public bool IsStackable { get; set; }
	}
}
