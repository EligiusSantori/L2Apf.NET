using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class ItemList : Packet
	{
		public struct Item
		{
			public Int32 ObjectId;
			public Int32 ItemId;
			public Int32 Count;
			public Int32 Slot;
			public Int16 Enchant;

			public Int16 Type1;
			public Int16 Type2;
			public Int16 Type3;
			public Int16 Type4;
			
			public bool IsEquipped;
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			OpenWindow = reader.ReadInt16() != 0;
			Items = new Item[reader.ReadInt16()];
			for(int i = 0; i < Items.Length; i++)
				Items[i] = new Item()
				{
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

		public override byte Id { get { return 0x1b; } }
		public bool OpenWindow { get; set; }
		public Item[] Items { get; set; }
	}
}
