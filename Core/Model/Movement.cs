using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Apf.Library;

namespace L2Apf.Model
{
	/// <summary>
	/// Linear Movement
	/// </summary>
	public sealed class Movement
	{
		public Movement(Point departure, Point destination, double speed)
		{
			Interval = new Interval(departure, destination);
			Start = DateTime.Now;
			Speed = speed;
		}

		public Point Current
		{
			get
			{
				double distance = Distance;
				double position = Position;

				if (position < distance)
					return Interval.Point(position);
				else
					return Destination;
			}
		}

		private Point Departure
		{
			get
			{
				return Interval.Begin;
			}
		}

		private Point Destination
		{
			get
			{
				return Interval.End;
			}
		}

		public double Position
		{
			get
			{
				return (DateTime.Now - Start).Milliseconds / 1000d * Speed;
			}
		}

		public double Distance
		{
			get
			{
				return Interval.Length;
			}
		}


		public double Speed { get; private set; }
		private Interval Interval;
		private DateTime Start;
	}
}
