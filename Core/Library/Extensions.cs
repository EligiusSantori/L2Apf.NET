using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	public static class Extensions
	{
		public static T ByMin<T>(this IEnumerable<T> source, Func<T, double> predicate)
		{
			T min = default(T);
			double value = double.MaxValue;
			foreach (var x in source)
			{
				var v = predicate(x);
				if (v < value)
				{
					value = v;
					min = x;
				}
			}
			if (value == double.MaxValue) // ToDo value? == null
			{
				throw new ArgumentException("Set is empty");
			}
			return min;
		}

		public static T ByMax<T>(this IEnumerable<T> source, Func<T, double> predicate)
		{
			T max = default(T);
			double value = double.MinValue;
			foreach (var x in source)
			{
				var v = predicate(x);
				if (v > value)
				{
					value = v;
					max = x;
				}
			}
			if (value == double.MinValue) // ToDo value? == null
			{
				throw new ArgumentException("Set must be non-empty");
			}
			return max;
		}

		/*public static T Random<T>(this IEnumerable<T> source)
		{
			var collection = source as ICollection<T>;
			int count = collection != null ? collection.Count : source.Count();

			if (count > 0)
			{
				int index = new Random().Next(0, count);

				int i = 0;
				T random = default(T);
				foreach (var e in source)
				{
					if (index == i)
					{
						random = e;
						break;
					}
					i++;
				}

				return random;
			}
			else
				throw new ArgumentException("Set must be non-empty");
		}*/
	}
}
