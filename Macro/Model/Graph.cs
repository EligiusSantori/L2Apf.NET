using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model
{
	public sealed class Graph
	{
		public sealed class Node
		{
			public Library.Point Point;
			public List<Node> Nigh;
		}

		public Graph(List<List<Library.Point>> parts)
		{
			var forks = new List<Node>();
			var nodes = new List<Node>();

			foreach (var part in parts)
			{
				int i = 0;
				Node last = null;
				var temp = new List<Node>();
				foreach (var point in part)
				{
					Node node = null;

					if (i == 0 || i == part.Count - 1)
					{
						// Ищем развилку среди уже известных, либо создаём новую
						if ((node = forks.Find(n => n.Point.Equals(point))) == null)
							forks.Add(node = new Node()
							{
								Point = point,
								Nigh = new List<Node>()
							});
						
						if(i != 0) // Для начальной развилки добавим во втором проходе
							node.Nigh.Add(last);
					}
					else
					{
						node = new Node()
						{
							Point = point,
							Nigh = new List<Node>() { last }
						};
					}

					temp.Add(node);
					last = node;
					i++;
				}

				i = 0;
				last = null;
				temp.Reverse();
				foreach (var node in temp)
				{
					if (i == part.Count - 1)
					{
						node.Nigh.Add(last);
					}
					else if (i != 0)
					{
						node.Nigh.Add(last);
						nodes.Add(node);
					}

					last = node;
					i++;
				}
			}

			nodes.AddRange(forks);
			Nodes = nodes;
		}

		public Node Near(Library.Point point)
		{
			return Nodes.Count > 0 ? Nodes.ByMin(node => Library.Interval.Distance(node.Point, point)) : null;
		}

		public Node Find(Library.Point point)
		{
			return Nodes.Find(node => node.Point.Equals(point));
		}

		private List<Node> Nodes;
	}
}
