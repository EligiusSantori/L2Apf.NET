using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class Attack : Packet
	{
		public struct Hit
		{
			public Int32 TargetId;
			public Int32 Damage;
			public byte Flags;
		};

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));
			
			reader.ReadByte(); // Id
			AttackerId = reader.ReadInt32();

			Hit hit = new Hit()
			{
				TargetId = reader.ReadInt32(),
				Damage = reader.ReadInt32(),
				Flags = reader.ReadByte()
			};

			Position = new Library.Point(
				reader.ReadInt32(),
				reader.ReadInt32(),
				reader.ReadInt32()
			);

			int count = reader.ReadInt16();
			Hits = new Hit[count + 1];
			Hits[0] = hit;
			for (int i = 1; i < Hits.Length; i++)
				Hits[i] = new Hit()
				{
					TargetId = reader.ReadInt32(),
					Damage = reader.ReadInt32(),
					Flags = reader.ReadByte()
				};
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0x05; } }
		public Int32 AttackerId { get; set; }
		public Library.Point Position { get; set; }
		public Hit[] Hits { get; set; }
	}
}
