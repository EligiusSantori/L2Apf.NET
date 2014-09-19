using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace L2Apf
{
	sealed class Memory
	{
		public Memory(string file)
		{
			Attackers = new List<Model.Creature>();
			Cache = new MemoryCache("Player.Memory");
			Database = new Database(file);
		}

		public Model.Memory.Player GetPlayer(int playerId) // Todo cache
		{
			throw new NotImplementedException();
		}

		public Model.Memory.Player GetPlayer(string name) // Todo cache
		{
			throw new NotImplementedException();
		}

		public Model.Npc GetNpc(int npcId) // Todo cache
		{
			throw new NotImplementedException();
		}

		// public Model.Memory.Spawn GetSpawn(? area)
		// public Model.Memory.Spawn GetSpawn(int spawnId)

		public Model.Memory.Config Config
		{
			get
			{
				if (_Config == null)
					_Config = Database.GetConfig();
				return _Config;
			}
			private set
			{
				_Config = value;
			}
		}

		public Model.Memory.Nature Nature
		{
			get
			{
				if (_Nature == null)
					_Nature = Database.GetNatue();
				return _Nature;
			}
			private set
			{
				_Nature = value;
			}
		}

		public List<Model.Creature> Attackers { get; private set; }
		public MemoryCache Cache { get; private set; }

		private Model.Memory.Config _Config = null;
		private Model.Memory.Nature _Nature = null;
		private Database Database;
	}
}
