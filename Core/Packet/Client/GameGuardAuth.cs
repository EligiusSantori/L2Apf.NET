using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class GameGuardAuth : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(32);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(SessionId);
			writer.Write(GameGuard);

			ulong chk = Utils.CheckSum(ms.GetBuffer(), 24);
			writer.Write((byte)(chk & 0xff));
			writer.Write((byte)(chk >> 0x08 & 0xff));
			writer.Write((byte)(chk >> 0x10 & 0xff));
			writer.Write((byte)(chk >> 0x18 & 0xff));
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x07; } }
		public Int32 SessionId { get; set; }
		private byte[] GameGuard =
		{
			0x23, 0x92, 0x90, 0x4d,
			0x18, 0x30, 0xb5, 0x7c,
			0x96, 0x61, 0x41, 0x47,
			0x05, 0x07, 0x96, 0xfb,
			0x00, 0x00, 0x00
		};
	}
}
