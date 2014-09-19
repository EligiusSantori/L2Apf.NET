using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace L2Apf
{
	public sealed class FileRoute
	{
		public FileRoute(FileInfo file)
		{
			Route = new Stack<Library.Point>();
			Handle = file;
		}

		public void Load()
		{
			Route = new Stack<Library.Point>();
			if (Handle.Exists)
			{
				var stream = Handle.Open(FileMode.Open, FileAccess.Read);
				var reader = new StreamReader(stream);
				for(string line = null; (line = reader.ReadLine()) != null;)
				{
					string[] temp = line.Trim().Split(SEPARATOR.ToCharArray());
					double x, y, z;
					if (temp.Length == 3
							&& double.TryParse(temp[0].Trim(), out x)
							&& double.TryParse(temp[1].Trim(), out y)
							&& double.TryParse(temp[2].Trim(), out z))
						Route.Push(new Library.Point(x, y, z));
				}
				stream.Close();
			}
		}

		public void Save()
		{
			var stream = Handle.Open(FileMode.Create, FileAccess.Write);
			var writer = new StreamWriter(stream);
			foreach (var point in Route)
				writer.WriteLine(string.Join(SEPARATOR, new int[]
				{
					(int)Math.Round(point.X),
					(int)Math.Round(point.Y),
					(int)Math.Round(point.Z)					
				}));
			writer.Flush();
			stream.Close();
		}

		public void Add(Library.Point point)
		{
			Route.Push(point);
		}

		public void Del()
		{
			Route.Pop();
		}

		public int Count
		{
			get
			{
				return Route.Count;
			}
		}
		public double Length
		{
			get
			{
				double length = 0;
				Library.Point? last = null;
				foreach (var point in Route)
				{
					if (last.HasValue)
						length += new Library.Interval(last.Value, point).Length;
					last = point;
				}
				return length;
			}
		}
		public IEnumerable<Library.Point> Points
		{
			get
			{
				return Route;
			}
		}


		private Stack<Library.Point> Route;
		private FileInfo Handle;

		public const string SEPARATOR = " ";
	}
}
