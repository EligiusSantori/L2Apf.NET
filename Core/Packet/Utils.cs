using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace L2Apf.Packet
{
	sealed class Utils
	{
		private Utils()
		{
			throw new NotSupportedException();
		}

		static public ulong CheckSum(byte[] raw, int count) // Todo: out from packet to raw send/read
		{
			ulong chksum = 0;
			ulong ecx = 0;
			int i = 0;

			for (i = 0; i < count; i += 4)
			{
				ecx = (ulong)((ulong)raw[i] & 0xff);
				ecx |= (ulong)((ulong)raw[i + 1] << 8 & 0xff00);
				ecx |= (ulong)((ulong)raw[i + 2] << 16 & 0xff0000);
				ecx |= (ulong)((ulong)raw[i + 3] << 24 & 0xff000000);

				chksum = chksum ^ ecx;
			}

			raw[i] = (byte)(chksum & 0xff);
			raw[i + 1] = (byte)(chksum >> 8 & 0xff);
			raw[i + 2] = (byte)(chksum >> 16 & 0xff);
			raw[i + 3] = (byte)(chksum >> 24 & 0xff);

			return chksum;
		}

		[Obsolete("To Library.StringFromHex?")]
		static public string prettyHex(byte[] buffer)
		{
			return string.Join(" ", buffer.Select(b =>  b.ToString("X2")));
		}
	}
}
