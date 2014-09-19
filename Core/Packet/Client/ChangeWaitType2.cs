using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class ChangeWaitType2 : Packet
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
			writer.Write((Int32)State);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x1d; } }
		private int Size { get { return 5; } }
		public Lineage.WaitType State { get; set; }
	}
}
