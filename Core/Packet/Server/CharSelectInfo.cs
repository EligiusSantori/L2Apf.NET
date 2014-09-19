using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	[Obsolete("Remove Model dependency!")] // ToDo Remove Model dependency!
	public sealed class CharSelectInfo : Packet
	{
		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			int Count = reader.ReadInt32();

			List = new List<Model.Player>(Count);
			for(int i = 0; i < Count; i++)
			{
				var c = new Model.Player();
				c.Number = i;
				c.Name = reader.ReadString(Encoding.Unicode);
				c.ObjectId = reader.ReadInt32();
				c.Login = reader.ReadString(Encoding.Unicode);
				c.SessionId = reader.ReadInt32();
				c.ClanId = reader.ReadInt32();
					reader.ReadInt32();
				c.Gender = reader.ReadInt32() == 0 ?
					Lineage.Gender.Male :
					Lineage.Gender.Female;
				c.Race = reader.ReadInt32();
				c.BaseClassId = reader.ReadInt32();
				c.IsActive = reader.ReadInt32() != 0;
				c.Position = new Library.Point(
					reader.ReadInt32(),
					reader.ReadInt32(),
					reader.ReadInt32());
				c.Hp = reader.ReadDouble();
				c.Mp = reader.ReadDouble();
				c.Sp = reader.ReadInt32();
				c.Xp = reader.ReadInt32();
				c.Level = reader.ReadInt32();
				c.Karma = reader.ReadInt32();

				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();

				c.Equipment = new Model.Equipment()
				{
					Underwear = reader.ReadInt32(),
					RightEaring = reader.ReadInt32(),
					LeftEaring = reader.ReadInt32(),
					Neck = reader.ReadInt32(),
					RightFinger = reader.ReadInt32(),
					LeftFinger = reader.ReadInt32(),
					Head = reader.ReadInt32(),
					RightHand = reader.ReadInt32(),
					LeftHand = reader.ReadInt32(),
					Gloves = reader.ReadInt32(),
					Chest = reader.ReadInt32(),
					Legs = reader.ReadInt32(),
					Feet = reader.ReadInt32(),
					Back = reader.ReadInt32(),
					BothHand = reader.ReadInt32(),
					Hair = reader.ReadInt32(),
				};

				((Model.Character)c).Equipment = new Model.Equipment()
				{
					Underwear = reader.ReadInt32(),
					RightEaring = reader.ReadInt32(),
					LeftEaring = reader.ReadInt32(),
					Neck = reader.ReadInt32(),
					RightFinger = reader.ReadInt32(),
					LeftFinger = reader.ReadInt32(),
					Head = reader.ReadInt32(),
					RightHand = reader.ReadInt32(),
					LeftHand = reader.ReadInt32(),
					Gloves = reader.ReadInt32(),
					Chest = reader.ReadInt32(),
					Legs = reader.ReadInt32(),
					Feet = reader.ReadInt32(),
					Back = reader.ReadInt32(),
					BothHand = reader.ReadInt32(),
					Hair = reader.ReadInt32(),
				};

				c.HairStyle = reader.ReadInt32();
				c.HairColor = reader.ReadInt32();
				c.FaceType = reader.ReadInt32();

				c.MaxHp = (int)reader.ReadDouble();
				c.MaxMp = (int)reader.ReadDouble();

				c.ToRemove = reader.ReadInt32();
				c.ClassId = reader.ReadInt32();
				c.IsLastUsed = reader.ReadInt32() != 0;
				c.Enchant = reader.ReadByte();

				List.Add(c);
			}
		}


		public override byte Id { get { return 0x13; } }
		public List<Model.Player> List { get; set; }
	}
}
