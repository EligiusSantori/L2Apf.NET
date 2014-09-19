using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program.Move
{
	public sealed class OnGraph : Base
	{
		public OnGraph(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.FinishMoving += (Model.Creature creature) =>
			{
				if (gsApi.World.Me == creature)
				{
					var current = Target;
					if (current.Nigh.Count > 1)
					{
						var variants = new List<Model.Graph.Node>();
						foreach(var node in current.Nigh)
							if(!node.Equals(Previous))
								variants.Add(node);
						var index = Random.Next(variants.Count);
						foreach (var node in variants)
							if (index-- == 0)
							{
								Target = node;
								break;
							}
					}
					else
						Target = current.Nigh.First();
					Previous = current;

					gsApi.MoveTo(Target.Point);
				}
			};
		}

		public void Play(Model.Graph graph)
		{
			if(IsMoving = (Target = graph.Near(gsApi.World.Me.Position)) != null)
				gsApi.MoveTo(Target.Point);
		}

		/*public void Pause()
		{
			throw new NotImplementedException();
		}*/

		public override void Dispose()
		{
			IsMoving = false;
		}

		//private Model.Graph NavigationGraph = null;
		private Model.Graph.Node Previous = null;
		private Model.Graph.Node Target = null;
		private bool IsMoving = false;
		private Model.Graph Graph;

		static Random Random = new Random();
	}
}
