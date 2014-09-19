using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestReturnPoint : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(1);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			int point = 0;
			switch (Point)
			{
				case Lineage.ReturnPoint.Town: point = 0; break;
				case Lineage.ReturnPoint.Clanhall: point = 1; break;
				case Lineage.ReturnPoint.Castle: point = 2; break;
				case Lineage.ReturnPoint.SiegeHq: point = 3; break;
				case Lineage.ReturnPoint.Fixed: point = 4; break;
			}
			writer.Write((Int32)point);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x6d; } }
		public Lineage.ReturnPoint Point { get; set; }
	}
}
