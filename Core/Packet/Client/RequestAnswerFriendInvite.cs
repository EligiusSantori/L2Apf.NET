using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestAnswerFriendInvite : Packet
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
			writer.Write((Int32)Answer);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0x5f; } }
		private int Size { get { return 5; } }
		public Lineage.Answer Answer { get; set; }
	}
}
