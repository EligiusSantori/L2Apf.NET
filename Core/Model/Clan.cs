using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Clan
	{
		public int Id { get; set; }
		public int Level { get; set; }
		public int CrestId { get; set; }
		public int LargetId { get; set; }
		public int AllyId { get; set; }
		public string Name { get; set; }
		public string Ally { get; set; }
		public string Leader { get; set; }
		public bool InWar { get; set; }
		public bool HasCastle { get; set; }
		public bool HasClanhall { get; set; }
		public bool IsAlly { get { return AllyId != 0; } }
		public Model.ClanMember[] Members { get; set; }
	}
}
