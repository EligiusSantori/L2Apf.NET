using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace L2Apf
{
	sealed class Realm
	{
		static void Main(string[] args)
		{
			NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.config");

			var list = new List<Tuple<Player, Thread>>();
			foreach (var file in args)
			{
				var player = new Player(file);
				var thread = new Thread(player.Main);
				list.Add(new Tuple<Player, Thread>(player, thread));
			}
			list.ForEach(t => t.Item2.Start());
		}
	}
}
