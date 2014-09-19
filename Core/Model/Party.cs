using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Party
	{
		public Model.Character Leader = null;
		public List<Model.Character> Members = new List<Model.Character>();
		public Lineage.PartyLoot Loot;
	}
}
