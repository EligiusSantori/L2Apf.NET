using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class InventoryUpdate : Packet
	{
		public struct Item
		{
			public Int32 ObjectId;
			public Int32 ItemId;
			public Int32 Count; // Quantity
			public Int32 Slot; // 0006-lr.ear, 0008-neck, 0030-lr.finger, 0040-head, 0100-l.hand, 0200-gloves, 0400-chest, 0800-pants, 1000-feet, 4000-r.hand, 8000-r.hand
			public Int16 Enchant; // Enchant level (pet level shown in control item)
			public int Change;

			public Int16 Type1; // Item Type 1 : 00-weapon/ring/earring/necklace, 01-armor/shield, 04-item/questitem/adena
			public Int16 Type2; // Item Type 2 : 00-weapon, 01-shield/armor, 02-ring/earring/necklace, 03-questitem, 04-adena, 05-item
			public Int16 Type3; // Filler (always 0)
			public Int16 Type4; // Pet name exists or not shown in control item

			public bool IsEquipped;
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Items = new Item[reader.ReadInt16()];
			for (int i = 0; i < Items.Length; i++)
				Items[i] = new Item()
				{
					Change = reader.ReadInt16(),
					Type1 = reader.ReadInt16(),
					ObjectId = reader.ReadInt32(),
					ItemId = reader.ReadInt32(),
					Count = reader.ReadInt32(),
					Type2 = reader.ReadInt16(),
					Type3 = reader.ReadInt16(),
					IsEquipped = reader.ReadInt16() != 0,
					Slot = reader.ReadInt32(),
					Enchant = reader.ReadInt16(),
					Type4 = reader.ReadInt16()
				};
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x27; } }
		public Item[] Items { get; set; }
	}
}
