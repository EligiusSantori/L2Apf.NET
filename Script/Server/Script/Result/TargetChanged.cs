using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Server.Script.Result
{
	public sealed class TargetChanged : Result
	{
		public Model.Creature Creature;
		public Model.Creature Target;
	}
}
