using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class LoginServer
	{
		public LoginServer()
		{
			Address = "127.0.0.1";
			Port = 2106;
			Protocol = 660;
			Token = new byte[]
			{
				0x5f, 0x3b, 0x35, 0x2e,
				0x5d, 0x39, 0x34, 0x2d,
				0x33, 0x31, 0x3d, 0x3d,
				0x2d, 0x25, 0x78, 0x54,
				0x21, 0x5e, 0x5b, 0x24,
				0x00
			};
		}

		public string Address { get; set; } // ToDo IPv4 class
		public int Port { get; set; }
		public int Protocol { get; set; }
		public byte[] Token { get; set; }
	}
}
