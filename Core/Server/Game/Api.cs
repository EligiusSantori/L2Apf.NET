using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace L2Apf.Server.Game
{
	// Todo partial class?
	public sealed class Api : IDisposable
	{
		private Thread GameThread;
		private Thread MoveThread;
		private Network Network;
		private NLog.Logger Logger;

		public Model.World World { get; private set; }
		public State State { get; private set; }
		public bool IsBackground
		{
			get { return GameThread.IsBackground; }
			set { GameThread.IsBackground = value; }
		}

		public int MoveInterval = 0;

		private object IdleLock = new object();
		private bool IsIdle = false;

		public event ConnectedHandler Connected;
		public event RestartedHandler Restarted;
		public event EmptyHandler EnterWorld;
		public event EmptyHandler LoggedOut;
		public event EmptyHandler ActionFailed;
		public event CreatureHandler Revive;
		public event DieHandler Die;
		public event SocialActionHandler UsedSocialAction;
		public event SkillListHandler SkillList;
		public event CreatureHandler Teleported;
		public event TargetChangedHandler TargetChanged;
		public event QuestionHandler QuestionAsked;
		public event CreatureHandler CreatureUpdate;
		public event CreatureHandler CreatureInfo;
		public event ObjectHandler ObjectRemoved;
		public event PartyInfoHandler PartyInfo;
		public event PartyModifyHandler PartyAppend;
		public event PartyModifyHandler PartyUpdate;
		public event PartyModifyHandler PartyDelete;
		public event ClanInfoHandler ClanInfo;
		public event ClanUpdateHandler ClanUpdate;
		public event HennaInfoHandler HennaInfo;
		public event ItemListHandler InventoryRefresh;
		public event ItemListHandler InventoryUpdate;
		public event ItemHandler ItemInfo;
		public event ChangeTimeHandler ChangeTime;
		public event ChatMessageHandler ChatMessage;
		public event SystemMessageHandler SystemMessage;
		public event CreatureHandler Attack;
		public event PickupHandler Pickup;
		public event CreatureHandler StartMoving;
		public event CreatureHandler FinishMoving;
		public event CreatureHandler SkillStarted;
		public event CreatureHandler SkillLaunched;
		public event CreatureHandler SkillCanceled;
		public event SkillReuseHandler SkillReused;

		public delegate void EmptyHandler();
		public delegate void ObjectHandler(Model.Object obj);
		public delegate void ItemHandler(Model.Item item);
		public delegate void CreatureHandler(Model.Creature creature); // Todo changed args with old & new
		public delegate void ConnectedHandler(IEnumerable<Model.Player> characters);
		public delegate void RestartedHandler();
		public delegate void DieHandler(Model.Creature creature, Lineage.ReturnPoint? points);
		public delegate void SocialActionHandler(Model.Creature creature, Lineage.SocialAction action);
		public delegate void SkillListHandler(IEnumerable<Model.Skill> skills);
		public delegate void ItemListHandler(IEnumerable<Tuple<Model.Item, ItemUpdate>> items);
		public delegate void TargetChangedHandler(Model.Creature creature, Model.Creature target); // ToDo: newtarget,oldtarget?
		public delegate void QuestionHandler(Lineage.Question question, string name, string value);
		public delegate void PickupHandler(Model.Item item, Model.Creature creature, Library.Point position);
		public delegate void PartyInfoHandler(Model.Party party);
		public delegate void PartyModifyHandler(Model.Party party, Model.Character member);
		public delegate void ClanInfoHandler(Model.Clan clan, Model.Ally ally = null);
		public delegate void ClanUpdateHandler(Model.Clan clan);
		public delegate void HennaInfoHandler(int[] symbols);
		public delegate void ChangeTimeHandler(Lineage.DayTime? time, Lineage.SignsSky? sky);
		public delegate void ChatMessageHandler(Lineage.Channel channel, string message, string from, Model.Creature author);
		public delegate void SystemMessageHandler(int messageId, object[] arguments);
		public delegate void SkillReuseHandler(Model.Skill skill);


		public Api()
		{
			Logger = NLog.LogManager.GetCurrentClassLogger();

			GameThread = new Thread(GameMainThread);
			MoveThread = new Thread(MoveStuffThread)
			{
				IsBackground = true
			};

			Network = new Network();
			Network.Readed += (Network sender, Packet.Packet packet) =>
			{
				lock (IdleLock)
					if (IsIdle)
					{
						IsIdle = false;
						Monitor.Pulse(IdleLock);
					}
			};

			World = new Model.World();
		}

		private void GameMainThread()
		{
			Packet.Packet packet = null;
			while (State >= State.Begun)
			{
				lock (IdleLock)
				{
					packet = Network.Read();
					if (packet == null)
					{
						IsIdle = true;
						Monitor.Wait(IdleLock);
						continue;
					}
				}

				if (packet is Packet.Server.KeyPacket)
				{
					var _packet = packet as Packet.Server.KeyPacket;
					Network.Crypt(_packet.Key);
				}
				else if (packet is Packet.Server.CharSelectInfo)
				{
					var _packet = packet as Packet.Server.CharSelectInfo;
					State = State.Lobby;

					if (Connected != null && _packet.List != null)
						Connected(_packet.List);
				}
				else if (packet is Packet.Server.CharMoveToLocation)
				{
					var _packet = packet as Packet.Server.CharMoveToLocation;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Destination = _packet.Destination;
						creature.MoveTarget = 0;
						if (_packet.Position == _packet.Destination)
						{
							creature.IsMoving = false;
							if (FinishMoving != null)
								FinishMoving(creature);
						}
						else
						{
							creature.IsMoving = true;
							creature.LastMoveTime = System.DateTime.Now;
							if (StartMoving != null)
								StartMoving(creature);
						}
					}
					else
					{
						Logger.Warn("Creature not found");
					}
				}
				else if (packet is Packet.Server.CharInfo)
				{
					var _packet = packet as Packet.Server.CharInfo;
					var character = World[_packet.ObjectId] as Model.OtherPlayer;
					if (character == null)
						World.Add(character = new Model.OtherPlayer()
						{
							ObjectId = _packet.ObjectId
						});

					// mapping Packet => CharInfo
					character.Position = _packet.Position;
					character.Heading = _packet.Heading;
					character.ObjectId = _packet.ObjectId;
					character.Name = _packet.Name;
					character.Race = _packet.Race;
					character.Gender = _packet.Gender;
					character.ClassId = _packet.Class;
					character.Equipment = new Model.Equipment()
					{
						Underwear = _packet.Underwear,
						Head = _packet.Head,
						RightHand = _packet.RightHand,
						LeftHand = _packet.LeftHand,
						Gloves = _packet.Gloves,
						Chest = _packet.Chest,
						Legs = _packet.Legs,
						Feet = _packet.Feet,
						Back = _packet.Back,
						BothHand = _packet.BothHand,
						Hair = _packet.Hair,
					};
					character.InPvP = _packet.IsPvP;
					character.Karma = _packet.Karma;
					character.MAtkSpd = _packet.MAttackSpeed;
					character.PAtkSpd = _packet.PAttackSpeed;
					character.RunSpd = _packet.RunSpeed;
					character.WalkSpd = _packet.WalkSpeed;
					character.SwimRunSpd = _packet.SwimRunSpeed;
					character.SwimWalkSpd = _packet.SwimWalkSpeed;
					character.FlRunSpd = _packet.FlRunSpeed;
					character.FlWalkSpd = _packet.FlWalkSpeed;
					character.FlyRunSpd = _packet.FlyRunSpeed;
					character.FlyWalkSpd = _packet.FlyWalkSpeed;
					character.MoveSpdMult = _packet.MoveSpeedMult;
					character.AtkSpdMult = _packet.AttackSpeedMult;
					character.CollisionRadius = _packet.CollisionRadius;
					character.CollisionHeight = _packet.CollisionHeight;
					character.HairStyle = _packet.HairSytle;
					character.HairColor = _packet.HairColor;
					character.FaceType = _packet.FaceType;
					character.Title = _packet.Title;
					character.ClanId = _packet.ClanId;
					character.ClanCrestId = _packet.ClanCrestId;
					character.AllyId = _packet.AllyId;
					character.AllyCrestId = _packet.AllyCrestId;
					//ch_inf.SiegeFlags = cip.SiegeFlags;
					character.IsSitting = !_packet.IsStanding;
					character.IsRunning = _packet.IsRunning;
					character.IsInCombat = _packet.IsInCombat;
					character.IsAlikeDead = _packet.IsAlikeDead;
					character.IsInvisible = _packet.IsInvisible;
					character.MountType = _packet.MountType;
					character.PrivateStore = _packet.PrivateStore;
					character.Cubics = _packet.Cubics.Cast<int>().ToList<int>();
					character.IsFindParty = _packet.IsFindParty;
					character.AbnormalEffects = _packet.AbnormalEffects;
					character.RecLeft = _packet.RecommendLeft;
					character.RecAmount = _packet.RecommendAmount;
					character.MaxCp = _packet.MaxCp;
					character.Cp = _packet.Cp;
					character.Enchant = _packet.EnchantAmount;
					character.TeamCircle = _packet.TeamCircle;
					character.LargeCrestId = _packet.ClanLargeCrestId;
					character.IsHeroIcon = _packet.IsHeroIcon;
					character.IsHeroGlow = _packet.IsHeroGlow;
					character.IsFishing = _packet.IsFishing;
					character.Fish = _packet.Fish;
					character.NameColor = _packet.NameColor;

					if (CreatureInfo != null)
						CreatureInfo(character);
				}
				else if (packet is Packet.Server.UserInfo)
				{
					var _packet = packet as Packet.Server.UserInfo;
					Model.Player me = World.Me;
					if (me == null)
						World.Add(World.Me = me = new Model.Player());

					// mapping Packet => Model.Player
					me.Name = _packet.Name;
					me.Title = _packet.Title;
					me.Heading = _packet.Heading;
					me.ObjectId = _packet.ObjectId;
					me.NameColor = _packet.NameColor;
					me.Race = _packet.Race;
					me.ClassId = _packet.ClassId;
					me.BaseClassId = _packet.BaseClassId;
					me.Gender = _packet.Gender;
					me.MountType = _packet.MountType;
					me.PrivateStore = _packet.PrivateStore;
					me.ClanId = _packet.ClanId;
					me.ClanCrestId = _packet.ClanCrestId;
					me.AllyId = _packet.AllyId;
					me.AllyCrestId = _packet.AllyCrestId;
					me.LargeCrestId = _packet.LargeCrestId;
					me.MoveSpdMult = _packet.MoveSpdMult;
					me.AtkSpdMult = _packet.AtkSpdMult;
					me.CollisionRadius = _packet.CollisionRadius;
					me.CollisionHeight = _packet.CollisionHeight;
					me.HairStyle = _packet.HairStyle;
					me.HairColor = _packet.HairColor;
					me.FaceType = _packet.FaceType;
					me.Level = _packet.Level;
					me.MaxHp = _packet.MaxHp;
					me.Hp = _packet.Hp;
					me.MaxMp = _packet.MaxMp;
					me.Mp = _packet.Mp;
					me.MaxCp = _packet.MaxCp;
					me.Cp = _packet.Cp;
					me.Xp = _packet.Xp;
					me.Sp = _packet.Sp;
					me.Load = _packet.Load;
					me.MaxLoad = _packet.MaxLoad;
					me.PvP = _packet.PvP;
					me.Pk = _packet.Pk;
					me.PAtk = _packet.PAtk;
					me.PAtkSpd = _packet.PAtkSpd;
					me.PDef = _packet.PDef;
					me.Evasion = _packet.Evasion;
					me.Accuracy = _packet.Accuracy;
					me.Focus = _packet.Focus;
					me.MAtk = _packet.MAtk;
					me.MAtkSpd = _packet.MAtkSpd;
					me.MDef = _packet.MDef;
					me.Karma = _packet.Karma;
					me.RunSpd = _packet.RunSpd;
					me.WalkSpd = _packet.WalkSpd;
					me.SwimRunSpd = _packet.SwimRunSpd;
					me.SwimWalkSpd = _packet.SwimWalkSpd;
					me.FlRunSpd = _packet.FlRunSpd;
					me.FlWalkSpd = _packet.FlWalkSpd;
					me.FlyRunSpd = _packet.FlyRunSpd;
					me.FlyWalkSpd = _packet.FlyWalkSpd;
					me.InPvP = _packet.InPvP;
					me.IsFishing = _packet.IsFishing;
					me.IsHeroIcon = _packet.IsHeroIcon;
					me.IsHeroGlow = _packet.IsHeroGlow;
					me.IsFindParty = _packet.IsFindParty;
					me.IsClanLeader = _packet.IsClanLeader;
					me.HasDwarfCraft = _packet.HasDwarfCraft;
					me.Position = _packet.Position;
					me.Fish = _packet.Fish;
					me.INT = _packet.INT;
					me.STR = _packet.STR;
					me.CON = _packet.CON;
					me.MEN = _packet.MEN;
					me.DEX = _packet.DEX;
					me.WIT = _packet.WIT;

					((Model.Character)me).Equipment = new Model.Equipment()
					{
						Underwear = _packet.Underwear.ItemId,
						RightEaring = _packet.RightEaring.ItemId,
						LeftEaring = _packet.LeftEaring.ItemId,
						Neck = _packet.Neck.ItemId,
						RightFinger = _packet.RightFinger.ItemId,
						LeftFinger = _packet.LeftFinger.ItemId,
						Head = _packet.Head.ItemId,
						RightHand = _packet.RightHand.ItemId,
						LeftHand = _packet.LeftHand.ItemId,
						Gloves = _packet.Gloves.ItemId,
						Chest = _packet.Chest.ItemId,
						Legs = _packet.Legs.ItemId,
						Feet = _packet.Feet.ItemId,
						Back = _packet.Back.ItemId,
						BothHand = _packet.BothHand.ItemId,
						Hair = _packet.Hair.ItemId,
					};

					me.Equipment = new Model.Equipment()
					{
						Underwear = _packet.Underwear.ObjectId,
						RightEaring = _packet.RightEaring.ObjectId,
						LeftEaring = _packet.LeftEaring.ObjectId,
						Neck = _packet.Neck.ObjectId,
						RightFinger = _packet.RightFinger.ObjectId,
						LeftFinger = _packet.LeftFinger.ObjectId,
						Head = _packet.Head.ObjectId,
						RightHand = _packet.RightHand.ObjectId,
						LeftHand = _packet.LeftHand.ObjectId,
						Gloves = _packet.Gloves.ObjectId,
						Chest = _packet.Chest.ObjectId,
						Legs = _packet.Legs.ObjectId,
						Feet = _packet.Feet.ObjectId,
						Back = _packet.Back.ObjectId,
						BothHand = _packet.BothHand.ObjectId,
						Hair = _packet.Hair.ObjectId,
					};

					me.RecLeft = _packet.RecLeft;
					me.RecAmount = _packet.RecAmount;
					me.InventoryLimit = _packet.InventoryLimit;
					me.SpecialEffects = _packet.SpecialEffects;
					me.AbnormalEffects = _packet.AbnormalEffects;
					me.AccessLevel = _packet.AccessLevel;
					me.Enchant = _packet.Enchant;
					me.TeamCircle = _packet.TeamCircle;
					me.Cubics = _packet.Cubics;

					if (!World.Contains(World.Me.ObjectId))
						World.Add(World.Me);

					// Run event
					if (CreatureInfo != null)
						CreatureInfo(World.Me);
				}
				else if (packet is Packet.Server.Attack)
				{
					var _packet = packet as Packet.Server.Attack;
					int attackerId = _packet.AttackerId;
					int targetId = _packet.Hits[0].TargetId;

					//lets check who is attacking what and set things as needed
					var creature = World[attackerId] as Model.Creature;
					if (creature != null)
					{
						if (creature.TargetId == 0)
							creature.TargetId = targetId;
						creature.IsInCombat = true;

						if (Attack != null)
							Attack(creature);
					}
				}
				else if (packet is Packet.Server.Die)
				{
					var _packet = packet as Packet.Server.Die;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Destination = creature.Position;
						creature.IsAlikeDead = true;
						creature.IsMoving = false;

						var npc = creature as Model.Npc;
						if (npc != null)
							npc.IsSpoiled = _packet.IsSpoiled;

						if (Die != null)
							Die(creature, creature == World.Me ?
								(Lineage.ReturnPoint?)_packet.Points : null);
					}
				}
				else if (packet is Packet.Server.Revive)
				{
					var _packet = packet as Packet.Server.Revive;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.IsAlikeDead = false;

						if (Revive != null)
							Revive(creature);
					}
				}
				else if (packet is Packet.Server.SpawnItem)
				{
					var _packet = packet as Packet.Server.SpawnItem;
					var item = World[_packet.ObjectId] as Model.Item;
					if (item == null)
						World.Add(item = new Model.Item()
						{
							ObjectId = _packet.ObjectId
						});

					item.ItemId = _packet.ItemId;
					item.Count = _packet.Count;
					item.Position = _packet.Position;
					item.IsStackable = _packet.IsStackable;

					if (ItemInfo != null)
						ItemInfo(item);
				}
				else if (packet is Packet.Server.DropItem)
				{
					var _packet = packet as Packet.Server.DropItem;
					var item = World[_packet.ObjectId] as Model.Item;
					if (item == null)
						World.Add(item = new Model.Item()
						{
							ObjectId = _packet.ObjectId
						});

					item.DroppedBy = _packet.DroppedBy;
					item.ItemId = _packet.ItemId;
					item.Position = _packet.Position;
					item.IsStackable =_packet.IsStackable;
					item.Count = _packet.Count;

					if (ItemInfo != null)
						ItemInfo(item);
				}
				else if (packet is Packet.Server.GetItem)
				{
					var _packet = packet as Packet.Server.GetItem;
					var item = World[_packet.ObjectId] as Model.Item;
					var creature = World[_packet.PlayerId] as Model.Creature;

					if (item != null)
					{
						if (Pickup != null)
							Pickup(item, creature, _packet.Position);

						if (creature != World.Me)
							World.Remove(item);
					}
				}
				else if (packet is Packet.Server.StatusUpdate)
				{
					var _packet = packet as Packet.Server.StatusUpdate;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						var me = World.Me;

						foreach (var pair in _packet.Attributes)
						{
							int key = pair.Key;
							int value = pair.Value;
							switch (key) // ToDo enum
							{
								case 0x01: creature.Level = value; break;
								case 0x02: me.Xp = value; break;
								case 0x03: me.STR = value; break;
								case 0x04: me.DEX = value; break;
								case 0x05: me.CON = value; break;
								case 0x06: me.INT = value; break;
								case 0x07: me.WIT = value; break;
								case 0x08: me.MEN = value; break;
								case 0x09: creature.Hp = value; break;
								case 0x0A: creature.MaxHp = value; break;
								case 0x0B: creature.Mp = value; break;
								case 0x0C: creature.MaxMp = value; break;
								case 0x0D: me.Sp = value; break;
								case 0x0E: me.Load = value; break;
								case 0x0F: me.MaxLoad = value; break;
								case 0x11: me.PAtk = value; break;
								case 0x12: creature.PAtkSpd = value; break;
								case 0x13: me.PDef = value; break;
								case 0x14: me.Evasion = value; break;
								case 0x15: me.Accuracy = value; break;
								case 0x16: me.Focus = value; break;
								case 0x17: me.MAtk = value; break;
								case 0x18: creature.MAtkSpd = value; break;
								case 0x19: me.MDef = value; break;
								case 0x1A: ((Model.Character)creature).InPvP = value != 0; break;
								case 0x1B: ((Model.Character)creature).Karma = value; break;
								case 0x21: creature.Cp = value; break;
								case 0x22: creature.MaxCp = value; break;
							}
						}

						if (CreatureUpdate != null)
							CreatureUpdate(creature);
					}
				}
				else if (packet is Packet.Server.NpcHtmlMessage)
				{
					var _packet = packet as Packet.Server.NpcHtmlMessage;
					//NPC_Chat.Npc_Chat(hmp.MessageId, hmp.Content);
				}
				else if (packet is Packet.Server.DeleteObject)
				{
					var _packet = packet as Packet.Server.DeleteObject;
					Model.Object obj = World[_packet.ObjectId];
					if (obj != null)
					{
						World.Remove(obj);

						if (ObjectRemoved != null)
							ObjectRemoved(obj);
					}
					else
						Logger.Warn("Object Removed, but not found");
				}
				else if (packet is Packet.Server.CharSelected)
				{
					var _packet = packet as Packet.Server.CharSelected;
					Model.Player me = World.Me;
					if (me == null)
						World.Add(World.Me = me = new Model.Player());

					// mapping Packet => Model.Player
					me.Name = _packet.Name;
					me.Title = _packet.Title;
					me.ObjectId = _packet.ObjectId;
					me.SessionId = _packet.SessionId;
					me.Position = _packet.Position;
					me.Gender = _packet.Gender;
					me.Race = _packet.Race;
					me.BaseClassId = _packet.BaseClassId;
					me.Hp = _packet.Hp;
					me.Mp = _packet.Mp;
					me.Sp = _packet.Sp;
					me.Xp = _packet.Xp;
					me.Level = _packet.Level;
					me.Karma = _packet.Karma;
					me.INT = _packet.INT;
					me.STR = _packet.STR;
					me.CON = _packet.CON;
					me.MEN = _packet.MEN;
					me.DEX = _packet.DEX;
					me.WIT = _packet.WIT;

					((Model.Character)me).Equipment = new Model.Equipment()
					{
						Underwear = _packet.Underwear.ItemId,
						RightEaring = _packet.RightEaring.ItemId,
						LeftEaring = _packet.LeftEaring.ItemId,
						Neck = _packet.Neck.ItemId,
						RightFinger = _packet.RightFinger.ItemId,
						LeftFinger = _packet.LeftFinger.ItemId,
						Head = _packet.Head.ItemId,
						RightHand = _packet.RightHand.ItemId,
						LeftHand = _packet.LeftHand.ItemId,
						Gloves = _packet.Gloves.ItemId,
						Chest = _packet.Chest.ItemId,
						Legs = _packet.Legs.ItemId,
						Feet = _packet.Feet.ItemId,
						Back = _packet.Back.ItemId,
						BothHand = _packet.BothHand.ItemId,
						Hair = _packet.Hair.ItemId
					};

					me.Equipment.Underwear = _packet.Underwear.ObjectId;
					me.Equipment.RightEaring = _packet.RightEaring.ObjectId;
					me.Equipment.LeftEaring = _packet.LeftEaring.ObjectId;
					me.Equipment.Neck = _packet.Neck.ObjectId;
					me.Equipment.RightFinger = _packet.RightFinger.ObjectId;
					me.Equipment.LeftFinger = _packet.LeftFinger.ObjectId;
					me.Equipment.Head = _packet.Head.ObjectId;
					me.Equipment.RightHand = _packet.RightHand.ObjectId;
					me.Equipment.LeftHand = _packet.LeftHand.ObjectId;
					me.Equipment.Gloves = _packet.Gloves.ObjectId;
					me.Equipment.Chest = _packet.Chest.ObjectId;
					me.Equipment.Legs = _packet.Legs.ObjectId;
					me.Equipment.Feet = _packet.Feet.ObjectId;
					me.Equipment.Back = _packet.Back.ObjectId;
					me.Equipment.BothHand = _packet.BothHand.ObjectId;
					me.Equipment.Hair = _packet.Hair.ObjectId;
					me.ClanId = _packet.ClanId;
					me.IsActive = _packet.IsActive;

					State = State.World;

					MoveThread.Start();

					RefreshManorList();
					RefreshQuestList();

					Network.Send(new Packet.Client.EnterWorld());

					RefreshSkillList();

					if (EnterWorld != null)
						EnterWorld();
				}
				else if (packet is Packet.Server.NpcInfo)
				{
					var _packet = packet as Packet.Server.NpcInfo;
					var npc = World[_packet.ObjectId] as Model.Npc;
					if (npc == null)
						World.Add(npc = new Model.Npc()
						{
							ObjectId = _packet.ObjectId
						});

					npc.NpcId = _packet.NpcId;
					npc.ObjectId = _packet.ObjectId;
					npc.Heading = _packet.Heading;
					npc.Name = _packet.Name;
					npc.Title = _packet.Title;
					npc.Position = _packet.Position;

					npc.MAtkSpd = _packet.MAtkSpd;
					npc.PAtkSpd = _packet.PAtkSpd;

					npc.RunSpd = _packet.RunSpd;
					npc.WalkSpd = _packet.WalkSpd;
					npc.SwimRunSpd = _packet.SwimRunSpd;
					npc.SwimWalkSpd = _packet.SwimWalkSpd;
					npc.FlRunSpd = _packet.FlRunSpd;
					npc.FlWalkSpd = _packet.FlWalkSpd;
					npc.FlyRunSpd = _packet.FlyRunSpd;
					npc.FlyWalkSpd = _packet.FlyWalkSpd;

					npc.RightHand = _packet.RightHand;
					npc.BothHand = _packet.BothHand;
					npc.LeftHand = _packet.LeftHand;

					npc.MoveSpdMult = _packet.MoveSpdMult;
					npc.AtkSpdMult = _packet.AtkSpdMult;
					npc.CollisionRadius = _packet.CollisionRadius;
					npc.CollisionHeight = _packet.CollisionHeight;

					npc.IsAttackable = _packet.IsAttackable;
					npc.IsSummoned = _packet.IsSummoned;
					npc.IsAlikeDead = _packet.IsAlikeDead;
					npc.IsInCombat = _packet.IsInCombat;
					npc.IsRunning = _packet.IsRunning;
					npc.IsShowName = _packet.IsShowName;

					if (CreatureInfo != null)
						CreatureInfo(npc);
				}
				else if (packet is Packet.Server.ItemList)
				{
					var _packet = packet as Packet.Server.ItemList;
					var items = new List<Tuple<Model.Item, ItemUpdate>>();
					foreach (var _item in _packet.Items)
					{
						var item = World[_item.ObjectId] as Model.Item;
						if (item == null)
							World.Add(item = new Model.Item()
							{
								ObjectId = _item.ObjectId
							});

						item.ObjectId = _item.ObjectId;
						item.ItemId = _item.ItemId;
						item.Count = _item.Count;
						item.Slot = _item.Slot;
						item.Enchant = _item.Enchant;
						item.IsEquipped = _item.IsEquipped;
						item.Type1 = _item.Type1;
						item.Type2 = _item.Type2;
						item.Type3 = _item.Type3;
						item.Type4 = _item.Type4;

						items.Add(new Tuple<Model.Item, ItemUpdate>(item, ItemUpdate.Append));
					}

					if (InventoryRefresh != null)
						InventoryRefresh(items);
				}
				else if (packet is Packet.Server.Sunrise)
				{
					var _packet = packet as Packet.Server.Sunrise;
					if (ChangeTime != null)
						ChangeTime(Lineage.DayTime.Night, null);
				}
				else if (packet is Packet.Server.Sunset)
				{
					var _packet = packet as Packet.Server.Sunset;
					if (ChangeTime != null)
						ChangeTime(Lineage.DayTime.Day, null);
				}
				else if (packet is Packet.Server.ActionFailed)
				{
					if (ActionFailed != null)
						ActionFailed();
				}
				else if (packet is Packet.Server.InventoryUpdate)
				{
					var _packet = packet as Packet.Server.InventoryUpdate;
					var items = new List<Tuple<Model.Item, ItemUpdate>>();
					foreach (var _item in _packet.Items)
					{
						var item = World[_item.ObjectId] as Model.Item;
						if (_item.Change == (int)ItemUpdate.Append ||
							_item.Change == (int)ItemUpdate.Modify)
						{
							if (item == null)
								World.Add(item = new Model.Item()
								{
									ObjectId = _item.ObjectId
								});

							item.ItemId = _item.ItemId;
							item.Count = _item.Count;
							item.Slot = _item.Slot;
							item.Enchant = _item.Enchant;
							item.IsEquipped = _item.IsEquipped;
							item.Type1 = _item.Type1;
							item.Type2 = _item.Type2;
							item.Type3 = _item.Type3;
							item.Type4 = _item.Type4;
						}

						// Если Drop, удалять объект нельзя
						// Тут ничего не делаем т.к. нахождение в инвентаре отслеживается по position, а та уже установлена

						items.Add(new Tuple<Model.Item, ItemUpdate>(item, (ItemUpdate)_item.Change));
					}

					if (InventoryUpdate != null)
						InventoryUpdate(items);
				}
				else if (packet is Packet.Server.TeleportToLocation)
				{
					var _packet = packet as Packet.Server.TeleportToLocation;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Position = _packet.Position;
						if (creature.ObjectId == World.Me.ObjectId)
						{
							var me = World.Me;
							lock (World)
							{
								World.Clear();
								World.Add(me);
							}

							Network.Send(new Packet.Client.ValidatePosition()
							{
								Position = World.Me.Position,
								Angle = World.Me.Heading
							});

							Network.Send(new Packet.Client.Appearing());
						}

						if (Teleported != null)
							Teleported(creature);
					}
				}
				else if (packet is Packet.Server.TargetSelected)
				{
					var _packet = packet as Packet.Server.TargetSelected;
					var creature = World[_packet.ObjectId] as Model.Creature;
					var target = World[_packet.TargetId] as Model.Creature;

					if (creature != null && target != null)
						if (creature.TargetId != target.ObjectId)
						{
							creature.TargetId = target.ObjectId;

							if (TargetChanged != null)
								TargetChanged(creature, target);
						}
				}
				else if (packet is Packet.Server.TargetUnselected)
				{
					var _packet = packet as Packet.Server.TargetUnselected;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.TargetId = 0; // ToDo Target = null?

						if (TargetChanged != null)
							TargetChanged(creature, null);
					}
				}
				else if (packet is Packet.Server.AutoAttackStart)
				{
					var _packet = packet as Packet.Server.AutoAttackStart;
					var creature = World[_packet.TargetId] as Model.Creature;
					if (creature != null)
						creature.IsInCombat = true;
				}
				else if (packet is Packet.Server.AutoAttackStop)
				{
					var _packet = packet as Packet.Server.AutoAttackStop;
					var creature = World[_packet.TargetId] as Model.Creature;
					if (creature != null)
						creature.IsInCombat = false;
				}
				else if (packet is Packet.Server.SocialAction)
				{
					var _packet = packet as Packet.Server.SocialAction;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null && UsedSocialAction != null)
						UsedSocialAction(creature, _packet.Action);
				}
				else if (packet is Packet.Server.ChangeMoveType)
				{
					var _packet = packet as Packet.Server.ChangeMoveType;
					var character = World[_packet.ObjectId] as Model.Character;
					if (character != null)
						character.IsRunning = _packet.MoveType == Lineage.MoveType.Run;
				}
				else if (packet is Packet.Server.ChangeWaitType)
				{
					var _packet = packet as Packet.Server.ChangeWaitType;
					var character = World[_packet.ObjectId] as Model.Character;
					if (character != null)
						character.IsSitting = _packet.WaitType == Lineage.WaitType.Sit;
				}
				else if (packet is Packet.Server.AskJoinPledge)
				{
					var _packet = packet as Packet.Server.AskJoinPledge;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (QuestionAsked != null && creature != null)
						QuestionAsked(Lineage.Question.JoinClan, creature.Name, _packet.ClanName);
					else if (creature == null)
						Logger.Warn("Join a clan question author is not found.");
				}
				else if (packet is Packet.Server.AskJoinParty)
				{
					var _packet = packet as Packet.Server.AskJoinParty;
					if (QuestionAsked != null)
						QuestionAsked(Lineage.Question.JoinParty, _packet.LeaderName, _packet.PartyLoot.ToString());
				}
				else if (packet is Packet.Server.ShortcutInit)
				{
					var _packet = packet as Packet.Server.ShortcutInit;
					foreach (Model.Shortcut shortcut in _packet.List)
						World.Shortcut[shortcut.Page, shortcut.Slot] = shortcut;
				}
				else if (packet is Packet.Server.StopMove)
				{
					var _packet = packet as Packet.Server.StopMove;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Position = _packet.Position;
						creature.Heading = _packet.Heading;
						creature.IsMoving = false;
					}
				}
				else if (packet is Packet.Server.MagicSkillUser)
				{
					var _packet = packet as Packet.Server.MagicSkillUser;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Casting = new Model.Casting()
						{
							SkillId = _packet.SkillId,
							SkillLevel = _packet.SkillLevel,
							Start = DateTime.Now,
							Length = _packet.CastTime
						};

						if (creature == World.Me)
						{
							var skills = World.Me.Skills;
							lock (skills)
								if (skills.ContainsKey(_packet.SkillId))
								{
									var skill = skills[_packet.SkillId];

									if (_packet.ReuseDelay > TimeSpan.Zero)
									{
										skill.IsReady = false;

										if (skill.Reuse != null)
											skill.Reuse.Dispose();

										skill.Reuse = new System.Timers.Timer(_packet.ReuseDelay.TotalMilliseconds) { AutoReset = false };
										skill.Reuse.Elapsed += (object sender, System.Timers.ElapsedEventArgs args) =>
										{
											skill.IsReady = true;
											if (SkillReused != null)
												SkillReused(skill);
										};

										skill.Reuse.Start();
									}
								}
								else
									Logger.Warn(string.Format("Skill #{0} not found, but got in MagicSkillUser packet", _packet.SkillId));
						}

						if (SkillStarted != null)
							SkillStarted(creature);
					}
				}
				else if (packet is Packet.Server.MagicSkillCanceld)
				{
					var _packet = packet as Packet.Server.MagicSkillCanceld;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						if (SkillCanceled != null)
							SkillCanceled(creature);

						creature.Casting = null;
					}
				}
				else if (packet is Packet.Server.CreatureSay)
				{
					var _packet = packet as Packet.Server.CreatureSay;
					if (ChatMessage != null)
					{
						Model.Creature author = World[_packet.ObjectId] as Model.Creature;
						ChatMessage(_packet.Type, _packet.Message, _packet.Author, author);
					}
				}
				else if (packet is Packet.Server.PartySmallWindowAll)
				{
					var _packet = packet as Packet.Server.PartySmallWindowAll;
					var party = World.Party = new Model.Party()
					{
						Loot = _packet.Loot
					};

					foreach (var _member in _packet.Members)
					{
						var member = World[_member.ObjectId] as Model.Character;
						if (member == null)
							World.Add(member = new Model.OtherPlayer()
							{
								ObjectId = _member.ObjectId
							});

						lock (party.Members)
							party.Members.Add(member);
						if (member.ObjectId == _packet.LeaderId)
							party.Leader = member;

						member.Name = _member.Name;
						member.Cp = _member.Cp;
						member.MaxCp = _member.MaxCp;
						member.Hp = _member.Hp;
						member.MaxHp = _member.MaxHp;
						member.Mp = _member.Mp;
						member.MaxMp = _member.MaxMp;
						member.Level = _member.Level;
						member.ClassId = _member.Class;
					}

					lock (party.Members)
						party.Members.Add(World.Me);

					if (PartyInfo != null)
						PartyInfo(null);
				}
				else if (packet is Packet.Server.PartySmallWindowAdd)
				{
					var _packet = packet as Packet.Server.PartySmallWindowAdd;
					var member = World[_packet.ObjectId] as Model.Character;
					if (member == null)
						World.Add(member = new Model.OtherPlayer()
						{
							ObjectId = _packet.ObjectId
						});

					var party = World.Party;
					lock (party.Members)
						party.Members.Add(member);

					member.Name = _packet.Name;
					member.Cp = _packet.Cp;
					member.MaxCp = _packet.MaxCp;
					member.Hp = _packet.Hp;
					member.MaxHp = _packet.MaxHp;
					member.Mp = _packet.Mp;
					member.MaxMp = _packet.MaxMp;
					member.Level = _packet.Level;
					member.ClassId = _packet.Class;

					if (PartyAppend != null)
						PartyAppend(party, member);
				}
				else if (packet is Packet.Server.PartySmallWindowDeleteAll)
				{
					var _packet = packet as Packet.Server.PartySmallWindowDeleteAll;
					World.Party = null;

					if (PartyInfo != null)
						PartyInfo(null);
				}
				else if (packet is Packet.Server.PartySmallWindowDelete)
				{
					var _packet = packet as Packet.Server.PartySmallWindowDelete;
					var party = World.Party;
					var member = World[_packet.ObjectId] as Model.Character;
					if (member != null)
					{
						lock (party.Members)
							party.Members.Remove(member);

						if (PartyDelete != null)
							PartyDelete(party, member);
					}
				}
				else if (packet is Packet.Server.PartySmallWindowUpdate)
				{
					var _packet = packet as Packet.Server.PartySmallWindowUpdate;
					var member = World[_packet.ObjectId] as Model.Character;
					if (member != null)
					{
						member.Name = _packet.Name;
						member.Cp = _packet.Cp;
						member.MaxCp = _packet.MaxCp;
						member.Hp = _packet.Hp;
						member.MaxHp = _packet.MaxHp;
						member.Mp = _packet.Mp;
						member.MaxMp = _packet.MaxMp;
						member.Level = _packet.Level;
						member.ClassId = _packet.Class;

						if (PartyUpdate != null)
							PartyUpdate(World.Party, member);
					}
				}
				else if (packet is Packet.Server.PledgeShowMemberListAll)
				{
					var _packet = packet as Packet.Server.PledgeShowMemberListAll;
					Model.Ally ally = !_packet.InAlly ? null : new Model.Ally()
					{
						Id = _packet.AllyId,
						Name = _packet.AllyName,
						CrestId = _packet.AllyCrestId
					};

					Model.Clan clan = new Model.Clan()
					{
						Id = _packet.ClanId,
						Level = _packet.ClanLevel,
						CrestId = _packet.ClanCrestId,
						AllyId = _packet.InAlly ? _packet.AllyId : 0,
						Name = _packet.ClanName,
						Leader = _packet.LeaderName,
						InWar = _packet.InWar,
						HasCastle = _packet.HasCastle,
						HasClanhall = _packet.HasClanhall
					};

					clan.Members = new Model.ClanMember[_packet.Members.Length];
					for (int i = 0; i < _packet.Members.Length; i++)
					{
						clan.Members[i].ObjectId = _packet.Members[i].ObjectId;
						clan.Members[i].Name = _packet.Members[i].Name;
						clan.Members[i].Level = _packet.Members[i].Level;
						clan.Members[i].ClassId = _packet.Members[i].ClassId;
						clan.Members[i].IsOnline = _packet.Members[i].IsOnline;
					}

					if (ClanInfo != null)
						ClanInfo(clan, ally);
				}
				else if (packet is Packet.Server.SkillList)
				{
					var _packet = packet as Packet.Server.SkillList;
					var skills = World.Me.Skills;
					var list = new List<Model.Skill>();
					lock (skills)
						foreach (Packet.Server.SkillList.SkillItem _skill in _packet.List)
						{
							Model.Skill skill = null;
							if (skills.ContainsKey(_skill.Id))
								skill = skills[_skill.Id];
							else
								skills.Add(_skill.Id, skill = new Model.Skill());

							skill.SkillId = _skill.Id;
							skill.Level = _skill.Level;
							skill.IsActive = _skill.Active;
						}

					if (SkillList != null)
						SkillList(list);
				}
				else if (packet is Packet.Server.RestartResponse)
				{
					var _packet = packet as Packet.Server.RestartResponse;
					if (_packet.Reason == Packet.Server.RestartResponse.reason.Ok)
					{
						State = State.Lobby;
						if (Restarted != null)
							Restarted();
					}
					else
						throw new NotImplementedException();
				}
				else if (packet is Packet.Server.MoveToPawn)
				{
					var _packet = packet as Packet.Server.MoveToPawn;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.IsMoving = true;
						creature.MoveTarget = _packet.TargetId;
						creature.LastMoveTime = System.DateTime.Now;
					}
				}
				else if (packet is Packet.Server.ValidateLocation)
				{
					var _packet = packet as Packet.Server.ValidateLocation;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Heading = _packet.Heading;
						creature.Position = new Library.Point(
							_packet.PositionX,
							_packet.PositionY,
							_packet.PositionZ
						);
					}
				}
				else if (packet is Packet.Server.SystemMessage)
				{
					var _packet = packet as Packet.Server.SystemMessage;

					if (_packet.MessageId == 46) //TODO MAGIC CONST
					{
						//use skill 
						//changing of bot state based on successful skill done elsewhere now
					}

					//Todo Spoiling failed / Spoiling condition has activated catch

					const int already_spoiled = 357; //TODO MAGIC CONST
					const int spoil_landed = 612; //TODO MAGIC CONST
					var npc = World[World.Me.TargetId] as Model.Npc;
					if (npc != null && (_packet.MessageId == already_spoiled || _packet.MessageId == spoil_landed))
						npc.IsSpoiled = true;

					//
					object[] arguments = new object[_packet.Arguments.Length];
					for (int i = 0; i < _packet.Arguments.Length; i++)
					{
						object argument = null;
						var pair = _packet.Arguments[i];
						switch (pair.Key)
						{
							case Packet.Server.SystemMessage.type.Text: argument = (string)pair.Value; break;
							case Packet.Server.SystemMessage.type.Number: argument = ((int)pair.Value).ToString(); break;
							case Packet.Server.SystemMessage.type.NpcName: argument = new Model.Npc() { NpcId = (int)pair.Value }; break;
							case Packet.Server.SystemMessage.type.ItemName: argument = new Model.Item() { ItemId = (int)pair.Value }; break;
							case Packet.Server.SystemMessage.type.SkillName: argument = new Model.Skill() { SkillId = (int)pair.Value }; break;
						}
						arguments[i] = argument;
					}

					//
					if (SystemMessage != null)
						SystemMessage(_packet.MessageId, arguments);
				}
				else if (packet is Packet.Server.PledgeCrest)
				{
					var _packet = packet as Packet.Server.PledgeCrest;
					//throw new NotImplementedException();
					//ToDo: Crests.Get_ClanCrest(buffe);
				}
				else if (packet is Packet.Server.ValidateLocationInVehicle)
				{
					var _packet = packet as Packet.Server.ValidateLocationInVehicle;
					var creature = World[_packet.ObjectId] as Model.Creature;
					if (creature != null)
					{
						creature.Heading = _packet.Heading;
						creature.Position = new Library.Point(
							_packet.PositionX,
							_packet.PositionY,
							_packet.PositionZ
						);
					}
				}
				else if (packet is Packet.Server.MagicSkillLaunched)
				{
					var _packet = packet as Packet.Server.MagicSkillLaunched;
					var creature = World[_packet.ObjectId] as Model.Creature;

					if (creature != null)
					{
						if (SkillLaunched != null)
							SkillLaunched(creature);

						creature.Casting = null;
					}
				}
				else if (packet is Packet.Server.AskJoinFriend)
				{
					var _packet = packet as Packet.Server.AskJoinFriend;
					if (QuestionAsked != null)
						QuestionAsked(Lineage.Question.JoinFriend, _packet.PlayerName, null);
				}
				else if (packet is Packet.Server.Logout)
				{
					var _packet = packet as Packet.Server.Logout;
					Abort();

					if (LoggedOut != null)
						LoggedOut();
				}
				else if (packet is Packet.Server.PledgeInfo)
				{
					var _packet = packet as Packet.Server.PledgeInfo;
					lock (World.Clans)
					{
						Model.Clan clan = null;
						if (World.Clans.ContainsKey(_packet.ClanId))
							clan = World.Clans[_packet.ClanId];
						else
							World.Clans.Add(_packet.ClanId, clan = new Model.Clan());

						clan.Name = _packet.ClanName;
						clan.Ally = _packet.AllyName;
					}

					// Crests.Get_ClanCrest(buffe);
				}
				else if (packet is Packet.Server.PledgeShowInfoUpdate)
				{
					var _packet = packet as Packet.Server.PledgeShowInfoUpdate;
					Model.Clan clan = null;
					lock (World.Clans)
						if (World.Clans.ContainsKey(_packet.ClanId))
							clan = World.Clans[_packet.ClanId];
						else
							World.Clans.Add(_packet.ClanId, clan = new Model.Clan());

					clan.Id = _packet.ClanId;
					clan.CrestId = _packet.CrestId;
					clan.Level = _packet.ClanLevel;
					clan.HasCastle = _packet.HasCastle;
					clan.HasClanhall = _packet.HasClanhall;

					if (ClanUpdate != null)
						ClanUpdate(clan);
				}
				else if (packet is Packet.Server.MyTargetSelected)
				{
					var _packet = packet as Packet.Server.MyTargetSelected;
					var me = World.Me;
					var target = World[me.TargetId] as Model.Creature;
					if (target != null)
						target.NameColor = _packet.TargetColor;
					if (me.TargetId != _packet.ObjectId)
					{
						target = World[_packet.ObjectId] as Model.Creature;
						if (target != null && TargetChanged != null)
							TargetChanged(me, target);
					}
				}
				else if (packet is Packet.Server.PartyMemberPosition)
				{
					var _packet = packet as Packet.Server.PartyMemberPosition;
					foreach (var _member in _packet.Members)
					{
						var member = World[_member.ObjectId] as Model.Creature;
						if (member != null)
							member.Position = _member.Position;
					}
				}
				else if (packet is Packet.Server.AskJoinAlly)
				{
					var _packet = packet as Packet.Server.AskJoinAlly;
					if (QuestionAsked != null)
						QuestionAsked(Lineage.Question.JoinAlly, _packet.AllyName, null);
				}
				else if (packet is Packet.Server.AllyCrest)
				{
					var _packet = packet as Packet.Server.AllyCrest;
					//throw new NotImplementedException();
					//ToDo: Crests.Get_AllyCrest(buffe);
				}
				else if (packet is Packet.Server.HennaInfo)
				{
					var _packet = packet as Packet.Server.HennaInfo;
					var me = World.Me;
					me.INT = _packet.INT;
					me.STR = _packet.STR;
					me.CON = _packet.CON;
					me.MEN = _packet.MEN;
					me.DEX = _packet.DEX;
					me.WIT = _packet.WIT;
					me.Symbols = new int[_packet.Symbols.Length];
					for (int i = 0; i < me.Symbols.Length; i++)
						me.Symbols[i] = _packet.Symbols[i];

					if (HennaInfo != null)
						HennaInfo(me.Symbols);
				}
				else if (packet is Packet.Server.SignsSky)
				{
					var _packet = packet as Packet.Server.SignsSky;
					if (ChangeTime != null)
						ChangeTime(null, _packet.Sky);
				}
				else if (packet is Packet.Server.GameGuardVerfy)
				{
					var _packet = packet as Packet.Server.GameGuardVerfy;
					Network.Send(new Packet.Client.GameGuardReply());
				}
				else if (packet is Packet.Server.FriendRecvMsg)
				{
					var _packet = packet as Packet.Server.FriendRecvMsg;
					if (ChatMessage != null)
						ChatMessage(Lineage.Channel.Friend, _packet.Message, _packet.Sender, null);
				}
			}

			Dispose();
		}

		private void MoveStuffThread()
		{
			while (State >= State.Begun)
			{
				lock(World)
					foreach (var obj in World) // Todo test speed World.OfType(Model.Creature)
						if (obj is Model.Creature)
						{
							// Есть вероятность, что позицию можно расчитывать "на лету", по запросу.
							// Можно, если сервер при расчёте следования за движущейся целью переодически присылает пакеты.
							var creature = (Model.Creature)obj;
							if (creature.IsMoving)
							{
								if (creature.MoveTarget != 0 && creature.MoveTarget != creature.ObjectId)
								{
									var target = World[creature.MoveTarget] as Model.Creature;
									if (target != null)
										creature.Destination = target.Position;
								}

								double vx = creature.Destination.X - creature.Position.X;
								double vy = creature.Destination.Y - creature.Position.Y;
								double vz = creature.Destination.Z - creature.Position.Z;

								double movespeed = ((double)creature.RunSpd) * creature.MoveSpdMult;
								double time = ((System.TimeSpan)(System.DateTime.Now - creature.LastMoveTime)).Milliseconds / 1000.0f;
								double distance = new Library.Interval(creature.Position, creature.Destination).Length;
								double vxx = (movespeed * time) / distance;

								if (vxx > 1) //if we are moving past our dest
									vxx = 1; //lets cap it at the dest

								creature.Position = new Library.Point(
									creature.Position.X + (vx * vxx),
									creature.Position.Y + (vy * vxx),
									creature.Position.Z + (vz * vxx)
								);
								creature.LastMoveTime = System.DateTime.Now;

								distance = new Library.Interval(creature.Position, creature.Destination).Length;
								if (creature.MoveTarget == 0 && distance < 5.0)
									creature.IsMoving = false;
							}
						}

				System.Threading.Thread.Sleep(MoveInterval); //moved sleep to the top for when breaking to top
			}
		}

		public void Dispose()
		{
			State = State.Still;
			Network.Dispose();
			NLog.LogManager.Flush();
		}


		// ======================================================================================== //
		// ====================================== Public API ====================================== //
		// ======================================================================================== //
		// ToDo ex states check (transactions)

		public void MoveTo(Library.Point point)
		{
			if (State == State.World)
				Network.Send(new Packet.Client.MoveBackwardToLocation()
				{
					Target = point,
					Origin = World.Me.Position,
					Device = Packet.Client.MoveBackwardToLocation.device.Mouse
				});
			else
				throw new Library.Exception.StateException();
		}

		public void Stay()
		{
			if (State == State.World)
				MoveTo(World.Me.Position);
			else
				throw new Library.Exception.StateException();
		}

		public void Target(Model.Object obj) // ToDo: Model.Creature? Doors is npc?
		{
			if (State == State.World)
			{
				if (obj != null)
					Target(obj.ObjectId);
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Target(int objectId)
		{
			if (State == State.World)
			{
				if (objectId != 0 && objectId != World.Me.TargetId)
					Network.Send(new Packet.Client.Action()
					{
						TargetId = objectId,
						Origin = World.Me.Position,
						Shift = false
					});
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Cancel()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestTargetCanceld());
			else
				throw new Library.Exception.StateException();
		}

		public void Interact(Model.Object obj, bool control = false, bool shift = false)
		{
			if (State == State.World)
			{
				if (obj != null)
				{
					Library.Point? point = null;
					if (obj is Model.Creature)
						point = ((Model.Creature)obj).Position;
					else if (obj is Model.Item)
						point = ((Model.Item)obj).Position;

					if (point.HasValue)
						Interact(obj.ObjectId, point.Value, control, shift);
				}
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Interact(int objectId, Library.Point? point = null, bool control = false, bool shift = false)
		{
			if (State == State.World)
			{
				if (objectId != 0 && !point.HasValue)
				{
					var obj = World[objectId];
					if (obj is Model.Creature)
						point = ((Model.Creature)obj).Position;
					else if (obj is Model.Item)
						point = ((Model.Item)obj).Position;
				}

				if (objectId != 0 && point.HasValue)
					if (control)
						Network.Send(new Packet.Client.AttackRequest()
						{
							TargetId = objectId,
							Origin = point.Value,
							Shift = shift
						});
					else
						Network.Send(new Packet.Client.Action()
						{
							TargetId = objectId,
							Origin = point.Value,
							Shift = shift
						});
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseShortcut(int slot, int page = 0, bool control = false, bool shift = false)
		{
			if (State == State.World)
				UseShortcut(World.Shortcut[page, slot], control, shift);
			else
				throw new Library.Exception.StateException();
		}

		// Нужна ли? Мешает ввести watchers.
		public void UseShortcut(Model.Shortcut shortcut, bool control = false, bool shift = false)
		{
			if (State == State.World)
			{
				if (shortcut != null)
					switch (shortcut.Type)
					{
						case Model.Shortcut.type.Item:
							UseItem(shortcut.DataId);
							break;
						case Model.Shortcut.type.Skill:
							UseSkill(shortcut.DataId, control, shift);
							break;
						case Model.Shortcut.type.Action:
							var action = (Lineage.Action)Enum.Parse(typeof(Lineage.Action), shortcut.DataId.ToString());
							UseAction(action, control, shift);
							break;
						case Model.Shortcut.type.Macro:
							//um... do we handle this? or the server?
							throw new NotImplementedException();
							break;
						case Model.Shortcut.type.Recipe:
							throw new NotImplementedException();
							break;
					}
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseAction(Lineage.Action action, bool control = false, bool shift = false)
		{
			if (State == State.World) // ToDo and not is die
			{
				Network.Send(new Packet.Client.RequestActionUse()
				{
					Action = action,
					Control = control,
					Shift = shift
				});
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseSkill(int skillId, bool control = false, bool shift = false)
		{
			if (State == State.World)
			{
				var skills = World.Me.Skills;
				if (skillId != 0 && skills.ContainsKey(skillId))
					UseSkill(skills[skillId]);
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseSkill(Model.Skill skill, bool control = false, bool shift = false)
		{
			if (State == State.World)
			{
				if (!World.Me.IsDied && skill.IsActive)
					Network.Send(new Packet.Client.RequestMagicSkillUse()
					{
						SkillId = skill.SkillId,
						Control = control,
						Shift = shift
					});
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseItem(int objectId)
		{
			if (State == State.World)
			{
				if (objectId != 0)
				{
					var item = World[objectId] as Model.Item;
					if (item != null)
						UseItem(item);
				}
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseItem(Model.Item item)
		{
			if (State == State.World)
			{
				if (!World.Me.IsDied && item.InInventory)
					Network.Send(new Packet.Client.UseItem()
					{
						ObjectId = item.ObjectId
					});
			}
			else
				throw new Library.Exception.StateException();
		}

		public void UseSocialAction(Lineage.SocialAction action)
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestSocialAction()
				{
					Type = action
				});
			else
				throw new Library.Exception.StateException();
		}

		public void UseUserCommand(Lineage.UserCommand action)
		{
			if (State == State.World)
				switch (action)
				{
					case Lineage.UserCommand.Loc:
					case Lineage.UserCommand.Unstuck:
					case Lineage.UserCommand.Mount:
					case Lineage.UserCommand.Dismount:
					case Lineage.UserCommand.Time:
					case Lineage.UserCommand.PartyInfo:
					case Lineage.UserCommand.AttackList:
					case Lineage.UserCommand.EnemyList:
					case Lineage.UserCommand.WarList:
					case Lineage.UserCommand.FriendList:
					case Lineage.UserCommand.ClanPenalty:
						Network.Send(new Packet.Client.RequestUserCommand()
						{
							Type = action
						});
						break;
					case Lineage.UserCommand.Sit:
					case Lineage.UserCommand.Stand:
						Network.Send(new Packet.Client.ChangeWaitType2()
						{
							State = action == Lineage.UserCommand.Sit ?
								Lineage.WaitType.Sit :
								Lineage.WaitType.Stand
						});
						break;
					case Lineage.UserCommand.Walk:
					case Lineage.UserCommand.Run:
						Network.Send(new Packet.Client.ChangeMoveType2()
						{
							State = action == Lineage.UserCommand.Walk ?
								Lineage.MoveType.Walk :
								Lineage.MoveType.Run
						});
						break;
					case Lineage.UserCommand.GmList:
						Network.Send(new Packet.Client.RequestGmList());
						break;
				}
			else
				throw new Library.Exception.StateException();
		}

		public void Return(Lineage.ReturnPoint point)
		{
			if (State == State.World) // Todo and player is died
				Network.Send(new Packet.Client.RequestReturnPoint()
				{
					Point = point
				});
			else
				throw new Library.Exception.StateException();
		}

		public void QuestionReply(Lineage.Question question, Lineage.Answer answer)
		{
			if (State == State.World)
				switch (question)
				{
					case Lineage.Question.JoinFriend:
						Network.Send(new Packet.Client.RequestAnswerFriendInvite()
						{
							Answer = answer
						});
						break;
					case Lineage.Question.JoinParty:
						Network.Send(new Packet.Client.RequestAnswerJoinParty()
						{
							Answer = answer
						});
						break;
					case Lineage.Question.JoinClan:
						Network.Send(new Packet.Client.RequestAnswerJoinPledge()
						{
							Answer = answer
						});
						break;
					case Lineage.Question.JoinAlly:
						Network.Send(new Packet.Client.RequestAnswerJoinAlly()
						{
							Answer = answer
						});
						break;
					case Lineage.Question.Resurrection:
						throw new NotSupportedException("C4 don't have ressurection question. Ressurect allowed only in party.");
						break;
				}
			else
				throw new Library.Exception.StateException();
		}

		public void Say(string message, Lineage.Channel channel = Lineage.Channel.All) // Common message
		{
			if (State == State.World)
			{
				if (channel != Lineage.Channel.Tell)
				{
					Network.Send(new Packet.Client.Say2()
					{
						Channel = channel,
						Message = message
					});
				}
				else
					throw new NotSupportedException("Please use \"Say(name, target)\" method for private messages.");
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Say(string message, string target, Lineage.Channel channel = Lineage.Channel.Tell) // Private message
		{
			if (State == State.World)
			{
				if (channel == Lineage.Channel.Tell)
					Network.Send(new Packet.Client.Say2()
					{
						Channel = channel,
						Message = message,
						Target = target
					});
				else if (channel == Lineage.Channel.Friend)
					Network.Send(new Packet.Client.RequestSendFriendMsg()
					{
						Name = target,
						Message = message
					});
				else
					throw new NotSupportedException();
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Drop(int objectId, int count = 0, ItemRemove type = ItemRemove.Drop, Library.Point? point = null)
		{
			if (State == State.World)
			{
				if (objectId != 0)
				{
					var item = World[objectId] as Model.Item;
					if (item != null)
						Drop(item, count, type, point);
				}
			}
			else
				throw new Library.Exception.StateException();
		}

		public void Drop(Model.Item item, int count = 0, ItemRemove type = ItemRemove.Drop, Library.Point? point = null)
		{
			if (State == State.World)
			{
				if (item != null && item.InInventory)
				{
					if (count <= 0 || count > item.Count)
						count = item.Count;

					switch (type)
					{
						case ItemRemove.Drop:
							Library.Point _point = point.HasValue ? point.Value : World.Me.Position;
							Network.Send(new Packet.Client.RequestDropItem()
							{
								ObjectId = item.ObjectId,
								Count = count,
								Point = _point
							});
							break;
						case ItemRemove.Destroy:
							Network.Send(new Packet.Client.RequestDestroyItem()
							{
								ObjectId = item.ObjectId,
								Count = count,
							});
							break;
						case ItemRemove.Crystallize:
							Network.Send(new Packet.Client.RequestCrystallizeItem()
							{
								ObjectId = item.ObjectId,
								Count = count,
							});
							break;
					}
				}
			}
			else
				throw new Library.Exception.StateException();
		}

		public void PartyInvite(string player, Lineage.PartyLoot loot)
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestJoinParty()
				{
					Name = player,
					Loot = loot
				});
			else
				throw new Library.Exception.StateException();
		}

		public void PartyDismiss(string player) // ToDo player = null (all)
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestOustPartyMember()
				{
					Name = player
				});
			else
				throw new Library.Exception.StateException();
		}

		public void PartyLeave()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestWithDrawalParty());
			else
				throw new Library.Exception.StateException();
		}


		// ======================================================================================== //
		// ====================================== Service API ===================================== //
		// ======================================================================================== //

		public void Connect(Model.GameServer gameServer, string login, byte[] loginKey, byte[] gameKey, int protocol)
		{
			if (State == State.Still)
			{
				State = State.Begun;

				GameThread.Start();
				Network.Start(gameServer.Address, gameServer.Port, protocol, login, loginKey, gameKey);
			}
			else
				throw new Library.Exception.StateException();
		}

		public void RefreshManorList()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RefreshManorList());
			else
				throw new Library.Exception.StateException();
		}

		public void RefreshQuestList()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RefreshQuestList());
			else
				throw new Library.Exception.StateException();
		}

		public void RefreshSkillList()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RefreshSkillList());
			else
				throw new Library.Exception.StateException();
		}

		public void SelectCharacter(int number)
		{
			if (State == State.Lobby)
				Network.Send(new Packet.Client.CharacterSelected()
				{
					Number = (Int16)number
				});
			else
				throw new Library.Exception.StateException();
		}

		public void Restart()
		{
			if (State == State.World)
				Network.Send(new Packet.Client.RequestRestart());
			else
				throw new Library.Exception.StateException();
		}

		public void Logout()
		{
			if (State >= State.Lobby)
				Network.Send(new Packet.Client.Logout()); // ToDo timeout timer
			else
				throw new Library.Exception.StateException();
		}

		public void Abort()
		{
			State = State.Still;
			Network.Stop();
		}

		public void Dialog(string command) // Html dialog command
		{
			Network.Send(new Packet.Client.RequestBypassToServer()
			{
				Command = command
			});
		}
	}
}
