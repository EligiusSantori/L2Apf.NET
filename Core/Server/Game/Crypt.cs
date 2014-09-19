using System;
using System.IO;
using System.Linq;

namespace L2Apf.Server.Game
{
	public class Crypt
	{
		private byte[] Key = new byte[8];

		public void SetKey(byte[] _key)
		{
			Array.Copy(_key, Key, Key.Length);
		}

		public void Encrypt(byte[] buffer)
		{
			byte prev = 0;
			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = prev = (byte)(buffer[i] ^ prev ^ Key[i % 8]);

			int temp = BitConverter.ToInt32(Key, 0);
			temp = unchecked(temp + buffer.Length);
			Key[0] = (byte)(temp & 0xff);
			Key[1] = (byte)(temp >> 8 & 0xff);
			Key[2] = (byte)(temp >> 16 & 0xff);
			Key[3] = (byte)(temp >> 24 & 0xff);
		}

		public void Decrypt(byte[] buffer)
		{
			byte b1 = 0, b2 = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				b1 = buffer[i];
				buffer[i] = (byte)(b1 ^ b2 ^ Key[i % 8]);
				b2 = b1;
			}

			int temp = BitConverter.ToInt32(Key, 0);
			temp = unchecked(temp + buffer.Length);
			Key[0] = (byte)(temp & 0xff);
			Key[1] = (byte)(temp >> 8 & 0xff);
			Key[2] = (byte)(temp >> 16 & 0xff);
			Key[3] = (byte)(temp >> 24 & 0xff);
		}
	}
}
