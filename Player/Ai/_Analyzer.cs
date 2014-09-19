using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Ai
{
	sealed class _Analyzer
	{
		public _Analyzer(Memory memory, Model.World world)
		{
			World = world;
			Memory = memory;
		}

		public float FightDanger()
		{
			throw new NotImplementedException();
		}

		//public float EscapePoint() // Todo: Маршрут должен расчитываться в реальном времени, анализатора тут недостаточно


		public Model.World World;
		public Memory Memory;
	}
}
