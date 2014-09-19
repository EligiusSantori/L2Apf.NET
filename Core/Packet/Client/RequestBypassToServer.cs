using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestBypassToServer : Packet
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
			writer.WriteString(Command, Encoding);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x21; } }
		private int Size
		{
			get
			{
				return 1
					+ Encoding.GetByteCount(Command)
					+ Encoding.GetByteCount("\0");
			}
		}
		public string Command { get; set; }
	}
}
