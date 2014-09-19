using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Command
{
	// Base Command Handler
	public abstract class Base
	{
		public Base(string command, string args, Model.Creature author, Server.Game.Api gsApi) // ?
		{
			throw new NotImplementedException();
		}

		public abstract void Run();
	}
}
