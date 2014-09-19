using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Npc : Creature
	{
		public int NpcId { get; set; }
		

		// ToDo override name to IDatabase.getNpcName()?

		public int BothHand { get; set; }
		public int LeftHand { get; set; }
		public int RightHand { get; set; }

		public bool IsAttackable { get; set; }
		public bool IsRunning { get; set; }
		public bool IsSummoned { get; set; }
		public bool IsSpoiled { get; set; }
		public bool IsShowName { get; set; }

		//AbnormalEffects
		//Team Circle
	}
}
