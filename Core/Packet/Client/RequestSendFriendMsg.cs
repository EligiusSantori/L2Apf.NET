﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet.Client
{
	public sealed class RequestSendFriendMsg : Packet
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
			writer.WriteString(Name, Encoding);

			return ms.GetBuffer();
		}

		public override byte Id { get { return 0xcc; } }
		private int Size
		{
			get
			{
				return 1
					+ Encoding.GetByteCount(Name)
					+ Encoding.GetByteCount(Message)
					+ Encoding.GetByteCount("\0\0");
			}
		}
		public string Name { get; set; }
		public string Message { get; set; }
	}
}