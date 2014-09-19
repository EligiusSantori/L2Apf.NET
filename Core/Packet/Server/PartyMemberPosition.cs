using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class PartyMemberPosition : Packet
	{
		public struct Member
		{
			public Int32 ObjectId;
			public Library.Point Position;
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Members = new Member[reader.ReadInt32()];
			for (int i = 0; i < Members.Length; i++)
				Members[i] = new Member()
				{
					ObjectId = reader.ReadInt32(),
					Position = new Library.Point(
						reader.ReadInt32(),
						reader.ReadInt32(),
						reader.ReadInt32()
					)
				};
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xa7; } }
		public Member[] Members { get; set; }
	}
}
