using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L2Apf.Packet
{
	public class Reader : BinaryReader
	{
		public Reader(Stream stream) : base(stream)
		{

		}

		// ToDo
		/*public string ReadString()
		{
			return ReadString(Encoding.Unicode);
		}*/

		public string ReadString(Encoding encoding)
		{
			if (!BaseStream.CanSeek || !BaseStream.CanRead)
				throw new NotSupportedException("Stream not supported");

			switch (encoding.HeaderName)
			{
				case "utf-16":
					{
						const int W = 2;

						int length = 0;
						while (this.ReadInt16() != 0)
							length += W;

						BaseStream.Seek(-(length + W), SeekOrigin.Current);
						string value = encoding.GetString(ReadBytes(length));
						BaseStream.Seek(W, SeekOrigin.Current);
						return value;
					}
				case "us-ascii":
				{
					const int W = 1;

					int length = 0;
					while (this.ReadByte() != 0)
						length += W;

					BaseStream.Seek(-(length + W), SeekOrigin.Current);
					string value = encoding.GetString(ReadBytes(length));
					BaseStream.Seek(W, SeekOrigin.Current);
					return value;
				}
				default:
					throw new NotSupportedException("Encoding not supported");
			}
		}
	}
}
