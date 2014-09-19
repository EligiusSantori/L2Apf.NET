using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	// ToDo split to Model.Skill | UserSkill ?
	// Skill may be a switch type or passive, remove excess methods?
	public sealed class Skill
	{
		public Skill()
		{
			IsReady = true;
		}

		public int SkillId { get; set; }
		public int Level { get; set; }
		public bool IsActive { get; set; } // Todo active, passive, toggle
		public bool IsReady { get; set; }
		//public bool IsCast { get; set; }


		public bool Equals(Model.Skill skill)
		{
			return (skill as object) != null && this.SkillId == skill.SkillId;
		}

		public static bool operator ==(Model.Skill a, Model.Skill b)
		{
			return (a as object) == null && (b as object) == null || (a as object) != null && a.Equals(b);
		}

		public static bool operator !=(Model.Skill a, Model.Skill b)
		{
			return !(a == b);
		}
		
		public System.Timers.Timer Reuse { get; set; }
	}
}
