using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model.Memory
{
	sealed class Player
	{
		public int PlayerId;
		public string Name = String.Empty;
		public float Loyalty = 0.5f;
		public bool IsHuman = true;
	}
}
