using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	// ToDo
	// abstract Base.Start();
	// abstract Base.Stop();
	// new Concrete(gsApi, args)
	// new Concrete(gsApi) { args = args }
	public abstract class Base : IDisposable
	{
		public Base(Server.Game.Api gsApi)
		{
			if (gsApi != null)
				this.gsApi = gsApi;
			else
				throw new ArgumentNullException();
		}

		public abstract void Dispose();

		protected Server.Game.Api gsApi { get; set; }
		public bool Enabled { get; protected set; }
	}
}
