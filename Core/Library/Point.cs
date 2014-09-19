using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Library
{
 // Важно определение структуры! Это позволяет копировать точку присваиванием (Obj1.Dest = Obj2.Pos);
 public struct Point
 {
		public Point(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/*public override bool Equals(object point)
		{
			if (point is Point)
			{
				Point p = (Point)point;
				return X == p.X && Y == p.Y && Z == p.Z;
			}
			else return false;
		}*/

		public static bool operator ==(Point a, Point b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Point a, Point b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", X, Y, Z);
		}

		public readonly double X;
		public readonly double Y;
		public readonly double Z;
 }
}
