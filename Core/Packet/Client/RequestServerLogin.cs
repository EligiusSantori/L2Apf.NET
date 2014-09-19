using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestServerLogin : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(24);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(LoginKey);
			writer.Write((byte)ServerId);
			writer.Write((Int16)0);
			writer.Write((Int32)0);

			ulong chk = Utils.CheckSum(ms.GetBuffer(), 16);
			writer.Write((byte)(chk & 0xff));
			writer.Write((byte)(chk >> 0x08 & 0xff));
			writer.Write((byte)(chk >> 0x10 & 0xff));
			writer.Write((byte)(chk >> 0x18 & 0xff));
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x02; } }
		public byte ServerId { get; set; }
		public byte[] LoginKey { get; set; }
	}
}
