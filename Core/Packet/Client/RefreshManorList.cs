using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RefreshManorList : Packet //ToDo PacketEx
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
			writer.Write(SubId);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0xd0; } }
		public Int16 SubId { get { return 0x08; } }
		private int Size { get { return 3; } }
	}
}
