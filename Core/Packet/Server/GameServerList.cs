using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class GameServerList : Packet
	{
		public override byte[] Build()
		{
			throw new NotImplementedException();
		}

		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			int Count = reader.ReadByte();
			reader.ReadByte(); // ?

			List = new List<Model.GameServer>(Count);
			for (int i = 0; i < Count; i++)
				List.Add(new Model.GameServer()
				{
					Id = reader.ReadByte(),
					Address = string.Format("{0}.{1}.{2}.{3}", // ToDo: replace to native
						reader.ReadByte().ToString(),
						reader.ReadByte().ToString(),
						reader.ReadByte().ToString(),
						reader.ReadByte().ToString()),
					Port = reader.ReadInt32(),
					Age = reader.ReadByte(),
					PvP = reader.ReadByte() != 0,
					Online = reader.ReadInt16(),
					Maximum = reader.ReadInt16(),
					State = reader.ReadByte() != 0,
					Poop = string.Format("{0}-{1}-{2}-{3}-{4}", // ToDo: What is?
						reader.ReadByte().ToString("X2"),
						reader.ReadByte().ToString("X2"),
						reader.ReadByte().ToString("X2"),
						reader.ReadByte().ToString("X2"),
						reader.ReadByte().ToString("X2"))
				});
		}

		
		public override byte Id { get { return 4; } }
		public List<Model.GameServer> List { get; set; }
	}
}
