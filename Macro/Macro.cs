using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace L2Apf
{
	sealed class Macro
	{
		static void Main(string[] args)
		{
			NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.config");

			lsApi = new Server.Login.Api()
			{
				IsBackground = false
			};
			gsApi = new Server.Game.Api()
			{
				IsBackground = false,
				MoveInterval = 100
			};

			Setup(Parse(args));
		}

		private static LoginFile Parse(string[] args)
		{
			LoginFile script = null;
			foreach(var arg in args)
			{
				if (arg.StartsWith("/login:"))
				{
					string name = arg.Split(":".ToCharArray(), 2).Last();
					if (global::System.IO.File.Exists(name))
						script = new LoginFile(name);
					else
						Console.Out.WriteLine("Script not found");
				}
				else if (arg.StartsWith("/global:"))
					bool.TryParse(arg.Split(":".ToCharArray(), 2).Last(), out Global);
				else if (arg.StartsWith("/auth:"))
				{
					bool.TryParse(arg.Split(":".ToCharArray(), 2).Last(), out Access.IsFree);
					Access.IsFree = !Access.IsFree;
				}
				else if (arg.StartsWith("/owners:"))
					Access.Owners.AddRange(arg.Split(":".ToCharArray(), 2).Last().Split(",".ToCharArray()));
				else if (arg.StartsWith("/password:"))
					Access.Password = arg.Split(":".ToCharArray(), 2).Last();

				else
					Console.Out.WriteLine("Unknown command");
			}

			if(script == null)
				Environment.Exit(1);
			return script;
		}

		private static void Setup(LoginFile script)
		{
			lsApi.Connected += (IEnumerable<Model.GameServer> gameServers) =>
			{
				Model.GameServer gameServer = gameServers
					.Single(gs => gs.Id == script.GameServer);

				if (gameServer != null)
					lsApi.SelectGameServer(gameServer);
				else
					throw new KeyNotFoundException();
			};

			lsApi.ServerSelected += (byte[] loginKey, byte[] gameKey) =>
			{
				string login = script.Account.Login;
				int protocol = script.LoginServer.Protocol;
				gsApi.Connect(lsApi.GameServer, login, loginKey, gameKey, protocol);
			};

			gsApi.Connected += (IEnumerable<Model.Player> characters) =>
			{
				int? number = null;
				foreach (var character in characters)
					if (character.Name == script.Character)
						number = character.Number;

				if (number.HasValue)
					gsApi.SelectCharacter(number.Value);
				else
					throw new KeyNotFoundException();
			};

			gsApi.ChatMessage += (Lineage.Channel channel, string message, string name, Model.Creature author) =>
			{
				var me = gsApi.World.Me;
				if(Global || channel == Lineage.Channel.Tell)
					if (name != me.Name && Access.Allow(name) && message.Trim().StartsWith("/"))
						Command(message.Trim().Substring(1).ToLower(), name, author);
			};

			gsApi.QuestionAsked += (Lineage.Question question, string name, string value) =>
			{
				if (Access.Allow(name))
					gsApi.QuestionReply(question, Lineage.Answer.Yes);
			};

			gsApi.LoggedOut += () =>
			{
				Environment.Exit(0);
			};

			gsApi.EnterWorld += () =>
			{
				ConsoleThread.Start();
			};

			lsApi.Connect(script.LoginServer, script.Account);
		}

		private static void Command(string command, string name, Model.Creature author)
		{
			string[] args = command.Split(" ".ToCharArray());
			command = args.First();
			args = args.Skip(1).ToArray();
			var world = gsApi.World;
			var me = gsApi.World.Me;

			switch (command)
			{
				case "hello":
					Reply("Hello", name);
					break;
				/*case "login":
					if (args.Length == 1)
						if (Access.LogIn(name, args.First()))
							Reply("Accept", name);
						else
							Reply("Reject", name);
					break;*/
				case "quit":
					gsApi.Logout();
					break;
				case "to":
				case "moveto":
				{
					Library.Point? point = null;
					if (args.Length == 0)
					{
						var target = world[me.TargetId] as Model.Creature;
						if (target != null)
							point = target.Position;
					}
					if (args.Length == 1 && author != null)
						if (args.First() == "me")
							point = author.Position;
						else if (args.First() == "my")
							point = author.Destination;

					if (point.HasValue)
					{
						Manager.Free(MOVE_PROGRAM);
						gsApi.MoveTo(point.Value);
					}
					break;
				}
				case "get":
				case "target":
					if (args.Length == 1)
						switch (args[0])
						{
							case "self":
								gsApi.Target(me);
								break;
							case "me":
								if (author != null)
									gsApi.Target(author);
								else
									Reply("Can't find", name);
								break;
							case "my":
								if (author != null)
								{
									var target = world[author.TargetId] as Model.Creature;
									if (target != null)
									{
										gsApi.Target(target);
										break;
									}
								}
								Reply("Can't find", name);
								break;
							case "npc": //Живой, для мёртвого можно ввести команду target corpse
							{
								Model.Npc nearest = null;
								var min = double.MaxValue;

								lock(world)
									foreach (var obj in world)
									{
										var npc = obj as Model.Npc;
										if (npc != null && !npc.IsAlikeDead)
										{
											var distance = me.Distance(npc);
											if (distance < min)
											{
												min = distance;
												nearest = npc;
											}
										}
									}

								if (nearest != null)
									gsApi.Target(nearest);
								break;
							}
							default:
								Model.Character character = world.Find(args[0]);
								if (character != null)
									gsApi.Target(character);
								else
									Reply("Can't find", name);
								break;
						}
					break;
				case "attack":
					gsApi.Interact(me.TargetId, null, true);
					break;
				case "cancel":
					gsApi.Cancel();
					break;
				case "up":
				case "pickup":
				{
					/*int count = 0;

					if(args.Length == 1)
						int.TryParse(args[0], out count);

					...
					
					if (items.Count > 0)
						gsApi.Interact(items.First());*/

					/* Централизованный подбор предметов:
					var items = world
						.Where(obj => obj is Model.Item && !((Model.Item)obj).InInventory)
						.OrderBy(obj => me.Distance((Model.Item)obj)).ToList();*/
					/* Децентрализованный подбор предметов:
					 * Сохраняем count и выполняем подбор ближайшего предмета с count-- пока count > 0 */

					Model.Item nearest = null;
					var min = double.MaxValue;

					lock(world)
						foreach (var obj in world)
						{
							var item = obj as Model.Item;
							if (item != null && !item.InInventory)
							{
								var distance = me.Distance(item);
								if (distance < min)
								{
									min = distance;
									nearest = item;
								}
							}
						}

					if (nearest != null)
						gsApi.Interact(nearest);
					break;
				}
				case "skill":
				case "useskill":
					int skillId = 0;
					if (args.Length == 1 && int.TryParse(args[0], out skillId))
						gsApi.UseSkill(skillId, true);
					break;
				case "item":
				case "useitem":
				{
					int itemId = 0;
					if (args.Length == 1 && int.TryParse(args[0], out itemId))
						lock (world)
							foreach (var obj in world)
							{
								var item = obj as Model.Item;
								if (item != null && item.ItemId == itemId && item.InInventory)
								{
									gsApi.UseItem(item);
									break;
								}
							}
					break;
				}
				case "items":
				case "itemlist":
					lock(world)
						foreach (var obj in world)
						{
							var item = obj as Model.Item;
							if (item != null && item.InInventory)
								Reply(string.Format("[{0}] {1}", item.ItemId, item.Count), name);
						}
					break;
				case "drop":
				case "dropitem":
				{
					int itemId = 0;
					int count = 0;
					if (args.Length > 0)
						int.TryParse(args[0], out itemId);
					if (args.Length > 1)
						int.TryParse(args[1], out count);

					if (itemId != 0)
					{
						lock (world)
							foreach (var obj in world)
							{
								var item = obj as Model.Item;
								if (item != null && item.ItemId == itemId && item.InInventory)
								{
									gsApi.Drop(item, count);
									break;
								}
							}
					}
					break;
				}
				case "return":
				{
					var point = Lineage.ReturnPoint.Town;
					if (args.Length == 1)
						Enum.TryParse<Lineage.ReturnPoint>(args[0], out point);
					gsApi.Return(point);
					break;
				}
				case "info":
					new Command.Info(gsApi, args, name).Run();
					break;
				case "travel":
				{
					if (args.Length == 0)
					{
						var program = Manager.Get(MOVE_PROGRAM) as Program.Move.Travel;
						var distance = program != null ? new Library.Interval(me.Position, program.Model.Waypoints.Last()).Length : 0;
						Reply(string.Format("{0}", (int)distance), name);
					}
					else if (args.Length == 3)
					{
						double x, y, z;
						if (double.TryParse(args[0], out x) &&
							double.TryParse(args[1], out y) &&
							double.TryParse(args[2], out z))
						{
							var program = new Program.Move.Travel(gsApi);
							program.Start(new Library.Point(x, y, z));
							Manager.Load(program, MOVE_PROGRAM);
						}
					}
					break;
				}
				case "follow":
				{
					var target = world[me.TargetId] as Model.Creature;
					if (args.Length >= 1 && target != null)
						switch (args[0])
						{
							case "fast":
							{
								var program = new Program.Move.FastFollow(gsApi);
								program.Bind(target);

								Manager.Load(program, MOVE_PROGRAM);
								break;
							}
							case "full":
							{
								var program = new Program.Move.FullFollow(gsApi);
								program.Bind(target);

								Manager.Load(program, MOVE_PROGRAM);
								break;
							}
						}
					break;
				}
				case "cross":
				{
					int size = 0;
					if (args.Length == 1 && int.TryParse(args[0], out size))
					{
						var center = me.Position;
						var program = new Program.Move.SimpleRoute(gsApi);
						program.Load(new Library.Point[]
						{
							new Library.Point(center.X + size, center.Y, center.Z),
							new Library.Point(center.X, center.Y + size, center.Z),
							new Library.Point(center.X - size, center.Y, center.Z),
							new Library.Point(center.X, center.Y - size, center.Z),
							new Library.Point(center.X + size, center.Y, center.Z),
							center
						});

						Manager.Load(program, MOVE_PROGRAM);
					}
					break;
				}
				case "action":
				{
					if (args.Length > 0)
						switch (args[0])
						{
							case "sit":
								gsApi.UseUserCommand(Lineage.UserCommand.Sit);
								break;
							case "stand":
								gsApi.UseUserCommand(Lineage.UserCommand.Stand);
								break;
						}
					break;
				}
				case "moving":
				{
					if(args.Length > 0 && System.IO.File.Exists(args[0]))
					{
						var parts = new List<List<Library.Point>>();
						var reader = new System.IO.BinaryReader(
							System.IO.File.OpenRead(args[0]));
						while (reader.PeekChar() != -1)
						{
							var points = new List<Library.Point>();
							int count = reader.ReadInt32();
							for (int i = 0; i < count; i++)
								points.Add(new Library.Point(
									reader.ReadInt32(),
									reader.ReadInt32(),
									reader.ReadInt32()));
							parts.Add(points);
						}

						var graph = new Model.Graph(parts);
						var program = new Program.Move.OnGraph(gsApi);
						program.Play(graph);
						Manager.Load(program, MOVE_PROGRAM);
					}
					break;
				}
				case "autospoil":
				{
					throw new NotImplementedException();
					break;
				}
				case "autosweep": // ToDo: Нуждается в тестировании и отладке
				{
					if (args.Length > 0)
					{
						var program = new Program.AutoSweep(gsApi);
						switch (args[0])
						{
							case "my": program.Start(true); break;
							case "all": program.Start(false); break;
						}
						if (program.Enabled)
							Manager.Load(program, typeof(Program.AutoSweep).Name);
					}
					else
					{
						var program = Manager.Get(typeof(Program.AutoSweep).Name) as Program.AutoSweep;
						if (program != null)
							Reply(string.Format("Enabled ({0})", program.OnlyMy ? "My" : "All"), name);
						else
							Reply("Disabled", name);
					}
					break;
				}
				case "autopickup":
				{
					if (args.Length > 0)
					{
						var program = new Program.AutoPickup(gsApi);
						switch (args[0])
						{
							case "my": program.Start(true); break;
							case "all": program.Start(false); break;
						}
						if (program.Enabled)
							Manager.Load(program, typeof(Program.AutoPickup).Name);
					}
					else
					{
						var program = Manager.Get(typeof(Program.AutoPickup).Name) as Program.AutoPickup;
						if (program != null)
							Reply(string.Format("Enabled ({0})", program.OnlyMy ? "My" : "All"), name);
						else
							Reply("Disabled", name);
					}
					break;
				}
				case "route":
				{
					if (args.Length > 0)
						switch(args[0])
						{
							case "record":
								if(args.Length > 1)
									switch (args[1])
									{
										case "add":
											if (RouteRecord != null && author != null)
											{
												RouteRecord.Add(author.Position);
												RouteRecord.Save();
											}
											break;
										case "del":
											if (RouteRecord != null)
											{
												RouteRecord.Del();
												RouteRecord.Save();
											}
											break;
										case "auto":
											int period = 1;
											if (args.Length == 3)
												int.TryParse(args[2], out period);
											if (RecordTimer != null)
												RecordTimer.Close();
											RecordTimer = new System.Timers.Timer(period * 1000);
											RecordTimer.Elapsed += (object timer, System.Timers.ElapsedEventArgs ea) =>
											{
												if (RecordLastPos != author.Position)
												{
													RecordLastPos = author.Position;
													RouteRecord.Add(author.Position);
													RouteRecord.Save();
												}
											};
											RecordTimer.Enabled = true;
											break;
										case "stop":
											if (RecordTimer != null)
												RecordTimer.Close();
											break;
										default: //ToDo ../ fix
											RouteRecord = new FileRoute(new System.IO.FileInfo(string.Format("{0}.route", args[1])));
											break;
									}
								break;
							default:
								if (args.Length == 1 && !string.IsNullOrEmpty(name = args[0])) //ToDo ../ fix
								{
									bool reverse = name.StartsWith("-");
									name = (name[0] == '-' || name[0] == '+' ? name.Substring(1) : name);
									var route = new FileRoute(new System.IO.FileInfo(string.Format("{0}.route", name)));
									route.Load();

									if (route.Count > 0)
									{
										var points = !reverse ? route.Points : route.Points.Reverse();
										var program = new Program.Move.SimpleRoute(gsApi);
										program.Load(points);
										Manager.Load(program, MOVE_PROGRAM);
									}
								}
								break;
						}

					else if (RouteRecord != null)
						Reply(string.Format("Count: {0}, Length: {1}",
							RouteRecord.Count, RouteRecord.Length), name);
					break;
				}
				default:
					Reply("Don’t understand", name);
				break;
			}
		}

		private static void Reply(string message, string name)
		{
			if (!string.IsNullOrEmpty(name))
				gsApi.Say(message, name);
			else
				Console.WriteLine(message);
		}

		private static void ConsoleLoop()
		{
			while (gsApi.State > Server.Game.State.Still)
				Command(Console.ReadLine().Trim().Substring(1).ToLower(), string.Empty, null);
		}

		private static Thread ConsoleThread = new Thread(ConsoleLoop);

		// Состояния
		private static bool Global = false;

		// Служебные объекты
		private static Program.Manager Manager = new Program.Manager();
		private static Model.Access Access = new Model.Access();
		private static Server.Login.Api lsApi;
		private static Server.Game.Api gsApi;

		// Частные объекты // ToDo: Move to Command.Route
		private static FileRoute RouteRecord;
		private static System.Timers.Timer RecordTimer;
		private static Library.Point RecordLastPos;

		//private static Model.Travel Travel = null;

		const string MOVE_PROGRAM = "Move";
	}
}
