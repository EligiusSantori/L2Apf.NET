using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Npc
	{
		public int NpcId;
		public Lineage.Npc Type;
		public int GroupId;
		public bool IsAggro;
		public int Deaths;
		public int Kills;
	}
}
