using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*0: Id
1: Title
2: Address
3: Port
4: Age
5: PvP
6: Online
7: Maximum
8: State
9: Poop*/

namespace L2Apf.Model
{
	public sealed class GameServer
	{
		public GameServer()
		{
			Id = 0;
			//Title = null;
			Address = "127.0.0.1";
			Port = 7777;
			Age = 0;
			PvP = false;
			Online = 0;
			Maximum = 0;
			State = false;
			Poop = null;
		}

		public int Id { get; set; }
		//public string Title { get; set; }
		public string Address { get; set; } // ToDo: IPv4 class
		public int Port { get; set; }
		public int Age { get; set; }
		public bool PvP { get; set; }
		public int Online { get; set; }
		public int Maximum { get; set; }

		public bool State { get; set; }
		public string Poop { get; set; }

	}
}
