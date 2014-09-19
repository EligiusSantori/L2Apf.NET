using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Ai.Analyzer
{
	abstract class Base
	{
		public static float Weigh(float[] f, float[] c = null)
		{
			if (c != null)
				return f.Select((fn, i) => fn * c[i]).Sum() / c.Sum();
			else
				return f.Sum() / f.Count();
		}

		public static bool Is(float weight)
		{
			return Library.Random.GetDouble() < weight;
		}
	}
}
