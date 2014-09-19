using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	// Если onlyMy == false, то спойлит всех монстров, которые в данный момент атакуются кем-либо
	public sealed class AutoSpoil : Base
	{
		public AutoSpoil(Server.Game.Api gsApi)
			: base(gsApi)
		{
			throw new NotImplementedException();
		}

		public void Start() // bool onlyMy = true
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			throw new NotImplementedException();
		}

		private bool NeedSpoil = false;
		private bool Enabled = false;
		const int SKILL_SPOIL = 254;
	}
}
