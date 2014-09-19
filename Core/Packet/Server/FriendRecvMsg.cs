using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class FriendRecvMsg : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			reader.ReadInt32();
			Sender = reader.ReadString(Encoding.Unicode);
			Receiver = reader.ReadString(Encoding.Unicode);
			Message = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xfd; } }
		public string Sender { get; set; }
		public string Receiver { get; set; }
		public string Message { get; set; }
	}
}
