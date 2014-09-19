using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program.Move
{
	public sealed class FastFollow : Base
	{
		public FastFollow(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.StartMoving += Catch;
			gsApi.FinishMoving += Catch;
		}

		public override void Dispose()
		{
			this.Leader = null;
		}

		public void Bind(Model.Creature creature)
		{
			this.Leader = creature;
			Catch(creature);
		}

		private void Catch(Model.Creature creature)
		{
			if(creature == Leader)
					gsApi.MoveTo(Leader.IsMoving ? Leader.Destination : Leader.Position);
		}

		public Model.Creature Leader { get; private set; }
	}
}
