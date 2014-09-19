using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class RestartResponse : Packet
	{
		public enum reason { Ok = 0x01 };

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Reason = (reason)reader.ReadInt32();
			Message = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}


		public override byte Id { get { return 0x5f; } }
		public string Message { get; set; }
		public reason Reason { get; set; }
	}
}
