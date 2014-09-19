using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	//open private craft shop = 25 0 0 0
	//open general craft shop = 33 0 0 0
	public sealed class RequestActionUse : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(10);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write((Int32)Action);
			writer.Write((Int32)(Control ? 1 : 0));
			writer.Write((byte)(Shift ? 1 : 0));

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x45; } }
		public Lineage.Action Action { get; set; }
		public bool Control { get; set; }
		public bool Shift { get; set; }
	}
}
