using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	public sealed class LoginFailException : SyncTargetException
	{
		public LoginFailException(Server.Script.Result.LoginFail result)
			: base(result)
		{
			Reason = result.Reason;
			// Todo: set message
		}

		public Packet.Server.LoginFail.ReasonType Reason { get; private set; }
	}
}
