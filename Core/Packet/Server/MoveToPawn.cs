using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	/// <summary>
	/// Описывает перемещения окружающих объектов
	/// </summary>
	public sealed class MoveToPawn : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			TargetId = reader.ReadInt32();
			Distance = reader.ReadInt32();
			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x60; } }
		public Int32 ObjectId { get; set; }
		public Int32 TargetId { get; set; }
		public Double Distance { get; set; }
		public Library.Point Position { get; set; }
	}
}
