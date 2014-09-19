using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class AllyCrest : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			CrestId = reader.ReadInt32();
			Content = reader.ReadBytes(reader.ReadInt32());
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xae; } }
		public Int32 CrestId { get; set; }
		public byte[] Content { get; set; }
	}
}
