using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	public sealed class PlayFailException : SyncTargetException
	{
		public PlayFailException(Server.Script.Result.PlayFail result)
			: base(result)
		{
			Reason = result.Reason;
			// Todo: set message
		}

		public Packet.Server.PlayFail.ReasonType Reason { get; private set; }
	}
}
