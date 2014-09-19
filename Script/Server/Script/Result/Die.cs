using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Server.Script.Result
{
	public sealed class Die : Result
	{
		public Model.Creature Creature;
		public Lineage.ReturnPoint? Points;
	}
}
