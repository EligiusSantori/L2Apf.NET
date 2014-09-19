using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace L2Apf
{
	public sealed class LoginFile
	{
		public LoginFile(string file)
		{
			XmlDocument document = new XmlDocument();
			document.Load(file);

			foreach (XmlNode section in document.DocumentElement.ChildNodes)
				switch (section.Name.ToLower())
				{
					case "loginserver":
						LoginServer = new Model.LoginServer();
						foreach (XmlNode property in section.ChildNodes)
							switch (property.Name.ToLower())
							{
								case "address": LoginServer.Address = property.InnerText; break;
								case "port": LoginServer.Port = int.Parse(property.InnerText); break;
								case "protocol": LoginServer.Protocol = int.Parse(property.InnerText); break;
								case "token": LoginServer.Token = GetToken(property.InnerText); break;
							}
						break;
					case "gameserver":
						foreach (XmlNode property in section.ChildNodes)
							switch (property.Name.ToLower())
							{
								case "id": GameServer = int.Parse(property.InnerText); break;
							}
						break;
					case "account":
						Account = new Model.Account();
						foreach (XmlNode property in section.ChildNodes)
							switch (property.Name.ToLower())
							{
								case "login": Account.Login = property.InnerText; break;
								case "password": Account.Password = property.InnerText; break;
								case "character": Character = property.InnerText; break;
							}
						break;
				}

			if (LoginServer == null || Account == null || string.IsNullOrEmpty(Character))
				throw new System.FormatException();
		}

		static public byte[] GetToken(string token, char separator = ' ')
		{
			var s = System.Globalization.NumberStyles.HexNumber;
			return token.Split(separator).Select<string, byte>
				(b => byte.Parse(b, s)).ToArray();
		}


		public Model.LoginServer LoginServer { get; set; }
		public Model.Account Account { get; set; }
		public int GameServer { get; set; }
		public string Character { get; set; }
	}
}
