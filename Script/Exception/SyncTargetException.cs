using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	public class SyncTargetException : Exception
	{
		public SyncTargetException(Server.Script.Result.Result result)
		{
			Result = result;
		}

		public Server.Script.Result.Result Result { get; protected set; }
	}
}
