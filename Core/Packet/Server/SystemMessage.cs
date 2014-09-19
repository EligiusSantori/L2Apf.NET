using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class SystemMessage : Packet
	{
		public enum type
		{
			Text = 0x00,
			Number = 0x01,
			NpcName = 0x02,
			ItemName = 0x03,
			SkillName = 0x04,
		};

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			MessageId = reader.ReadInt32();

			int count = reader.ReadInt32();
			Arguments = new KeyValuePair<type, object>[count];
			for (int i = 0; i < count; i++)
			{
				var key = (type)reader.ReadInt32();
				object value = null;
				switch (key)
				{
					case type.Text:
						value = reader.ReadString(Encoding.Unicode);
						break;
					case type.Number:
						value = reader.ReadInt32();
						break;
					case type.NpcName:
						value = reader.ReadInt32();
						break;
					case type.ItemName:
						value = reader.ReadInt32();
						break;
					case type.SkillName:
						value = reader.ReadInt32();
						reader.ReadInt32(); //there are 4 more bytes after this... the skill level by chance?
						break;
				}
				Arguments[i] = new KeyValuePair<type, object>(key, value);
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x64; } }
		public Int32 MessageId { get; set; }
		public KeyValuePair<type, object>[] Arguments { get; set; }
	}
}
