using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace L2Apf.Server.Login
{
	// Todo Move to Server.Login.Api
	// Todo: Thread safe || Async socket
	// Todo: Add transaction checks (в том числе вызовы методов)
	public sealed class Api : IDisposable
	{
		private int SessionId;
		private byte[] LoginKey;
		private byte[] GameKey;
		private byte[] EncKey;
		private System.Net.Sockets.TcpClient Connection;
		private System.Threading.Thread LoginThread;
		private Library.BlowfishEngine Blowfish;
		private Model.LoginServer Server;
		private Model.Account Account;
		private NLog.Logger Logger;

		public Model.GameServer GameServer { get; private set; }
		public State State { get; private set; } // ToDo: Replace to transaction
		public bool IsBackground { get { return LoginThread.IsBackground; } set { LoginThread.IsBackground = value; } }

		public event ConnectedHandler Connected;
		public event LoginFailHandler LoginFail;
		public event ServerSelectedHandler ServerSelected;
		public event PlayFailHandler PlayFail;
		public delegate void LoginFailHandler(Packet.Server.LoginFail.ReasonType reason);
		public delegate void ConnectedHandler(IEnumerable<Model.GameServer> gameServers);
		public delegate void ServerSelectedHandler(byte[] LoginKey, byte[] GameKey);
		public delegate void PlayFailHandler(Packet.Server.PlayFail.ReasonType reason);


		public Api()
		{
			Logger = NLog.LogManager.GetCurrentClassLogger();
			State = State.Init;

			LoginThread = new System.Threading.Thread(Main);
		}

		private void Main()
		{
			using (Connection = new System.Net.Sockets.TcpClient())
			{
				try
				{
					Connection.Connect(Server.Address, Server.Port);
					Blowfish = new Library.BlowfishEngine(Server.Token);
					State = State.Load;

					BinaryReader br = new BinaryReader(Connection.GetStream());
					for (int n = 0; State == State.Load; n++)
					{
						int size = br.ReadInt16();
						byte[] buffer = br.ReadBytes(size - 2);

						if (size > 2)
						{
							if (n > 0) //ToDo encrypt = true, false
								buffer = Blowfish.Decrypt(buffer);

							Logger.Trace("Server packet: " + Packet.Utils.prettyHex(buffer));

							switch (buffer[0])
							{
								case 0x00:
									_onInit(buffer);
									break;
								case 0x01:
									_onLoginFail(buffer);
									State = State.Init;
									break;
								case 0x03:
									_onLoginOk(buffer);
									break;
								case 0x04:
									_onServerList(buffer);
									break;
								case 0x06:
									_onPlayFail(buffer);
									State = State.Init;
									break;
								case 0x07:
									_onPlayOk(buffer);
									State = State.Last;
									break;
								case 0x0B:
									//gameguard check verified from server
									_onGameGuardReply(buffer);
									break;
							}
						}
						else if (size < 2)
							throw new EndOfStreamException();
					}

					if (State == State.Last)
					{
						if (ServerSelected != null)
							ServerSelected(LoginKey, GameKey);
					}
					else
					{
						Logger.Fatal("State fail");
					}
				}
				catch (System.Net.Sockets.SocketException e)
				{
					Logger.Error(e.Message);
				}
				catch (Exception e)
				{
					Logger.Fatal(e.ToString());
				}
				finally
				{
					Connection.Close();
				}
			}
		}

		public void Connect(Model.LoginServer loginServer, Model.Account account)
		{
			this.Server = loginServer;
			this.Account = account;
			LoginThread.Start();
		}

		public void SelectGameServer(Model.GameServer gameServer)
		{
			if (gameServer != null)
				GameServer = gameServer;
			else
				throw new ArgumentNullException();

			SendPacket(new Packet.Client.RequestServerLogin()
			{
				ServerId = (byte)gameServer.Id,
				LoginKey = LoginKey
			});
		}

		private void _onInit(byte[] buffer)
		{
			Packet.Server.Init inp = new Packet.Server.Init();
			inp.Parse(buffer);

			SessionId = inp.SessionId;
			EncKey = inp.EncKey;

			SendPacket(new Packet.Client.GameGuardAuth()
			{
				SessionId = SessionId
			});
		}

		private void _onGameGuardReply(byte[] buffer)
		{
			SendPacket(new Packet.Client.RequestAuthLogin()
			{
				Login = Account.Login,
				Password = Account.Password,
				SessionId = SessionId,
				EncKey = EncKey
			});
		}

		private void _onServerList(byte[] buffer)
		{
			Packet.Server.GameServerList gsp = new Packet.Server.GameServerList();
			gsp.Parse(buffer);

			if (Connected != null)
				Connected(gsp.List);
		}

		private void _onLoginFail(byte[] buffer)
		{
			Packet.Server.LoginFail lfp = new Packet.Server.LoginFail();
			lfp.Parse(buffer);

			if (LoginFail != null)
				LoginFail(lfp.Reason);
		}

		private void _onPlayFail(byte[] buffer)
		{
			Packet.Server.PlayFail pfp = new Packet.Server.PlayFail();
			pfp.Parse(buffer);

			if (PlayFail != null)
				PlayFail(pfp.Reason);
		}

		private void _onLoginOk(byte[] buffer)
		{
			Packet.Server.LoginOk lop = new Packet.Server.LoginOk();
			lop.Parse(buffer);
			LoginKey = lop.LoginKey;

			SendPacket(new Packet.Client.RequestServerList()
			{
				LoginKey = LoginKey
			});
		}

		private void _onPlayOk(byte[] buffer)
		{
			Packet.Server.PlayOk pop = new Packet.Server.PlayOk();
			pop.Parse(buffer);
			GameKey = pop.GameKey;
		}

		private void SendPacket(Packet.Packet packet)
		{
			byte[] buffer = packet.Build();

			Logger.Trace("Client packet: " + Packet.Utils.prettyHex(buffer));

			System.IO.BinaryWriter bw = new System.IO.BinaryWriter(Connection.GetStream());
			bw.Write((Int16)(buffer.Length + 2));
			bw.Write(Blowfish.Encrypt(buffer));
		}

		public void Dispose()
		{
			try
			{
				if (LoginThread != null & LoginThread.IsAlive)
					LoginThread.Abort();
			}
			catch (System.Threading.ThreadAbortException) { }

			if (Connection != null)
				Connection.Close();

			NLog.LogManager.Flush();

			State = State.Init;
		}
	}
}
