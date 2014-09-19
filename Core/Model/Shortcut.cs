using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Shortcut
	{
		public enum type
		{
			//Unknown = 0,
			Item = 1,
			Skill = 2,
			Action = 3,
			Macro = 4,
			Recipe = 5
		}

		public type Type { get; set; }
		public int Number
		{
			get
			{
				return Slot + Page * ICON_COUNT;
			}
			set
			{
				Page = value / ICON_COUNT;
				Slot = value % ICON_COUNT;
			}
		}

		public int Slot { get; set; }
		public int Page { get; set; }

		public int DataId { get; set; }
		public int Level { get; set; }

		public const int ICON_COUNT = 12;
		public const int PAGE_COUNT = 10;
	}
}
