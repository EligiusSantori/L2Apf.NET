using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class MagicSkillLaunched : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			SkillId = reader.ReadInt32();
			SkillLevel = reader.ReadInt32();
			reader.ReadInt32();
			TargetId = reader.ReadInt32();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x76; } }
		public Int32 ObjectId { get; set; }
		public Int32 TargetId { get; set; }
		public Int32 SkillId { get; set; }
		public Int32 SkillLevel { get; set; }
	}
}
