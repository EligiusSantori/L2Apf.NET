using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Config
	{
		public string Login;
		public string Password;
		public string Name;
		public string Address = "127.0.0.1";
		public int Port = 2106;
		public int Protocol = 660;
		public byte[] Token;
		public int ServerId;
	}
}
