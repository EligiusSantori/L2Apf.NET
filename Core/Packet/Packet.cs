using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Packet
{
	public abstract class Packet
	{
		public abstract void Parse(byte[] buffer);
		/*{
			BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
		}*/

		public abstract byte[] Build();
		/*{
			BinaryWriter writer = new BinaryWriter(new MemoryStream(Size));
			return (writer.BaseStream as MemoryStream).GetBuffer();
		}*/

		public abstract byte Id { get; }

		protected const int ID_SIZE = 1;
		protected const int LENGTH_SIZE = 0; // 2
		protected const int CHECKSUM_SIZE = 4;
		protected readonly Encoding Encoding = Encoding.Unicode;
	}
}
