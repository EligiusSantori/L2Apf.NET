using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class LoginFail : Packet
	{
		public enum ReasonType
		{
			SystemError = 0x01,
			PasswordWrong = 0x02,
			AuthPairWrong = 0x03,
			AccessFailed = 0x04,
			AccountInUse = 0x07,
			AccountBanned = 0x09
		}

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

		public override byte Id { get { return 0x01; } }
		public ReasonType Reason { get; set; }
	}
}
