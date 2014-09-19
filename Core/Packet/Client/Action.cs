using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class Action : Packet
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
			writer.Write(TargetId);
			writer.Write((Int32)Origin.X);
			writer.Write((Int32)Origin.Y);
			writer.Write((Int32)Origin.Z);
			writer.Write((byte)(Shift ? 1 : 0));

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x04; } }
		private int Size { get { return 18; } }
		public Int32 TargetId { get; set; }
		public Library.Point Origin { get; set; }
		public bool Shift { get; set; }
	}
}
