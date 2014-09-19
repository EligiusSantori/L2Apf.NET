using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class Init : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			SessionId = reader.ReadInt32();
			Protocol = reader.ReadInt32();
			EncKey = reader.ReadBytes(128);

			//got the encoded key in enckey
			// step 4 : xor last 0x40 bytes with  first 0x40 bytes
			for (int i = 0; i < 0x40; i++)
				EncKey[0x40 + i] = (byte)(EncKey[0x40 + i] ^ EncKey[i]);

			// step 3 : xor bytes 0x0d-0x10 with bytes 0x34-0x38
			for (int i = 0; i < 4; i++)
				EncKey[0x0d + i] = (byte)(EncKey[0x0d + i] ^ EncKey[0x34 + i]);

			// step 2 : xor first 0x40 bytes with  last 0x40 bytes 
			for (int i = 0; i < 0x40; i++)
				EncKey[i] = (byte)(EncKey[i] ^ EncKey[0x40 + i]);

			// step 1 : 0x4d-0x50 <-> 0x00-0x04
			for (int i = 0; i < 4; i++)
			{
				byte temp = EncKey[i];
				EncKey[i] = EncKey[0x4d + i];
				EncKey[0x4d + i] = temp;
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x00; } }
		public Int32 SessionId { get; set; }
		public Int32 Protocol { get; set; }
		public byte[] EncKey { get; set; }
	}
}
