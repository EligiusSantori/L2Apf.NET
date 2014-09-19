using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program.Move
{
	public sealed class FullFollow : Base
	{
		public FullFollow(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.StartMoving += (Model.Creature creature) =>
			{
				if (creature == Leader)
				{
					if (!gsApi.World.Me.IsMoving) // Бот обошёл все точки и стоит рядом с персонажем
					{
						Last = creature.Destination;
						Next();
					}
					else if (Queue.Count == 0 && !Last.HasValue) // Бот бежит к месту назначения персонажа
					{
						Queue.Enqueue(creature.Position);
						Last = creature.Destination;
						Next();
					}
					else // Бот ещё обходит очередь
					{
						Queue.Enqueue(creature.Position);
						Last = creature.Destination;
					}
				}
			};
			gsApi.FinishMoving += (Model.Creature creature) =>
			{
				if (creature == gsApi.World.Me)
					Next();
				else if (creature == Leader)
				{
					Queue.Enqueue(creature.Position);
					if (!gsApi.World.Me.IsMoving)
						Next();
				}
			};
		}

		public void Bind(Model.Creature creature)
		{
			Leader = creature;
			Queue.Enqueue(creature.Position);
			if (creature.Position != creature.Destination)
				Last = creature.Destination;
			Next();
		}

		public override void Dispose()
		{
			Leader = null;
		}

		private void Next()
		{
			Library.Point? point = null;
			if (Queue.Count > 0)
				point = Queue.Dequeue();
			else if (Last.HasValue)
			{
				point = Last.Value;
				Last = null;
			}

			if (point.HasValue && point.Value != gsApi.World.Me.Position)
				gsApi.MoveTo(point.Value);
			else if (Queue.Count > 0 || Last.HasValue)
				Next();
		}

		public Model.Creature Leader { get; private set; }
		private Queue<Library.Point> Queue = new Queue<Library.Point>();
		private Library.Point? Last = null;
	}
}
