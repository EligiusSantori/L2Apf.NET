using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	// ToDo Manager<T> (enum, string, ...) или всё-таки присвоить идентификатор самим программам?
	public sealed class Manager
	{
		public void Load(Base program, string id)
		{
			Free(id);
			Map.Add(id, program);
			//program.Start();
		}

		public void Free(string id)
		{
			if (Map.ContainsKey(id))
			{
				Map[id].Dispose();
				Map.Remove(id);
			}
		}

		public Base Get(string id)
		{
			if (Map.ContainsKey(id))
				return Map[id];
			else
				return null;
		}

		private Dictionary<string, Base> Map = new Dictionary<string, Base>();
	}
}
