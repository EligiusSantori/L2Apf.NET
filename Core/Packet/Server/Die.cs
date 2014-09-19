using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class Die : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			Points += reader.ReadInt32() != 0 ? (int)Lineage.ReturnPoint.Town : 0;
			Points += reader.ReadInt32() != 0 ? (int)Lineage.ReturnPoint.Clanhall : 0;
			Points += reader.ReadInt32() != 0 ? (int)Lineage.ReturnPoint.Castle : 0;
			Points += reader.ReadInt32() != 0 ? (int)Lineage.ReturnPoint.SiegeHq : 0;
			IsSpoiled = reader.ReadInt32() != 0;
			Points += reader.ReadInt32() != 0 ? (int)Lineage.ReturnPoint.Fixed : 0;
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x06; } }
		public Int32 ObjectId { get; set; }
		public bool IsSpoiled { get; set; }
		public Lineage.ReturnPoint Points { get; set; }
	}
}
