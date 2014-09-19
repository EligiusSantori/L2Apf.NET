using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Server.Script.Result
{
	public sealed class ChatMessage : Result
	{
		public Lineage.Channel Channel;
		public string Message;
		public string Name;
		public Model.Creature Author;
	}
}
