using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class UseItem : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(9);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(ObjectId);
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x14; } }
		public Int32 ObjectId { get; set; }
	}
}
