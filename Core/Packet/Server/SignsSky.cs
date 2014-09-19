using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class SignsSky : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			Sky = Lineage.SignsSky.None;
			if(reader.BaseStream.Length > 1)
				switch (reader.ReadInt16())
				{
					case 257: Sky = Lineage.SignsSky.Dusk; break;
					case 258: Sky = Lineage.SignsSky.Dawn; break;
				}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xf8; } }
		public Lineage.SignsSky Sky { get; set; }
	}
}
