using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class ClanMember : Object
	{
		public string Name { get; set; }
		public int Level { get; set; }
		public int ClassId { get; set; }
		public bool IsOnline { get; set; }
	}
}
