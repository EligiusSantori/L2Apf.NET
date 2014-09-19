using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestDropItem : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(Size);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(ObjectId);
			writer.Write(Count);
			writer.Write((Int32)Point.X);
			writer.Write((Int32)Point.Y);
			writer.Write((Int32)Point.Z);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x12; } }
		private int Size { get { return 21; } }
		public Int32 ObjectId { get; set; }
		public Int32 Count { get; set; }
		public Library.Point Point { get; set; }
	}
}
