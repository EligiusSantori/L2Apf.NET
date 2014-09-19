using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Library
{
	public class Random
	{
		private Random()
		{

		}

		public static double GetDouble()
		{
			return Handle.NextDouble();
		}

		private static System.Random Handle = new System.Random();
	}
}
