﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class MyTargetSelected : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			ObjectId = reader.ReadInt32();
			TargetColor = reader.ReadInt16();
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override byte Id { get { return 0xa6; } }
		public Int32 ObjectId { get; set; }
		public Int16 TargetColor { get; set; }
	}
}
