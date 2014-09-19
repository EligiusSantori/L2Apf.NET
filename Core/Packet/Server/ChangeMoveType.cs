using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class ChangeMoveType : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			MoveType = (Lineage.MoveType)reader.ReadInt32();
			reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x2e; } }
		public Int32 ObjectId { get; set; }
		public Lineage.MoveType MoveType { get; set; }
	}
}
