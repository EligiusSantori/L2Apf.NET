using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class CharInfo : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);
			Heading = reader.ReadInt32();
			ObjectId = reader.ReadInt32();
			Name = reader.ReadString(Encoding.Unicode);
			Race = reader.ReadInt32();
			Gender = (Lineage.Gender)reader.ReadInt32();
			Class = reader.ReadInt32();

			Underwear = reader.ReadInt32();
			Head = reader.ReadInt32();
			RightHand = reader.ReadInt32();
			LeftHand = reader.ReadInt32();
			Gloves = reader.ReadInt32();
			Chest = reader.ReadInt32();
			Legs = reader.ReadInt32();
			Feet = reader.ReadInt32();
			Back = reader.ReadInt32();
			BothHand = reader.ReadInt32();
			Hair = reader.ReadInt32();

			IsPvP = reader.ReadInt32() != 0;
			Karma = reader.ReadInt32();
			MAttackSpeed = reader.ReadInt32();
			PAttackSpeed = reader.ReadInt32();
			reader.ReadInt32(); // IsPvP again
			reader.ReadInt32(); // Karma again
			RunSpeed = reader.ReadInt32();
			WalkSpeed = reader.ReadInt32();
			SwimRunSpeed = reader.ReadInt32();
			SwimWalkSpeed = reader.ReadInt32();
			FlRunSpeed = reader.ReadInt32();
			FlWalkSpeed = reader.ReadInt32();
			FlyRunSpeed = reader.ReadInt32();
			FlyWalkSpeed = reader.ReadInt32();
			MoveSpeedMult = reader.ReadDouble();
			AttackSpeedMult = reader.ReadDouble();
			CollisionRadius = reader.ReadDouble();
			CollisionHeight = reader.ReadDouble();

			HairSytle = reader.ReadInt32();
			HairColor = reader.ReadInt32();
			FaceType = reader.ReadInt32();
			Title = reader.ReadString(Encoding.Unicode);
			ClanId = reader.ReadInt32();
			ClanCrestId = reader.ReadInt32();
			AllyId = reader.ReadInt32();
			AllyCrestId = reader.ReadInt32();
			
			SiegeFlags = reader.ReadInt32();
			IsStanding = reader.ReadByte() != 0;
			IsRunning = reader.ReadByte() != 0;
			IsInCombat = reader.ReadByte() != 0;
			IsAlikeDead = reader.ReadByte() != 0;
			IsInvisible = reader.ReadByte() != 0;
			MountType = (Lineage.MountType)reader.ReadByte();
			PrivateStore = (Lineage.PrivateStore)reader.ReadByte();

			Cubics = new Int16[reader.ReadInt16()];
			for (int i = 0; i < Cubics.Length; i++)
				Cubics[i] = reader.ReadInt16();

			IsFindParty = reader.ReadByte() != 0;
			AbnormalEffects = reader.ReadInt32();
			RecommendLeft = reader.ReadByte();
			RecommendAmount = reader.ReadInt16();
			reader.ReadInt32();
			MaxCp = reader.ReadInt32();
			Cp = reader.ReadInt32();
			EnchantAmount = reader.ReadByte();
			TeamCircle = reader.ReadByte();
			ClanLargeCrestId = reader.ReadInt32();
			IsHeroIcon = reader.ReadByte() != 0;
			IsHeroGlow = reader.ReadByte() != 0;
			IsFishing = reader.ReadByte() != 0;

			Fish = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32());

			NameColor = reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x03; } }
		public string Name { get; set; }
		public string Title { get; set; }
		public Library.Point Position { get; set; }
		public Library.Point Fish { get; set; }

		public Int32 Heading { get; set; }
		public Int32 ObjectId { get; set; }
		public Int32 Race { get; set; }
		public Lineage.Gender Gender { get; set; }
		public Int32 Class { get; set; }
		public Int32 HairSytle { get; set; } // ToDo enum
		public Int32 HairColor { get; set; } // ToDo enum
		public Int32 FaceType { get; set; } // ToDo enum
		public Int32 ClanId { get; set; }
		public Int32 ClanCrestId { get; set; }
		public Int32 ClanLargeCrestId { get; set; }
		public Int32 AllyId { get; set; }
		public Int32 AllyCrestId { get; set; }
		public Int32 NameColor { get; set; }

		public Int32 Karma { get; set; }
		public Int32 MAttackSpeed { get; set; }
		public Int32 PAttackSpeed { get; set; }
		public Int32 RunSpeed { get; set; }
		public Int32 WalkSpeed { get; set; }
		public Int32 SwimRunSpeed { get; set; }
		public Int32 SwimWalkSpeed { get; set; }
		public Int32 FlRunSpeed { get; set; }
		public Int32 FlWalkSpeed { get; set; }
		public Int32 FlyRunSpeed { get; set; }
		public Int32 FlyWalkSpeed { get; set; }
		public Double MoveSpeedMult { get; set; }
		public Double AttackSpeedMult { get; set; }
		public Double CollisionRadius { get; set; }
		public Double CollisionHeight { get; set; }
		public Byte EnchantAmount { get; set; }
		public Byte RecommendLeft { get; set; }
		public Int16 RecommendAmount { get; set; }
		public Int32 Cp { get; set; }
		public Int32 MaxCp { get; set; }
		public Int16[] Cubics { get; set; }
		public Byte TeamCircle { get; set; }

		public Int32 Underwear { get; set; }
		public Int32 Head { get; set; }
		public Int32 RightHand { get; set; }
		public Int32 LeftHand { get; set; }
		public Int32 Gloves { get; set; }
		public Int32 Chest { get; set; }
		public Int32 Legs { get; set; }
		public Int32 Feet { get; set; }
		public Int32 Back { get; set; }
		public Int32 BothHand { get; set; }
		public Int32 Hair { get; set; }

		public bool IsPvP { get; set; }
		public bool IsStanding { get; set; }
		public bool IsRunning { get; set; }
		public bool IsInCombat { get; set; }
		public bool IsAlikeDead { get; set; }
		public bool IsInvisible { get; set; }
		public bool IsFindParty { get; set; }
		public bool IsHeroIcon { get; set; }
		public bool IsHeroGlow { get; set; }
		public bool IsFishing { get; set; }
		public Lineage.MountType MountType { get; set; }
		public Lineage.PrivateStore PrivateStore { get; set; }
		public Int32 SiegeFlags { get; set; }
		public Int32 AbnormalEffects { get; set; }
	}
}
