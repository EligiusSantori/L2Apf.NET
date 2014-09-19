using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class CharacterSelected : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(19);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(Number);
			writer.Write((Int32)0);
			writer.Write((Int32)0);
			writer.Write((Int32)0);
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}


		public override byte Id { get { return 0x0d; } }
		public Int16 Number { get; set; }
	}
}
