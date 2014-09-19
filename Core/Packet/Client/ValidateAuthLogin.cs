using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class ValidateAuthLogin : Packet
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
			writer.WriteString(Login, Encoding.Unicode);
			writer.Write(GameKey.Skip(4).ToArray());
			writer.Write(LoginKey.Take(4).ToArray());
			writer.Write(GameKey.Take(4).ToArray());
			writer.Write(LoginKey.Skip(4).ToArray());
			writer.Write((Int32)1);

			return ms.GetBuffer();
		}


		public override byte Id { get { return 0x08; } }
		private int Size { get { return 23 + (Login.Length + 1) * 2; } }

		public string Login { get; set; }
		public byte[] LoginKey { get; set; }
		public byte[] GameKey { get; set; }
	}
}
