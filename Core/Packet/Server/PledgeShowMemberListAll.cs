using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	/// <summary>
	/// Clan members list
	/// </summary>
	public sealed class PledgeShowMemberListAll : Packet
	{
		public sealed class ClanMember
		{
			public string Name { get; set; }
			public Int32 Level { get; set; }
			public Int32 ClassId { get; set; }
			public Int32 ObjectId { get; set; }
			public bool IsOnline { get { return ObjectId != 0; } }
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ClanId = reader.ReadInt32();
			ClanName = reader.ReadString(Encoding.Unicode);
			LeaderName = reader.ReadString(Encoding.Unicode);
			ClanCrestId = reader.ReadInt32();
			ClanLevel = reader.ReadInt32();
			HasCastle = reader.ReadInt32() != 0;
			HasClanhall = reader.ReadInt32() != 0;
			reader.ReadInt32();
			CharLevel = reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			AllyId = reader.ReadInt32();
			AllyName = reader.ReadString(Encoding.Unicode);
			AllyCrestId = reader.ReadInt32();
			InWar = reader.ReadInt32() != 0;

			int count = reader.ReadInt32();
			Members = new ClanMember[count];
			for (int i = 0; i < count; i++)
			{
				ClanMember Member = new ClanMember();
				Member.Name = reader.ReadString(Encoding.Unicode);
				Member.Level = reader.ReadInt32();
				Member.ClassId = reader.ReadInt32();
				reader.ReadInt32();
				reader.ReadInt32();
				Member.ObjectId = reader.ReadInt32();
				Members[i] = Member;
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x53; } }

		public Int32 ClanId { get; set; }
		public Int32 ClanCrestId { get; set; }
		public Int32 ClanLevel { get; set; }
		
		public Int32 AllyId { get; set; }
		public Int32 AllyCrestId { get; set; }

		public Int32 CharLevel { get; set; }

		public string LeaderName { get; set; }
		public string ClanName { get; set; }
		public string AllyName { get; set; }
		
		public bool InWar { get; set; }
		public bool InAlly { get { return AllyId != 0; } }
		public bool HasCastle { get; set; }
		public bool HasClanhall { get; set; }

		public ClanMember[] Members { get; set; }
	}
}
