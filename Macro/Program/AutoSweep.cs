using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	public sealed class AutoSweep : Base
	{
		// ToDo: List
		public AutoSweep(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.Die += (Model.Creature creature, Lineage.ReturnPoint? points) =>
			{
				Model.Npc npc = creature as Model.Npc;
				if (Enabled && npc != null && npc.IsSpoiled)
				{
					bool isMyTarget = gsApi.World.Me.TargetId == npc.ObjectId;
					if (isMyTarget)
						gsApi.UseSkill(SKILL_SWEEP);
					else if (!OnlyMy)
						gsApi.Target(npc); // ToDo: Запоминание цели/состояния и возврат к нему
				}
			};

			gsApi.TargetChanged += (Model.Creature creature, Model.Creature target) =>
			{
				if (Enabled && creature == gsApi.World.Me)
				{
					Model.Npc npc = target as Model.Npc;
					if (npc != null && npc.IsAlikeDead && npc.IsSpoiled)
						gsApi.UseSkill(SKILL_SWEEP);
				}
			};
		}

		public void Start(bool onlyMy = true)
		{
			OnlyMy = onlyMy;
			Enabled = true;
		}

		public override void Dispose()
		{
			Enabled = false;
		}

		public bool OnlyMy { get; set; }
		const int SKILL_SWEEP = 42;
	}
}
