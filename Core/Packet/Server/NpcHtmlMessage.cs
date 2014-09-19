using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class NpcHtmlMessage : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			MessageId = reader.ReadInt32();
			Content = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x0f; } }
		public Int32 MessageId { get; set; }
		public string Content { get; set; }
	}
}
