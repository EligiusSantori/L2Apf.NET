using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class GameGuardReply : Packet
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
			writer.Write(new byte[16]
			{
				0x45,0x00,0x01,0x00,
				0x1E,0x37,0xA2,0xF5,
				0x00,0x00,0x00,0x00,
				0x00,0x00,0x00,0x00,
			});

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0xca; } }
		private int Size { get { return 17; } }
	}
}
