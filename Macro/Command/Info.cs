using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Command
{
	public sealed class Info
	{
		public Info(Server.Game.Api gsApi, string[] args, string name)
		{
			Api = gsApi;
			Args = args;
			Name = name;
		}

		public void Run()
		{
			var name = Name;
			var args = Args;
			var gsApi = Api;
			var me = gsApi.World.Me;

			if (args.Length == 1)
				switch (args[0])
				{
					case "loc":
						var pos = me.Position;
						gsApi.Say(string.Format("{0}, {1}, {2}", Math.Round(pos.X), Math.Round(pos.Y), Math.Round(pos.Z)), name);
						break;
					case "level": // ToDo percentage
						gsApi.Say(string.Format("{0}", me.Level), name);
						break;
					case "sp":
						gsApi.Say(string.Format("{0}", me.Sp), name);
						break;
					case "hp":
						gsApi.Say(string.Format("{0} / {1} ({2}%)", Math.Round(me.Hp), Math.Round(me.MaxHp), Math.Round(100 / (me.MaxHp / me.Hp))), name);
						break;
					case "mp":
						gsApi.Say(string.Format("{0} / {1} ({2}%)", Math.Round(me.Mp), Math.Round(me.MaxMp), Math.Round(100 / (me.MaxMp / me.Mp))), name);
						break;
				}
		}

		private Server.Game.Api Api;
		private string[] Args;
		private string Name;
	}
}
