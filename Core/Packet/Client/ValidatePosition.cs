using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class ValidatePosition : Packet
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
			writer.Write((Int32)Position.X);
			writer.Write((Int32)Position.Y);
			writer.Write((Int32)Position.Z);
			writer.Write(Angle);
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x48; } }
		private int Size { get { return 21; } }
		public Library.Point Position { get; set; }
		public Int32 Angle { get; set; }
	}
}
