using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Model
{
	public sealed class Access
	{
		public bool LogIn(string name, string password)
		{
			if (!string.IsNullOrEmpty(Password) && password == Password && !Owners.Contains(name))
			{
				Owners.Add(name);
				return true;
			}
			else
				return false;
		}

		public bool Allow(string name)
		{
			return IsFree || Owners.Contains(name);
		}

		public bool IsFree = false;
		public string Password = string.Empty;
		public List<string> Owners = new List<string>();
	}
}
