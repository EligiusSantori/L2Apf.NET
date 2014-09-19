using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Library
{
	public class Interval
	{
		public double Length { get; private set; } // ToDo отложенное вычисление
		public Point Begin { get; private set; }
		public Point End { get; private set; }


		public Interval(Point begin, Point end)
		{
			this.Length = Distance(begin, end);
			this.Begin = begin;
			this.End = end;
		}

		public Point Point(double position)
		{
			double ratio = Length / position;
			return new Point(
						 (Begin.X - End.X) / ratio,
						 (Begin.Y - End.Y) / ratio,
						 (Begin.Z - End.Z) / ratio);
		}

		/// <summary>
		/// Разделить отрезок на N равных частей
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Point[] Split(int count)
		{
			if (count > 1)
			{
				var waypoints = new Library.Point[count];
				for (int i = 0; i < count; i++)
					waypoints[i] = new Library.Point(
						Begin.X + (End.X - Begin.X) / (count - 1) * i,
						Begin.Y + (End.Y - Begin.Y) / (count - 1) * i,
						Begin.Z + (End.Z - Begin.Z) / (count - 1) * i);
				return waypoints;
			}
			else
				throw new ArgumentOutOfRangeException();
		}

		public static double Distance(Point Begin, Point End)
		{
			// Not Math.Pow! It's infinity longer.
			double dx = Begin.X - End.X;
			double dy = Begin.Y - End.Y;
			double dz = Begin.Z - End.Z;
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}
	}
}
