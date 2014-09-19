using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace L2Apf.Server.Game
{
	sealed class Network : IDisposable
	{
		private string Login;
		private byte[] GameKey;
		private byte[] LoginKey;
		private bool Crypted;
		private bool Running;

		public event PacketHandler Readed;
		public event PacketHandler Sended;

		public delegate void PacketHandler(Network sender, Packet.Packet packet);

		private Thread ReadThread;
		private Thread SendThread;
		private Queue<Packet.Packet> ReadQueue = new Queue<Packet.Packet>();
		private Queue<Packet.Packet> SendQueue = new Queue<Packet.Packet>();
		private Crypt Encoder = new Crypt();
		private Crypt Decoder = new Crypt();

		private TcpClient Connection;
		private NLog.Logger Logger;


		public Network()
		{
			Logger = NLog.LogManager.GetCurrentClassLogger();
			Connection = new TcpClient();

			SendThread = new Thread(GameSendThread)
				{ IsBackground = true };
			ReadThread = new Thread(GameReadThread)
				{ IsBackground = true };
		}

		public void Start(string address, int port, int protocol, string login, byte[] loginKey, byte[] gameKey) // Todo: Split to {Start | Login}?
		{
			try
			{
				Connection.Connect(address, port);

				Running = true;
				Crypted = false;
				Login = login;
				GameKey = gameKey;
				LoginKey = loginKey;

				ReadThread.Start();
				SendThread.Start();

				Send(new Packet.Client.ProtocolVersion()
				{
					Protocol = protocol
				});
			}
			catch (System.Net.Sockets.SocketException e)
			{
				Logger.Error(e.Message);
			}
		}

		public void Crypt(byte[] key)
		{
			Crypted = true;
			Encoder.SetKey(key);
			Decoder.SetKey(key);

			Send(new Packet.Client.ValidateAuthLogin()
			{
				Login = Login,
				GameKey = GameKey,
				LoginKey = LoginKey
			});
		}

		public void Stop()
		{
			Running = false;

			try
			{
				if (ReadThread != null & ReadThread.IsAlive)
					ReadThread.Abort();
			}
			catch (System.Threading.ThreadAbortException)
			{
			
			}

			try
			{
				if (SendThread != null && SendThread.IsAlive)
					SendThread.Abort();
			}
			catch (System.Threading.ThreadAbortException)
			{
			
			}
		}

		public Packet.Packet Read()
		{
			Packet.Packet packet = null;
			lock (ReadQueue)
				if (ReadQueue.Count > 0)
					packet = ReadQueue.Dequeue();

			return packet;
		}

		public void Send(Packet.Packet packet)
		{
			lock (SendQueue)
				SendQueue.Enqueue(packet);

			if (SendThread.ThreadState.HasFlag(System.Threading.ThreadState.WaitSleepJoin))
				SendThread.Interrupt();
		}

		private void GameSendThread()
		{
			BinaryWriter bw = new BinaryWriter(Connection.GetStream());
			while (Running)
			{
				Packet.Packet packet = null;
				lock (SendQueue)
					if (SendQueue.Count > 0)
						packet = SendQueue.Dequeue();

				if (packet == null)
				{
					try
					{
						System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
					}
					catch (System.Threading.ThreadInterruptedException)
					{
						//everything worked perfect
					}
					continue;
				}

				var buffer = packet.Build();
				Logger.Trace("Client packet: " + Packet.Utils.prettyHex(buffer));
				if (Crypted) Encoder.Encrypt(buffer);

				int pl = buffer.Length + 2;
				bw.Write((Int16)pl);
				bw.Write(buffer);

				if (Sended != null)
					Sended(this, packet);
			}
		}

		private void GameReadThread()
		{
			BinaryReader br = new BinaryReader(Connection.GetStream());
			while (Running)
			{
				int size = br.ReadInt16(); // Todo stream.readByte(); if byte >=0 stream.readByte(); BitConverter.ToInt16
				byte[] buffer = br.ReadBytes(size - 2);

				if (size > 2)
				{
					if (Crypted) Decoder.Decrypt(buffer);
					Logger.Trace("Server packet: " + Packet.Utils.prettyHex(buffer));

					Parse(buffer);

				}
				else if (size < 2)
					throw new EndOfStreamException();
			}
		}

		private void Parse(byte[] buffer)
		{
			Packet.Packet packet = null;

			Action<Packet.Packet> create = (Packet.Packet _packet) =>
			{
				packet = _packet;
				packet.Parse(buffer);
			};

			if(buffer.Length > 0)
				switch (buffer[0])
				{
					case 0x00: create(new Packet.Server.KeyPacket()); break;
					case 0x13: create(new Packet.Server.CharSelectInfo()); break;
					case 0x01: create(new Packet.Server.CharMoveToLocation()); break;
					case 0x03: create(new Packet.Server.CharInfo()); break;
					case 0x04: create(new Packet.Server.UserInfo()); break;
					case 0x05: create(new Packet.Server.Attack()); break;
					case 0x06: create(new Packet.Server.Die()); break;
					case 0x07: create(new Packet.Server.Revive()); break;
					case 0x0B: create(new Packet.Server.SpawnItem()); break;
					case 0x0C: create(new Packet.Server.DropItem()); break;
					case 0x0D: create(new Packet.Server.GetItem()); break;
					case 0x0E: create(new Packet.Server.StatusUpdate()); break;
					case 0x0F: create(new Packet.Server.NpcHtmlMessage()); break;
					case 0x12: create(new Packet.Server.DeleteObject()); break;
					case 0x15: create(new Packet.Server.CharSelected()); break;
					case 0x16: create(new Packet.Server.NpcInfo()); break;
					case 0x1B: create(new Packet.Server.ItemList()); break;
					case 0x1C: create(new Packet.Server.Sunrise()); break;
					case 0x1D: create(new Packet.Server.Sunset()); break;
					case 0x25: create(new Packet.Server.ActionFailed()); break;
					case 0x27: create(new Packet.Server.InventoryUpdate()); break;
					case 0x28: create(new Packet.Server.TeleportToLocation()); break;
					case 0x29: create(new Packet.Server.TargetSelected()); break;
					case 0x2A: create(new Packet.Server.TargetUnselected()); break;
					case 0x2B: create(new Packet.Server.AutoAttackStart()); break;
					case 0x2C: create(new Packet.Server.AutoAttackStop()); break;
					case 0x2D: create(new Packet.Server.SocialAction()); break;
					case 0x2E: create(new Packet.Server.ChangeMoveType()); break;
					case 0x2F: create(new Packet.Server.ChangeWaitType()); break;
					case 0x32: create(new Packet.Server.AskJoinPledge()); break;
					case 0x39: create(new Packet.Server.AskJoinParty()); break;
					case 0x45: create(new Packet.Server.ShortcutInit()); break;
					case 0x47: create(new Packet.Server.StopMove()); break;
					case 0x48: create(new Packet.Server.MagicSkillUser()); break;
					case 0x49: create(new Packet.Server.MagicSkillCanceld()); break;
					case 0x4A: create(new Packet.Server.CreatureSay()); break;
					case 0x4E: create(new Packet.Server.PartySmallWindowAll()); break;
					case 0x4F: create(new Packet.Server.PartySmallWindowAdd()); break;
					case 0x50: create(new Packet.Server.PartySmallWindowDeleteAll()); break;
					case 0x51: create(new Packet.Server.PartySmallWindowDelete()); break;
					case 0x52: create(new Packet.Server.PartySmallWindowUpdate()); break;
					case 0x53: create(new Packet.Server.PledgeShowMemberListAll()); break;
					case 0x58: create(new Packet.Server.SkillList()); break;
					case 0x5F: create(new Packet.Server.RestartResponse()); break;
					case 0x60: create(new Packet.Server.MoveToPawn()); break;
					case 0x61: create(new Packet.Server.ValidateLocation()); break;
					case 0x64: create(new Packet.Server.SystemMessage()); break;
					case 0x6C: create(new Packet.Server.PledgeCrest()); break;
					case 0x73: create(new Packet.Server.ValidateLocationInVehicle()); break;
					case 0x76: create(new Packet.Server.MagicSkillLaunched()); break;
					case 0x7D: create(new Packet.Server.AskJoinFriend()); break;
					case 0x7E: create(new Packet.Server.Logout()); break;
					case 0x83: create(new Packet.Server.PledgeInfo()); break;
					case 0x88: create(new Packet.Server.PledgeShowInfoUpdate()); break;
					case 0xA6: create(new Packet.Server.MyTargetSelected()); break;
					case 0xA7: create(new Packet.Server.PartyMemberPosition()); break;
					case 0xA8: create(new Packet.Server.AskJoinAlly()); break;
					case 0xAE: create(new Packet.Server.AllyCrest()); break;
					case 0xE4: create(new Packet.Server.HennaInfo()); break;
					case 0xF8: create(new Packet.Server.SignsSky()); break;
					case 0xF9: create(new Packet.Server.GameGuardVerfy()); break;
					case 0xFD: create(new Packet.Server.FriendRecvMsg()); break;
					case 0x6d: //SetupGauge
					case 0x7f: //MagicEffectIcons
					case 0x80: //QuestList
					case 0xe7: //SendMacroList
					case 0xfe: //Ex*
					case 0xee: // ?
					default: Logger.Warn("Unknown packet " + Packet.Utils.prettyHex(buffer)); break;
				}

			if (packet != null)
			{
				lock (ReadQueue)
					ReadQueue.Enqueue(packet);

				if (Readed != null)
					Readed(this, packet);
			}
		}

		public void Dispose()
		{
			Stop();

			if (Connection != null)
				Connection.Close();
		}
	}
}
