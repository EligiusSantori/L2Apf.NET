using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class CharSelected : Packet
	{
		public struct Item
		{
			public Int32 ObjectId;
			public Int32 ItemId;
		};

		public override void Parse(byte[] buffer)
		{
			Reader r = new Reader(new MemoryStream(buffer));

			r.ReadByte(); // Id

			Name = r.ReadString(Encoding.Unicode);
			ObjectId = r.ReadInt32();
			Title = r.ReadString(Encoding.Unicode);
			SessionId = r.ReadInt32();
			ClanId = r.ReadInt32();

			r.ReadInt32(); // ?

			Gender = r.ReadInt32() == 0 ?
				Lineage.Gender.Male :
				Lineage.Gender.Female;
			Race = r.ReadInt32();
			BaseClassId = r.ReadInt32();
			IsActive = r.ReadInt32() != 0;

			Position = new Library.Point(
				r.ReadInt32(),
				r.ReadInt32(),
				r.ReadInt32());

			Hp = r.ReadDouble();
			Mp = r.ReadDouble();

			Sp = r.ReadInt32();
			Xp = r.ReadInt32();
			Level = r.ReadInt32();
			Karma = r.ReadInt32();

			r.ReadInt32(); // ?

			INT = r.ReadInt32();
			STR = r.ReadInt32();
			CON = r.ReadInt32();
			MEN = r.ReadInt32();
			DEX = r.ReadInt32();
			WIT = r.ReadInt32();

			r.ReadInt32(); // ?
			r.ReadInt32(); // ?

			Underwear.ObjectId = r.ReadInt32();
			RightEaring.ObjectId = r.ReadInt32();
			LeftEaring.ObjectId = r.ReadInt32();
			Neck.ObjectId = r.ReadInt32();
			RightFinger.ObjectId = r.ReadInt32();
			LeftFinger.ObjectId = r.ReadInt32();
			Head.ObjectId = r.ReadInt32();
			RightHand.ObjectId = r.ReadInt32();
			LeftHand.ObjectId = r.ReadInt32();
			Gloves.ObjectId = r.ReadInt32();
			Chest.ObjectId = r.ReadInt32();
			Legs.ObjectId = r.ReadInt32();
			Feet.ObjectId = r.ReadInt32();
			Back.ObjectId = r.ReadInt32();
			BothHand.ObjectId = r.ReadInt32();
			Hair.ObjectId = r.ReadInt32();

			Underwear.ItemId = r.ReadInt32();
			RightEaring.ItemId = r.ReadInt32();
			LeftEaring.ItemId = r.ReadInt32();
			Neck.ItemId = r.ReadInt32();
			RightFinger.ItemId = r.ReadInt32();
			LeftFinger.ItemId = r.ReadInt32();
			Head.ItemId = r.ReadInt32();
			RightHand.ItemId = r.ReadInt32();
			LeftHand.ItemId = r.ReadInt32();
			Gloves.ItemId = r.ReadInt32();
			Chest.ItemId = r.ReadInt32();
			Legs.ItemId = r.ReadInt32();
			Feet.ItemId = r.ReadInt32();
			Back.ItemId = r.ReadInt32();
			BothHand.ItemId = r.ReadInt32();
			Hair.ItemId = r.ReadInt32();

			// ToDo: other data
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x15; } }

		public string Name { get; set; }
		public string Title { get; set; }
		public Int32 ObjectId { get; set; }
		public Int32 SessionId { get; set; }
		public Library.Point Position { get; set; }
		public Lineage.Gender Gender { get; set; }

		public Int32 Race { get; set; }
		public Int32 BaseClassId { get; set; }

		public Double Hp { get; set; }
		public Double Mp { get; set; }

		public Int32 Sp { get; set; }
		public Int32 Xp { get; set; }
		public Int32 Level { get; set; }
		public Int32 Karma { get; set; }

		public Int32 INT { get; set; }
		public Int32 STR { get; set; }
		public Int32 CON { get; set; }
		public Int32 MEN { get; set; }
		public Int32 DEX { get; set; }
		public Int32 WIT { get; set; }

		public Item Underwear;
		public Item RightEaring;
		public Item LeftEaring;
		public Item Neck;
		public Item RightFinger;
		public Item LeftFinger;
		public Item Head;
		public Item RightHand;
		public Item LeftHand;
		public Item Gloves;
		public Item Chest;
		public Item Legs;
		public Item Feet;
		public Item Back;
		public Item BothHand;
		public Item Hair;

		public Int32 ClanId { get; set; }
		public bool IsActive { get; set; }
	}
}
