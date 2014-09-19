using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class PlayFail : Packet
	{
		public enum ReasonType
		{
			SystemError = 0x01,
			TooManyPlayers = 0x0f
		};

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Reason = (ReasonType)reader.ReadByte();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x06; } }
		public ReasonType Reason { get; set; }
	}
}
