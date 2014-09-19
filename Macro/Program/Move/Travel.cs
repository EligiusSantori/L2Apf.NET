using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program.Move
{
	public sealed class Travel : Base
	{
		public Travel(Server.Game.Api gsApi)
			: base(gsApi)
		{
			Program = new SimpleRoute(gsApi);
		}

		public void Start(Library.Point target)
		{
			Model = new Model.Travel(gsApi.World.Me.Position, target);
			Program.Load(Model.Waypoints.Skip(1));
		}

		public override void Dispose()
		{
			Model = null;
			Program.Dispose();
		}

		public Model.Travel Model { get; private set; }
		private SimpleRoute Program = null;
	}
}
