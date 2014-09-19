using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Server
{
	public sealed class ShortcutInit : Packet
	{
		public override void Parse(byte[] buffer)
		{
			Reader reader = new Reader(new MemoryStream(buffer));

			reader.ReadByte(); // Id
			
			int Count = reader.ReadInt32();
			List = new List<Model.Shortcut>(Count);

			for (int i = 0; i < Count; i++)
			{
				Model.Shortcut shortcut = new Model.Shortcut();
				shortcut.Type = (Model.Shortcut.type)reader.ReadInt32();
				shortcut.Number = reader.ReadInt32();

				switch(shortcut.Type)
				{
					case Model.Shortcut.type.Item:
						shortcut.DataId = reader.ReadInt32();
						break;
					case Model.Shortcut.type.Skill:
						shortcut.DataId = reader.ReadInt32();
						shortcut.Level = reader.ReadInt32();
						break;
					case Model.Shortcut.type.Action:
						shortcut.DataId = reader.ReadInt32();
						break;
					case Model.Shortcut.type.Macro:
						shortcut.DataId = reader.ReadInt32();
						break;
					case Model.Shortcut.type.Recipe:
						shortcut.DataId = reader.ReadInt32();
						break;
					default:
						throw new NotSupportedException();
				}

				reader.ReadInt32(); //? 0x01
				List.Add(shortcut);
			}
		}

		public override byte[] Build()
		{
			throw new NotImplementedException();
		}


		public override byte Id { get { return 0x45; } }
		public List<Model.Shortcut> List { get; set; }
	}
}
