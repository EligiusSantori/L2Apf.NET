using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program.Move
{
	// ToDo Дополнительное событие OnFinish
	public sealed class SimpleRoute : Base
	{
		public SimpleRoute(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.FinishMoving += (Model.Creature creature) =>
			{
				if (gsApi.World.Me == creature && Queue != null)
					Next();
			};
		}

		public override void Dispose()
		{
			CallBack = null;
			Queue = null;
		}

		public void Load(IEnumerable<Library.Point> waypoints, Action callback = null)
		{
			Queue = new Queue<Library.Point>(waypoints);
			CallBack = callback;
			Next();
		}

		private void Next()
		{
			if (Queue.Count > 0)
				gsApi.MoveTo(Queue.Dequeue());
			else
			{
				if (CallBack != null)
					CallBack();
				Dispose();
			}
		}

		public int Remain { get { return Queue != null ? Queue.Count : 0; } }
		private Queue<Library.Point> Queue = null;
		private Action CallBack = null;
	}
}
