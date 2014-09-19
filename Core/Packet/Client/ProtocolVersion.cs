using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class ProtocolVersion : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(5); // + Raw.length
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(Protocol);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x00; } }
		public Int32 Protocol { get; set; }
	}
}
