using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public abstract class Creature : Object
	{
		public double Distance(Model.Creature creature) // ToDo IPosition : Library.Point? Position
		{
			if (creature == this)
				return 0;
			else if (creature != null)
				return (new Library.Interval(Position, creature.Position)).Length;
			else
				throw new ArgumentNullException();
		}

		public double Distance(Model.Item item) // Todo IPosition
		{
			if (item != null)
				if (!item.InInventory)
					return (new Library.Interval(Position, item.Position.Value)).Length;
				else
					return 0;
			else
				throw new ArgumentNullException();
		}

		public string Name { get; set; }
		public string Title { get; set; }

		public int TargetId { get; set; } // Todo Nullable int, Todo replace to relation Model.Creature Target?
		public int Heading { get; set; }
		public int Level { get; set; }

		public int RunSpd { get; set; }
		public int WalkSpd { get; set; }
		public int SwimRunSpd { get; set; }
		public int SwimWalkSpd { get; set; }
		public int FlRunSpd { get; set; }
		public int FlWalkSpd { get; set; }
		public int FlyRunSpd { get; set; }
		public int FlyWalkSpd { get; set; }

		public double MoveSpdMult { get; set; }
		public double AtkSpdMult { get; set; }

		public double CollisionRadius { get; set; }
		public double CollisionHeight { get; set; }

		public int PAtkSpd { get; set; }
		public int MAtkSpd { get; set; }

		public double Hp { get; set; }
		public double Mp { get; set; }
		public double Cp { get; set; }
		public double MaxHp { get; set; }
		public double MaxMp { get; set; }
		public double MaxCp { get; set; }

		public Library.Point Position { get; set; }
		public Library.Point Destination { get; set; }

		public Casting Casting { get; set; }

		#region crap
			//public int Karma { get; set; } // npc karma? Wtf?
			public int NameColor { get; set; }

			public bool IsMoving { get; set; }
			public int MoveTarget { get; set; }
			public System.DateTime LastMoveTime { get; set; }

			public bool IsDied { get { return Hp <= 0; } } // Todo: Нужен ли этот параметр? Его достоверность вызывает сомнения.
			public bool IsInCombat { get; set; }
			public bool IsAlikeDead { get; set; } // Todo: isDead | isFakeDead
			//ToDo abstract bool IsInCombat
			//ToDo abstract bool IsAlikeDead
		#endregion

		/* // ToDo for timeout deletion from world map
		public System.DateTime lastVerifyTime{get;private set;} = System.DateTime.Now
		public void Verify() { lastVerifyTime = System.DateTime.Now; }*/
	}
}
