using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model
{
	public sealed class Travel
	{
		public Travel(Library.Point position, Library.Point destination)
		{
			var interval = new Library.Interval(position, destination);
			double distance = interval.Length;
			int count = (int)Math.Ceiling(distance / INTERVAL) + 1;
			if (count > 1)
				Waypoints = interval.Split(count);
			else
				Waypoints = new Library.Point[] { };
		}

		public Library.Point[] Waypoints { get; private set; }
		const int INTERVAL = 1000;
	}
}
