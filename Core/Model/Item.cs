using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	public sealed class Item : Model.Object //ToDo place enum?
	{
		public int ItemId { get; set; }
		public int Count { get; set; }
		public int Slot { get; set; }
		public short Enchant { get; set; }

		public bool IsEquipped { get; set; }
		public bool IsStackable { get; set; }
		public bool InInventory { get { return !Position.HasValue; } }

		public int DroppedBy { get; set; }
		public Library.Point? Position { get; set; }

		public short Type1 { get; set; }
		public short Type2 { get; set; }
		public short Type3 { get; set; }
		public short Type4 { get; set; }
	}
}
