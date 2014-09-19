using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestUserCommand : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(Size);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.Write(GetTypeMark(Type));

			return ms.GetBuffer();
		}

		private Int32 GetTypeMark(Lineage.UserCommand command)
		{
			switch (Type)
			{
				case Lineage.UserCommand.Loc: return 0x00;
				case Lineage.UserCommand.Unstuck: return 0x34;
				case Lineage.UserCommand.Mount: return 0x3d;
				case Lineage.UserCommand.Dismount: return 0x3e;
				case Lineage.UserCommand.Time: return 0x4d;
				case Lineage.UserCommand.PartyInfo: return 0x51;
				case Lineage.UserCommand.AttackList: return 0x58;
				case Lineage.UserCommand.EnemyList: return 0x59;
				case Lineage.UserCommand.WarList: return 0x5a;
				case Lineage.UserCommand.FriendList: return 0x60;
				case Lineage.UserCommand.ClanPenalty: return 0x64;
				default: throw new NotSupportedException();
			}
		}

		public override byte Id { get { return 0xaa; } }
		private int Size { get { return 5; } }
		public Lineage.UserCommand Type { get; set; }
	}
}
