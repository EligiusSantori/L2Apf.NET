using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Player : Character // ToDo Rename to Me
	{
		public int STR { get; set; }
		public int DEX { get; set; }
		public int CON { get; set; }
		public int INT { get; set; }
		public int WIT { get; set; }
		public int MEN { get; set; }

		public Lineage.State State { get; set; }
		//public isMoveBlocked
		//public isMagicBlocked
		//public isAttackBlocked

		public new Equipment Equipment;
		public int[] Symbols { get; set; }

		public int InventoryLimit { get; set; } // ToDo Inventory.Limit

		public int Number { get; set; }
		public string Login { get; set; }
		public int SessionId { get; set; }

		public bool IsActive { get; set; }
		public bool IsLastUsed { get; set; }
		public int AccessLevel { get; set; }
		public int ToRemove { get; set; } // ToDo timespan

		public Dictionary<int, Model.Skill> Skills = new Dictionary<int, Skill>();
	}
}
