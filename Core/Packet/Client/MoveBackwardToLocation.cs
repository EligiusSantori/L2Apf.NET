using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class MoveBackwardToLocation : Packet
	{
		public enum device { Keyboard = 0x00, Mouse = 0x01 }

		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(Size);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write((Int32)Target.X);
			writer.Write((Int32)Target.Y);
			writer.Write((Int32)Target.Z);
			writer.Write((Int32)Origin.X);
			writer.Write((Int32)Origin.Y);
			writer.Write((Int32)Origin.Z);
			writer.Write((Int32)Device);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x01; } }
		private int Size { get { return 29; } }
		public Library.Point Target { get; set; }
		public Library.Point Origin { get; set; }
		public device Device { get; set; }
	}
}
