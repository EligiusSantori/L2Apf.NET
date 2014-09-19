using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Region
	{
		public const float Size = 1000;

		/*public Region(DataReader)
		{

		}*/

		public static Library.Point FromPosition(Library.Point position)
		{
			return new Library.Point(
				(int)Math.Floor(position.X / Size),
				(int)Math.Floor(position.Y / Size),
				(int)Math.Floor(position.Z / Size)
			);
		}

		public static Library.Point ToPosition(Library.Point region)
		{
			return new Library.Point(
				region.X + Size,
				region.Y + Size,
				region.Z + Size
			);
		}

		public Library.Point Coords;
		public DateTime LastVisit;
	}
}
