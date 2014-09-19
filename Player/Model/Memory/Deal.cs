using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Deal
	{
		public int DealId;
		public int ItemId;
		public int Count;
		public int Cost;
		public Lineage.Trade Type;
		public DateTime Time;
	}
}
