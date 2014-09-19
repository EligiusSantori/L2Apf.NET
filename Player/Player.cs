using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	sealed class Player
	{
		public Player(string file)
		{
			Logger = NLog.LogManager.GetCurrentClassLogger();
			Memory = new L2Apf.Memory(file);

			Api = new Server.Script.Api()
			{
				Config = new Model.LoginServer()
				{
					Address = Memory.Config.Address,
					Port = Memory.Config.Port,
					Protocol = Memory.Config.Protocol,
					Token = Memory.Config.Token
				}
			};

			Ai = new Ai._Blob(Api, Memory);
		}

		public void Main()
		{
			try
			{
				var servers = Api.Login(Memory.Config.Login, Memory.Config.Password);
				var characters = Api.SelectServer(servers.Single(s => s.Id == Memory.Config.ServerId));
				Api.SelectPlayer(characters.Single(p => p.Name == Memory.Config.Name));
			}
			catch (Exception e)
			{
				Logger.Error(string.Format("Player \"{0}\" {1}", Memory.Config.Name, e.Message));
			}

			Logger.Info(string.Format("Player \"{0}\" entered into the world", Memory.Config.Name));

			Ai.Pass();
		}


		private Server.Script.Api Api;
		private Memory Memory;
		private Ai._Blob Ai;

		private NLog.Logger Logger;
	}
}
