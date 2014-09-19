using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
    public abstract class Object
    {
        public int ObjectId { get; set; }

		public bool Equals(Model.Object obj)
		{
			return (obj as object) != null && this.ObjectId == obj.ObjectId;
		}

		public static bool operator ==(Model.Object a, Model.Object b)
		{
			return (a as object) == null && (b as object) == null || (a as object) != null && a.Equals(b);
		}

		public static bool operator !=(Model.Object a, Model.Object b)
		{
			return !(a == b);
		}
    }
}
