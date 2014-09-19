using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet
{
	public class Writer : BinaryWriter
	{
		public Writer(Stream stream) : base(stream)
		{

		}

		public void WriteString(string value, Encoding encoding)
		{
			if (!BaseStream.CanWrite)
				throw new NotSupportedException("Stream not supporter");

			switch (encoding.HeaderName)
			{
				case "utf-16":
					{
						Write(encoding.GetBytes(value));
						Write((Int16)0x00);
					}
					break;
				case "us-ascii":
					{
						Write(encoding.GetBytes(value));
						Write((byte)0x00);
					}
					break;
				default:
					throw new NotSupportedException("Encoding not supported");
			}
		}
	}
}
