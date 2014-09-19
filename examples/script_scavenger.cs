using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf
{
	sealed class Scavenger
	{
		static void Main(string[] args)
		{
			(new Scavenger()).Run();
		}
	
		private void Run()
		{
			Api = new Server.Script.Api();
			var servers = Api.Login("test", "123456");
			var characters = Api.SelectServer(servers.First());
			Api.SelectPlayer(characters.Single(p => p.Name == "scavenger"));

			Api.ChatMessage += OnChatMessage;
			Api.CreatureUpdate += OnCreatureUpdate;
			Api.QuestionAsked += OnQuestionAsked;
			Api.SkillReused += OnSkillReused;
			Api.StartMoving += OnMoving;
			Api.FinishMoving += OnMoving;
			Api.Die += OnDie;

			while (Api.State > Server.Script.State.NotConnected)
				lock (Api.Sync)
				{
					Api.DoEvents();
					Api.Wait(r => true);
				}
		}

		private void Attack()
		{
			Enemy = Api.World[Master.TargetId] as Model.Creature;
			if (Enemy == null)
				Api.SayAsync("Don't find", Master.Name);
			else if (Enemy == Api.Me)
				Api.SayAsync("It's me", Master.Name);
			else
			{
				State = StateType.ATTACK;

				if (Api.Me.IsSitting)
					Api.UseUserCommandAsync(Lineage.UserCommand.Stand);

				if (Api.Me.TargetId != Enemy.ObjectId)
					Api.Target(Enemy);

				Api.InteractAsync(Enemy, true);
				if (Spoil.IsReady)
					Api.UseSkillAsync(Spoil, true);
				else if (WildSweep.IsReady)
					Api.UseSkillAsync(WildSweep, true);
			}
		}

		private void Return()
		{
			State = StateType.NOTHING;
			if (Api.Me.IsSitting)
				Api.UseUserCommandAsync(Lineage.UserCommand.Stand);
			Api.Cancel();
			Api.MoveToAsync(Master.Position);
		}

		private void Rest()
		{
			if (Api.Me.Hp < Api.Me.MaxHp || Api.Me.Mp < Api.Me.MaxMp)
			{
				State = StateType.REST;
				if (!Api.Me.IsSitting)
					Api.UseUserCommandAsync(Lineage.UserCommand.Sit);
			}
		}

		private void Flee()
		{
			State = StateType.FLEE;
			OnMoving(Master);
		}

		private void OnChatMessage(Lineage.Channel channel, string message, string name, Model.Creature author = null)
		{
			Console.WriteLine(string.Format("[{0}] {1}: {2}", channel, name, message));

			if (message == "Serve!" && author is Model.Character)
			{
				Api.Target(Master = author as Model.Character);
				Api.UseSocialActionAsync(Lineage.SocialAction.Bow);
			}
			else if (message == "Leave!" && author == Master)
			{
				Master = null;
				Api.UseSocialActionAsync(Lineage.SocialAction.Dance);
			}
			else if (Master != null && Master.Name == name && message.Trim().EndsWith("!"))
				switch (message.Trim().TrimEnd('!').ToLower())
				{
					case "attack":
						Attack();
					break;
					case "return":
						Return();
					break;
					case "rest":
						Rest();
					break;
					case "quit":
						Api.Logout();
					break;
				}
		}

		private void OnQuestionAsked(Lineage.Question question, string name, string value = null)
		{
			if (Master != null && Master.Name == name)
				Api.QuestionReplyAsync(question, Lineage.Answer.Yes);
		}

		private void OnCreatureUpdate(Model.Creature creature)
		{
			if (creature == Api.Me)
				if (Api.Me.MaxHp * HP_EMERGENCY_THRESHOLD >= Api.Me.Hp)
					Flee();
				else if (State == StateType.ATTACK)
				{
					var target = Api.World[Api.Me.TargetId] as Model.Npc;
					if(target != null && !target.IsAlikeDead)
						if(!target.IsSpoiled && Spoil.IsReady)
							Api.UseSkillAsync(Spoil);
						else if(IsAllowWildSweep && WildSweep.IsReady)
							Api.UseSkillAsync(WildSweep);
				}
				else if (State == StateType.REST && Api.Me.Hp == Api.Me.MaxHp && Api.Me.Mp == Api.Me.MaxMp)
				{
					State = StateType.NOTHING;
					if (Api.Me.IsSitting)
						Api.UseUserCommandAsync(Lineage.UserCommand.Stand);
				}
		}

		private void OnSkillReused(Model.Skill skill)
		{
			if (State == StateType.ATTACK)
				if (skill.SkillId == Spoil.SkillId)
				{
					var npc = Enemy as Model.Npc;
					if (npc != null && !npc.IsSpoiled)
						Api.UseSkillAsync(Spoil, true);
				}
				else if (skill.SkillId == WildSweep.SkillId && IsAllowWildSweep)
					Api.UseSkillAsync(WildSweep, true);
		}

		private void OnMoving(Model.Creature creature)
		{
			if (State == StateType.FLEE && creature == Master)
				Api.MoveToAsync(Master.IsMoving ? Master.Destination : Master.Position);
		}

		private void OnDie(Model.Creature creature, Lineage.ReturnPoint? points = null)
		{
			if (creature == Enemy)
			{
				State = StateType.NOTHING;
				var npc = Enemy as Model.Npc;
				if (npc != null && npc.IsSpoiled)
					Api.UseSkillAsync(Sweep, true);
			}
			else if (creature == Master)
			{
				Api.Cancel();
				Api.Logout();
			}
		}

		private bool IsAllowWildSweep
		{
			get
			{
				return Api.Me.MaxHp * HP_DANGER_THRESHOLD >= Api.Me.Hp || Api.Me.MaxMp * MP_SAVING_THRESHOLD < Api.Me.Mp;
			}
		}

		private Model.Skill _WildSweep = null;
		private Model.Skill WildSweep
		{
			get
			{
				if (_WildSweep == null)
					_WildSweep = Api.Me.Skills[SKILL_WILD_SWEEP];
				return _WildSweep;
			}
		}

		private Model.Skill _Spoil = null;
		private Model.Skill Spoil
		{
			get
			{
				if (_Spoil == null)
					_Spoil = Api.Me.Skills[SKILL_SPOIL];
				return _Spoil;
			}
		}

		private Model.Skill _Sweep = null;
		private Model.Skill Sweep
		{
			get
			{
				if (_Sweep == null)
					_Sweep = Api.Me.Skills[SKILL_SWEEP];
				return _Sweep;
			}
		}


		private Server.Script.Api Api = null;
		private Model.Creature Enemy = null;
		private Model.Character Master = null;
		private StateType State = StateType.NOTHING;

		enum StateType
		{
			NOTHING,
			ATTACK,
			REST,
			FLEE,
		}

		const int SKILL_WILD_SWEEP = 245;
		const int SKILL_SPOIL = 254;
		const int SKILL_SWEEP = 42;

		const float MP_SAVING_THRESHOLD = .33f;
		const float HP_DANGER_THRESHOLD = .33f;
		const float HP_EMERGENCY_THRESHOLD = .1f;
	}
}
