using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class NpcInfo : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id

			ObjectId = reader.ReadInt32();
			NpcId = reader.ReadInt32();
			IsAttackable = reader.ReadInt32() != 0;
			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32());
			Heading = reader.ReadInt32();
				reader.ReadInt32();
			MAtkSpd = reader.ReadInt32();
			PAtkSpd = reader.ReadInt32();
			RunSpd = reader.ReadInt32();
			WalkSpd = reader.ReadInt32();
			SwimRunSpd = reader.ReadInt32();
			SwimWalkSpd = reader.ReadInt32();
			FlRunSpd = reader.ReadInt32();
			FlWalkSpd = reader.ReadInt32();
			FlyRunSpd = reader.ReadInt32();
			FlyWalkSpd = reader.ReadInt32();
			MoveSpdMult = reader.ReadDouble();
			AtkSpdMult = reader.ReadDouble();
			CollisionRadius = reader.ReadDouble();
			CollisionHeight = reader.ReadDouble();

			RightHand = reader.ReadInt32();
			BothHand = reader.ReadInt32();
			LeftHand = reader.ReadInt32();

			IsShowName = reader.ReadByte() != 0;
			IsRunning = reader.ReadByte() != 0;
			IsInCombat = reader.ReadByte() != 0;
			IsAlikeDead = reader.ReadByte() != 0;
			IsSummoned = reader.ReadByte() != 0;
			Name = reader.ReadString(Encoding.Unicode);
			Title = reader.ReadString(Encoding.Unicode);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}


		public override byte Id { get { return 0x16; } }

		public Int32 NpcId { get; set; }
		public Int32 ObjectId { get; set; }
		public Int32 Heading { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public Library.Point Position { get; set; }

		public Int32 MAtkSpd { get; set; }
		public Int32 PAtkSpd { get; set; }

		public Int32 RunSpd { get; set; }
		public Int32 WalkSpd { get; set; }
		public Int32 SwimRunSpd { get; set; }
		public Int32 SwimWalkSpd { get; set; }
		public Int32 FlRunSpd { get; set; }
		public Int32 FlWalkSpd { get; set; }
		public Int32 FlyRunSpd { get; set; }
		public Int32 FlyWalkSpd { get; set; }

		public Int32 RightHand { get; set; }
		public Int32 BothHand { get; set; }
		public Int32 LeftHand { get; set; }

		public Double MoveSpdMult { get; set; }
		public Double AtkSpdMult { get; set; }
		public Double CollisionRadius { get; set; }
		public Double CollisionHeight { get; set; }

		public bool IsAttackable { get; set; }
		public bool IsSummoned { get; set; }
		public bool IsAlikeDead { get; set; }
		public bool IsInCombat { get; set; }
		public bool IsRunning { get; set; }
		public bool IsShowName { get; set; }
	}
}
