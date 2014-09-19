using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class Say2 : Packet
	{
		public override void Parse(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] Build()
		{
			MemoryStream ms = new MemoryStream(Size);
			Writer writer = new Writer(ms);

			writer.Write(Id);
			writer.WriteString(Message, Encoding);
			writer.Write((Int32)Channel);
			if(NeedTarget())
				writer.WriteString(Target, Encoding);

			return ms.GetBuffer();
		}

		private bool NeedTarget()
		{
			return Channel == Lineage.Channel.Tell && !string.IsNullOrEmpty(Target);
		}

		public override byte Id { get { return 0x38; } }
		private int Size
		{
			get
			{
				return 1 + 4
					+ Encoding.GetByteCount(Message + '\0')
					+ (NeedTarget() ? Encoding.GetByteCount(Target + '\0') : 0);
			}
		}
		public Lineage.Channel Channel { get; set; }
		public string Message { get; set; }
		public string Target { get; set; }
	}
}
