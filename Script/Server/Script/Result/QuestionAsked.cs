using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Server.Script.Result
{
	public sealed class QuestionAsked : Result
	{
		public Lineage.Question Question;
		public string Name;
		public string Value;
	}
}
