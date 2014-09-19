using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class MagicSkillUser : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			TargetId = reader.ReadInt32();
			SkillId = reader.ReadInt32();
			SkillLevel = reader.ReadInt32();
			CastTime = TimeSpan.FromMilliseconds(reader.ReadInt32());
			ReuseDelay = TimeSpan.FromMilliseconds(reader.ReadInt32());

			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);

			/*reader.ReadInt32();
			TargetX = reader.ReadInt32();
			TargetY = reader.ReadInt32();
			TargetZ = reader.ReadInt32();*/
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x48; } }
		public Int32 ObjectId { get; set; }
		public Int32 TargetId { get; set; }
		public Int32 SkillId { get; set; }
		public Int32 SkillLevel { get; set; }
		public TimeSpan CastTime { get; set; }
		public TimeSpan ReuseDelay { get; set; }
		public Library.Point Position { get; set; }
		//public Library.Point Destination { get; set; }
	}
}
