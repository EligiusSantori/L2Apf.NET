using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Nature
	{
		private static double Border(double scale)
		{
			if (scale < 0)
				return 0;
			else if (scale > 1)
				return 1;
			else return scale;
		}

		public double Intellect { get { return _Intellect; } set { _Intellect = Border(value); } }
		public double Courage { get { return _Courage; } set { _Courage = Border(value); } }
		public double Godliness { get { return _Godliness; } set { _Godliness = Border(value); } }
		public double Sociality { get { return _Sociality; } set { _Sociality = Border(value); } }
		public double Peaceness { get { return _Peaceness; } set { _Peaceness = Border(value); } }
		public double Curiosity { get { return _Curiosity; } set { _Curiosity = Border(value); } }
		public double Hunting { get { return _Hunting; } set { _Hunting = Border(value); } }
		public double Volatility { get { return _Volatility; } set { _Volatility = Border(value); } }
		public double Forgetness { get { return _Forgetness; } set { _Forgetness = Border(value); } }

		private double _Intellect;
		private double _Courage;
		private double _Godliness;
		private double _Sociality;
		private double _Peaceness;
		private double _Curiosity;
		private double _Hunting;
		private double _Volatility;
		private double _Forgetness;
	}
}
