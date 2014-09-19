using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	public sealed class SkillChain : Base
	{
		public SkillChain(Server.Game.Api gsApi)
			: base(gsApi)
		{
			throw new NotImplementedException();

			//gsApi.SkillReused
			//gsApi.SkillLaunch
		}

		public void Load(IEnumerable<Model.Skill> skills)
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
