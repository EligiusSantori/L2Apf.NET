using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class HennaInfo : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			INT = reader.ReadByte();
			STR = reader.ReadByte();
			CON = reader.ReadByte();
			MEN = reader.ReadByte();
			DEX = reader.ReadByte();
			WIT = reader.ReadByte();

			int count = reader.ReadInt32();
			Symbols = new Int32[count];
			for(int i = 0; i < count; i++)
			{
				reader.ReadInt32();
				Symbols[i] = reader.ReadInt32();
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xe4; } }
		public byte INT { get; set; }
		public byte STR { get; set; }
		public byte CON { get; set; }
		public byte MEN { get; set; }
		public byte DEX { get; set; }
		public byte WIT { get; set; }
		public Int32[] Symbols { get; set; }
	}
}
