using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestAuthLogin : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream temp = new MemoryStream(128);
			Writer writer = new Writer(temp);

			writer.Seek(0x62, SeekOrigin.Begin);
			writer.WriteString(Login, Encoding.ASCII);
			writer.Seek(0x70, SeekOrigin.Begin);
			writer.WriteString(Password, Encoding.ASCII);


			MemoryStream ms = new MemoryStream(168);
			writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(Encrypt(temp.GetBuffer()));
			writer.Write(SessionId);
			writer.Write(GameGuard);

			ulong chk = Utils.CheckSum(ms.GetBuffer(), 160);
			writer.Write((byte)(chk & 0xff));
			writer.Write((byte)(chk >> 0x08 & 0xff));
			writer.Write((byte)(chk >> 0x10 & 0xff));
			writer.Write((byte)(chk >> 0x18 & 0xff));
			writer.Write((Int32)0);

			return ms.GetBuffer();
		}

		private byte[] Encrypt(byte[] buffer)
		{
			var RSAKeyInfo = new System.Security.Cryptography.RSAParameters();
			RSAKeyInfo.Modulus = EncKey;
			RSAKeyInfo.Exponent = Exponent;
			var rsa = new Mono.Security.Cryptography.RSAManaged();
			rsa.ImportParameters(RSAKeyInfo);
			return rsa.EncryptValue(buffer);
		}


		public const int MAX_LOGIN_SIZE = 14 - 1;
		public const int MAX_PASSWORD_SIZE = 16 - 1;

		public override byte Id { get { return 0x00; } }
		public Int32 SessionId { get; set; }
		public byte[] EncKey { get; set; }
		private byte[] Exponent = { 1, 0, 1 };
		private string login;
		private string password;
		public string Login
		{
			get
			{
				return login;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > MAX_LOGIN_SIZE)
					throw new ArgumentOutOfRangeException("username is too long");
				else
					login = value;
			}
		}
		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > MAX_PASSWORD_SIZE)
					throw new ArgumentOutOfRangeException("password is too long");
				else
					password = value;
			}
		}

		private byte[] GameGuard =
		{
			0x23, 0x92, 0x90, 0x4D,
			0x18, 0x30, 0xB5, 0x7C,
			0x96, 0x61, 0x41, 0x47,
			0x05, 0x07, 0x96, 0xFB,
			0x08, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00
		};
	}
}
