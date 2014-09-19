using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestJoinParty : Packet
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
			writer.WriteString(Name, Encoding);
			writer.Write((Int32)Loot);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x29; } }
		private int Size
		{
			get
			{
				return 1 + 4
					+ Encoding.GetByteCount(Name)
					+ Encoding.GetByteCount("\0");
			}
		}
		public string Name { get; set; }
		public Lineage.PartyLoot Loot { get; set; }
	}
}
