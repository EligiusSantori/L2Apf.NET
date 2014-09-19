using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class StatusUpdate : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			Attributes = new KeyValuePair<Int32, Int32>[reader.ReadInt32()];
			for (int i = 0; i < Attributes.Length; i++)
				Attributes[i] = new KeyValuePair<Int32, Int32>(
					reader.ReadInt32(),	
					reader.ReadInt32()
				);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x0e; } }
		public Int32 ObjectId { get; set; }
		public KeyValuePair<Int32, Int32>[] Attributes { get; set; }
	}
}
