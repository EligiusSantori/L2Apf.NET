using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Battle
	{
		public int EnemyId;
		public Lineage.Genus Type;
		public sbyte Result; // 1 - win, 0 - not ended, -1 - lost
		public int DealDamage;
		public int GotDamage;
		public DateTime Start;
		public TimeSpan Length;
	}
}
