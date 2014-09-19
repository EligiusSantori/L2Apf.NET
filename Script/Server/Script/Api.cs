using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L2Apf.Server.Script
{
	public sealed class Api
	{
		public Api()
		{
			Logger = NLog.LogManager.GetCurrentClassLogger();
			Config = new Model.LoginServer();

			LoginServer = new Server.Login.Api()
			{
				IsBackground = true
			};
			GameServer = new Server.Game.Api()
			{
				IsBackground = true,
				MoveInterval = 100
			};

			LoginServer.Connected += OnLoginConnected;
			LoginServer.ServerSelected += OnServerSelected;
			GameServer.Connected += OnGameConnected;
			GameServer.EnterWorld += OnEnterWorld;
			GameServer.LoggedOut += OnLoggedOut;
			GameServer.StartMoving += OnStartMoving;
			GameServer.FinishMoving += OnFinishMoving;
			GameServer.ActionFailed += OnActionFailed;
			GameServer.TargetChanged += OnTargetChanged;
			GameServer.Attack += OnAttack;
			GameServer.SkillStarted += OnSkillStarted;
			GameServer.SkillLaunched += OnSkillLaunched;
			GameServer.SkillCanceled += OnSkillCanceled;
			GameServer.SkillReused += OnSkillReused;
			GameServer.CreatureUpdate += OnCreatureUpdate;
			GameServer.ItemInfo += OnItemInfo;
			GameServer.Die += OnDie;
			GameServer.QuestionAsked += OnQuestionAsked;
			GameServer.ChatMessage += OnChatMessage;
		}

		~Api()
		{
			Logout(true);

			LoginServer.Connected -= OnLoginConnected;
			LoginServer.ServerSelected -= OnServerSelected;
			GameServer.Connected -= OnGameConnected;
			GameServer.EnterWorld -= OnEnterWorld;
			GameServer.LoggedOut -= OnLoggedOut;
			GameServer.StartMoving -= OnStartMoving;
			GameServer.FinishMoving -= OnFinishMoving;
			GameServer.ActionFailed -= OnActionFailed;
			GameServer.TargetChanged -= OnTargetChanged;
			GameServer.Attack -= OnAttack;
			GameServer.SkillStarted -= OnSkillStarted;
			GameServer.SkillLaunched -= OnSkillLaunched;
			GameServer.SkillCanceled -= OnSkillCanceled;
			GameServer.SkillReused -= OnSkillReused;
			GameServer.CreatureUpdate -= OnCreatureUpdate;
			GameServer.ItemInfo -= OnItemInfo;
			GameServer.Die -= OnDie;
			GameServer.QuestionAsked -= OnQuestionAsked;
			GameServer.ChatMessage -= OnChatMessage;
		}

		#region Synchronization

		public Result.Result Wait(Func<Result.Result, bool> predicate, TimeSpan? timeout = null)
		{
			if (timeout == null)
				timeout = Timeout.InfiniteTimeSpan;

			if (predicate != null)
			{
				Predicate = predicate;
				Result = null;

				Monitor.Wait(Sync, timeout.Value);

				return Result;
			}
			else
				throw new ArgumentNullException();
		}

		private void Wake(Result.Result result)
		{
			if (Predicate != null && Predicate(result))
			{
				Predicate = null;
				Result = result;
				Monitor.Pulse(Sync);
			}
		}

		#endregion

		#region Event Handle

		private void OnLoginConnected(IEnumerable<Model.GameServer> gameServers)
		{
			lock (Sync)
			{
				Wake(new Result.LoginConnected()
				{
					GameServers = gameServers
				});
			}
		}

		private void OnServerSelected(byte[] loginKey, byte[] gameKey)
		{
			lock (Sync)
			{
				Wake(new Result.ServerSelected()
				{
					LoginKey = loginKey,
					GameKey = gameKey
				});
			}
		}

		private void OnGameConnected(IEnumerable<Model.Player> characters)
		{
			lock (Sync)
			{
				Wake(new Result.GameConnected()
				{
					Characters = characters
				});
			}
		}

		private void OnEnterWorld()
		{
			lock (Sync)
			{
				Wake(new Result.EnterWorld());
			}
		}

		private void OnLoggedOut()
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (LoggedOut != null)
						LoggedOut();
				});
				Wake(new Result.LoggedOut());
			}
		}

		private void OnTargetChanged(Model.Creature creature, Model.Creature target)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (TargetChanged != null)
						TargetChanged(creature, target);
				});
				Wake(new Result.TargetChanged()
				{
					Creature = creature,
					Target = target
				});
			}
		}

		private void OnAttack(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (Attack != null)
						Attack(creature);
				});
				Wake(new Result.Attack()
				{
					Creature = creature
				});
			}
		}

		private void OnStartMoving(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (StartMoving != null)
						StartMoving(creature);
				});
				Wake(new Result.StartMoving()
				{
					Creature = creature
				});
			}
		}

		private void OnFinishMoving(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (FinishMoving != null)
						FinishMoving(creature);
				});
				Wake(new Result.FinishMoving()
				{
					Creature = creature
				});
			}
		}

		private void OnActionFailed()
		{
			lock (Sync)
			{
				Wake(new Result.ActionFailed());
			}
		}

		private void OnSkillStarted(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (SkillStarted != null)
						SkillStarted(creature);
				});
				Wake(new Result.SkillStarted()
				{
					Creature = creature
				});
			}
		}

		private void OnSkillLaunched(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (SkillLaunched != null)
						SkillLaunched(creature);
				});
				Wake(new Result.SkillLaunched()
				{
					Creature = creature
				});
			}
		}

		private void OnSkillCanceled(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (SkillCanceled != null)
						SkillCanceled(creature);
				});
				Wake(new Result.SkillCanceled()
				{
					Creature = creature
				});
			}
		}

		private void OnSkillReused(Model.Skill skill)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (SkillReused != null)
						SkillReused(skill);
				});
				Wake(new Result.SkillReused()
				{
					Skill = skill
				});
			}
		}

		private void OnCreatureUpdate(Model.Creature creature)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (CreatureUpdate != null)
						CreatureUpdate(creature);
				});
				Wake(new Result.CreatureUpdated()
				{
					Creature = creature
				});
			}
		}

		private void OnItemInfo(Model.Item item)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (ItemInfo != null)
						ItemInfo(item);
				});
				Wake(new Result.ItemInfo()
				{
					Item = item
				});
			}
		}

		private void OnDie(Model.Creature creature, Lineage.ReturnPoint? points = null)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (Die != null)
						Die(creature, points);
				});
				Wake(new Result.Die()
				{
					Creature = creature,
					Points = points
				});
			}
		}

		private void OnQuestionAsked(Lineage.Question question, string name, string value = null)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (QuestionAsked != null)
						QuestionAsked(question, name, value);
				});
				Wake(new Result.QuestionAsked()
				{
					Question = question,
					Name = name,
					Value = value,
				});
			}
		}

		private void OnChatMessage(Lineage.Channel channel, string message, string name, Model.Creature author = null)
		{
			lock (Sync)
			{
				EventQueue.Enqueue(() =>
				{
					if (ChatMessage != null)
						ChatMessage(channel, message, name, author);
				});
				Wake(new Result.ChatMessage()
				{
					Channel = channel,
					Message = message,
					Name = name,
					Author = author,
				});
			}
		}

		#endregion

		#region Public API

		/// <returns>GameServer list</returns>
		public IEnumerable<Model.GameServer> Login(string login, string password) // Todo: SocketException
		{
			lock (Sync)
			{
				LoginServer.Connect(Config, Identity = new Model.Account()
				{
					Login = login,
					Password = password
				});

				var result = Wait(r => r is Result.LoginConnected || r is Result.LoginFail);
				if (result is Result.LoginConnected)
					return ((Result.LoginConnected)result).GameServers;
				else
					throw new LoginFailException((Result.LoginFail)result);
			}
		}

		/// <returns>Is logged out correctly</returns>
		public bool Logout(bool force = false, TimeSpan? timeout = null)
		{
			if (force && timeout == null)
				timeout = TimeSpan.FromSeconds(5);
			else
				timeout = Timeout.InfiniteTimeSpan;

			if (GameServer.State > Game.State.Still)
				lock (Sync)
				{
					GameServer.Logout();
					var result = Wait(r => r is Result.LoggedOut || r is Result.ActionFailed,
						timeout.Value);

					if (force && result is Result.ActionFailed)
						GameServer.Abort();

					return result is Result.LoggedOut;
				}
			else
				return true;
		}

		/// <returns>Character list</returns>
		public IEnumerable<Model.Player> SelectServer(Model.GameServer server) // Todo: SocketException
		{
			lock (Sync)
			{
				LoginServer.SelectGameServer(server);
				var result = Wait(r => r is Result.ServerSelected | r is Result.PlayFail);

				if (result is Result.ServerSelected)
					lock(Sync)
					{
						var sk = (Result.ServerSelected)result;
						GameServer.Connect(server, Identity.Login, sk.LoginKey, sk.GameKey, Config.Protocol);
						var cl = Wait(r => r is Result.GameConnected) as Result.GameConnected;
						return cl.Characters;
					}
				else
					throw new PlayFailException((Result.PlayFail)result);
			}
		}

		public void SelectPlayer(Model.Player player)
		{
			lock (Sync)
			{
				GameServer.SelectCharacter(player.Number);
				Wait(r => r is Result.EnterWorld);
			}
		}

		/// <returns>Is target been set to obj</returns>
		public bool Target(Model.Object obj)
		{
			if (obj == null)
				return Cancel();
			else if (Me.TargetId != obj.ObjectId)
				lock (Sync)
				{
					GameServer.Target(obj);
					return Wait(r =>
					{
						if (r is Result.TargetChanged)
						{
							var _r = (Result.TargetChanged)r;
							return _r.Creature == Me && _r.Target == obj;
						}
						else
							return r is Result.ActionFailed;
					}) is Result.TargetChanged;
				}
			else
				return true;
		}

		/// <returns>Is target been unset</returns>
		public bool Cancel()
		{
			if (Me.TargetId != 0)
				lock(Sync)
				{
					GameServer.Cancel();
					return Wait(r =>
					{
						if (r is Result.TargetChanged)
						{
							var _r = (Result.TargetChanged)r;
							return _r.Creature == Me && _r.Target == null;
						}
						else
							return r is Result.ActionFailed;
					}) is Result.TargetChanged;
				}
			else
				return true;
		}

		public bool UseSkill(Model.Skill skill, bool control = false, bool shift = false, Target target = 0)
		{
			lock (Sync)
			{
				GameServer.UseSkill(skill, control, shift);
				switch (target)
				{
					case Script.Target.SkillReused:
						Wait(r =>
						{
							var _r = r as Result.SkillReused;
							return _r != null && _r.Skill == skill;
						});
						return true;
					case Script.Target.SkillLaunched:
						return Wait(r =>
						{
							return r is Result.SkillLaunched || r is Result.SkillCanceled || r is Result.ActionFailed;
						}) is Result.SkillLaunched;
					default: // Script.Target.SkillStarted
						return Wait(r =>
						{
							return r is Result.SkillStarted || r is Result.ActionFailed;
						}) is Result.SkillStarted;
				}
			}
		}

		// Todo: public void UseUserCommand(Lineage.UserCommand command) // Success or error
		// Todo: public void UseSocialAction(Lineage.SocialAction action) // Finished or interrupt
		// Todo: public Result.Result MoveTo(Library.Point point) Todo: FinishMoving or AbnormalEffect or MagicSkillUse or ?
		// Todo: public void Interact(Model.Object obj, bool control = false, bool shift = false) // Succes/Die or ActionFail
		// Todo: public void UseSkill(Model.Skill skill, bool control = false, bool shift = false, SyncTarget target = 0) // Cast/Launched/Reused or Canceled
		// Todo: public void QuestionReply(Lineage.Question question, Lineage.Answer answer) // When relative action completed
		// Todo: public void Say(string message, Lineage.Channel channel = Lineage.Channel.All) // Success or block chat
		// Todo: public void Say(string message, string target) // Success or block chat or ignore or not logged

		public void MoveToAsync(Library.Point point)
		{
			GameServer.MoveTo(point);
		}

		public void InteractAsync(Model.Object obj, bool control = false, bool shift = false)
		{
			GameServer.Interact(obj, control, shift);
		}

		public void UseSkillAsync(Model.Skill skill, bool control = false, bool shift = false)
		{
			GameServer.UseSkill(skill, control, shift);
		}

		public void UseUserCommandAsync(Lineage.UserCommand command)
		{
			GameServer.UseUserCommand(command);
		}

		public void UseSocialActionAsync(Lineage.SocialAction action)
		{
			GameServer.UseSocialAction(action);
		}

		public void QuestionReplyAsync(Lineage.Question question, Lineage.Answer answer)
		{
			GameServer.QuestionReply(question, answer);
		}

		public void SayAsync(string message, Lineage.Channel channel = Lineage.Channel.All)
		{
			GameServer.Say(message, channel);
		}

		public void SayAsync(string message, string target)
		{
			GameServer.Say(message, target);
		}

		public void DoEvents()
		{
			while (EventQueue.Count > 0)
				EventQueue.Dequeue()();
			/*while (true)
			{
				Action _event = null;
				lock (Sync)
					if (EventQueue.Count > 0)
						_event = EventQueue.Dequeue();
					else
						break;
				_event();
			}*/
		}

		#endregion

		public State State
		{
			get
			{
				if (GameServer.State >= Server.Game.State.World)
					return State.Running;
				else if (LoginServer.State > Server.Login.State.Init || GameServer.State > Server.Game.State.Still)
					return State.Preparing;
				else
					return State.NotConnected;
			}
		}

		public Model.World World
		{
			get { return GameServer.World; }
		}
		public Model.Player Me
		{
			get { return World.Me; }
		}

		public bool IsBackground
		{
			get { return LoginServer.IsBackground && GameServer.IsBackground; }
			set { LoginServer.IsBackground = GameServer.IsBackground = value; }
		}


		public event Server.Game.Api.TargetChangedHandler TargetChanged;
		public event Server.Game.Api.CreatureHandler CreatureUpdate;
		public event Server.Game.Api.ItemHandler ItemInfo;
		public event Server.Game.Api.DieHandler Die;
		public event Server.Game.Api.CreatureHandler Attack;
		public event Server.Game.Api.CreatureHandler StartMoving;
		public event Server.Game.Api.CreatureHandler FinishMoving;
		public event Server.Game.Api.CreatureHandler SkillStarted;
		public event Server.Game.Api.CreatureHandler SkillLaunched;
		public event Server.Game.Api.CreatureHandler SkillCanceled;
		public event Server.Game.Api.SkillReuseHandler SkillReused;
		public event Server.Game.Api.QuestionHandler QuestionAsked;
		public event Server.Game.Api.ChatMessageHandler ChatMessage;
		public event Server.Game.Api.EmptyHandler LoggedOut;


		public Model.LoginServer Config;
		private Model.Account Identity;
		private Server.Login.Api LoginServer;
		private Server.Game.Api GameServer;
		private NLog.Logger Logger;

		private Func<Result.Result, bool> Predicate; // Условие выхода из ожидания (предикат wait)
		private Result.Result Result = null; // Поле для временного хранения результата, возвращаемого wait
		public readonly object Sync = new object(); // handle критических секций и блокировки потока
		private Queue<Action> EventQueue = new Queue<Action>(); // Очередь событий
	}
}