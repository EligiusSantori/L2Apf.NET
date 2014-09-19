using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class UserInfo : Packet
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

			Position = new Library.Point(
				r.ReadInt32(),
				r.ReadInt32(),
				r.ReadInt32());

			Heading = r.ReadInt32();
			ObjectId = r.ReadInt32();

			Name = r.ReadString(Encoding.Unicode);
			Race = r.ReadInt32();
			Gender = r.ReadInt32() == 0 ?
				Lineage.Gender.Male :
				Lineage.Gender.Female;
			BaseClassId = r.ReadInt32();
			Level = r.ReadInt32();
			Xp = r.ReadInt32();

			STR = r.ReadInt32();
			DEX = r.ReadInt32();
			CON = r.ReadInt32();
			INT = r.ReadInt32();
			WIT = r.ReadInt32();
			MEN = r.ReadInt32();

			MaxHp = r.ReadInt32();
			Hp = r.ReadInt32();
			MaxMp = r.ReadInt32();
			Mp = r.ReadInt32();

			Sp = r.ReadInt32();
			Load = r.ReadInt32();
			MaxLoad = r.ReadInt32();

			r.ReadInt32(); // ? 0x28

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

			PAtk = r.ReadInt32();
			PAtkSpd = r.ReadInt32(); // ToDo ???
			PDef = r.ReadInt32();
			Evasion = r.ReadInt32();
			Accuracy = r.ReadInt32();
			Focus = r.ReadInt32();
			MAtk = r.ReadInt32();
			MAtkSpd = r.ReadInt32();
			PAtkSpd = r.ReadInt32(); // ToDo ???
			MDef = r.ReadInt32();
			InPvP = r.ReadInt32() != 0;
			Karma = r.ReadInt32();

			RunSpd = r.ReadInt32();
			WalkSpd = r.ReadInt32();
			SwimRunSpd = r.ReadInt32();
			SwimWalkSpd = r.ReadInt32();
			FlRunSpd = r.ReadInt32();
			FlWalkSpd = r.ReadInt32();
			FlyRunSpd = r.ReadInt32();
			FlyWalkSpd = r.ReadInt32();

			MoveSpdMult = r.ReadDouble();
			AtkSpdMult = r.ReadDouble();
			CollisionRadius = r.ReadDouble();
			CollisionHeight = r.ReadDouble();

			HairStyle = r.ReadInt32();
			HairColor = r.ReadInt32();
			FaceType = r.ReadInt32();

			AccessLevel = r.ReadInt32();
			Title = r.ReadString(Encoding.Unicode);

			ClanId = r.ReadInt32();
			ClanCrestId = r.ReadInt32();
			AllyId = r.ReadInt32();
			AllyCrestId = r.ReadInt32();
			IsClanLeader = r.ReadInt32() != 0;
			MountType = (Lineage.MountType)r.ReadByte();
			PrivateStore = (Lineage.PrivateStore)r.ReadByte();
			HasDwarfCraft = r.ReadByte();
			Pk = r.ReadInt32();
			PvP = r.ReadInt32();

			int count = r.ReadInt16();
			Cubics = new List<Int32>(count);
			for (int i = 0; i < count; i++)
				Cubics.Add(r.ReadInt16());

			IsFindParty = r.ReadByte() != 0;
			AbnormalEffects = r.ReadInt32();
			r.ReadByte(); // ?
			r.ReadInt32(); // ClanPrivileges
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			r.ReadInt32(); // ?
			
			RecLeft = r.ReadInt16();
			RecAmount = r.ReadInt16();
			r.ReadInt32(); // ?
			InventoryLimit = r.ReadInt16();
			ClassId = r.ReadInt32();
			SpecialEffects = r.ReadInt32();
			MaxCp = r.ReadInt32();
			Cp = r.ReadInt32();
			Enchant = r.ReadByte();
			TeamCircle = r.ReadByte();
			LargeCrestId = r.ReadInt32();

			IsHeroIcon = r.ReadByte() != 0;
			IsHeroGlow = r.ReadByte() != 0;

			IsFishing = r.ReadByte() != 0;
			Fish = new Library.Point(
				r.ReadInt32(),
				r.ReadInt32(),
				r.ReadInt32());

			NameColor = r.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x04; } }

		public string Name { get; set; }
		public string Title { get; set; }
		public Int32 Heading { get; set; }
		public Int32 ObjectId { get; set; }
		public Int32 NameColor { get; set; }

		public Int32 Race { get; set; }
		public Int32 ClassId { get; set; }
		public Int32 BaseClassId { get; set; }
		public Lineage.Gender Gender { get; set; }
		public Lineage.MountType MountType { get; set; }
		public Lineage.PrivateStore PrivateStore { get; set; }

		public Int32 ClanId { get; set; }
		public Int32 ClanCrestId { get; set; }
		public Int32 AllyId { get; set; }
		public Int32 AllyCrestId { get; set; }
		public Int32 LargeCrestId { get; set; }

		public Double MoveSpdMult { get; set; }
		public Double AtkSpdMult { get; set; }
		public Double CollisionRadius { get; set; }
		public Double CollisionHeight { get; set; }

		public Int32 HairStyle { get; set; }
		public Int32 HairColor { get; set; }
		public Int32 FaceType { get; set; }

		public Int32 Level { get; set; }
		public Int32 MaxHp { get; set; }
		public Int32 Hp { get; set; }
		public Int32 MaxMp { get; set; }
		public Int32 Mp { get; set; }
		public Int32 MaxCp { get; set; }
		public Int32 Cp { get; set; }
		public Int32 Xp { get; set; }
		public Int32 Sp { get; set; }
		public Int32 Load { get; set; }
		public Int32 MaxLoad { get; set; }
		public Int32 PvP { get; set; }
		public Int32 Pk { get; set; }

		public Int32 PAtk { get; set; }
		public Int32 PAtkSpd { get; set; }
		public Int32 PDef { get; set; }
		public Int32 Evasion { get; set; }
		public Int32 Accuracy { get; set; }
		public Int32 Focus { get; set; }
		public Int32 MAtk { get; set; }
		public Int32 MAtkSpd { get; set; }
		public Int32 MDef { get; set; }
		public Int32 Karma { get; set; }

		public Int32 RunSpd { get; set; }
		public Int32 WalkSpd { get; set; }
		public Int32 SwimRunSpd { get; set; }
		public Int32 SwimWalkSpd { get; set; }
		public Int32 FlRunSpd { get; set; }
		public Int32 FlWalkSpd { get; set; }
		public Int32 FlyRunSpd { get; set; }
		public Int32 FlyWalkSpd { get; set; }

		public bool InPvP { get; set; }
		public bool IsFishing { get; set; }
		public bool IsHeroIcon { get; set; }
		public bool IsHeroGlow { get; set; }
		public bool IsFindParty { get; set; }
		public bool IsClanLeader { get; set; }
		public byte HasDwarfCraft { get; set; }

		public Library.Point Position { get; set; }
		public Library.Point Fish { get; set; }

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

		// ToDo EquipObj | EquipItem

		public Int16 RecLeft { get; set; }
		public Int16 RecAmount { get; set; }
		public Int16 InventoryLimit { get; set; }
		public Int32 SpecialEffects { get; set; }
		public Int32 AbnormalEffects { get; set; }
		public Int32 AccessLevel { get; set; }
		public byte Enchant { get; set; }
		public byte TeamCircle { get; set; }

		public List<Int32> Cubics { get; set; }
	}
}
