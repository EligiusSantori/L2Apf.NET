using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Ai.Analyzer
{
	sealed class Factory
	{
		public Factory(Memory memory, Model.World world)
		{

		}

		// Как быть с тем, что не все анализаторы возвращают вес? Например большинство возвращает double, а анализатор направления - rads
			// Приводить анализатор к реальному классу по получению?
		/*public Model.Object this[int objectId]
		{
			get
			{

			}
		}*/
	}
}
