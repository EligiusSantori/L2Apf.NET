using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace L2Apf.Ai
{
	class _Blob
	{
		public _Blob(Server.Script.Api api, Memory memory)
		{
			Api = api;
			World = api.World;
			Memory = memory;

			Timer = new System.Timers.Timer()
			{
				AutoReset = true,
				Enabled = false,
				Interval = 1000,
				//Todo: Timer.SynchronizingObject
			};
		}

		public void Pass()
		{
			Api.Attack += OnAttack;
			Api.ChatMessage += OnChatMessage;
			Timer.Elapsed += OnTimerElapsed;
			Timer.Start();

			Console.WriteLine(sizeof(long));

			while (Api.State > Server.Script.State.NotConnected)
				lock (Api.Sync)
				{
					Api.DoEvents();
					Api.Wait(r => true);
				}
		}

		public void Free()
		{
			Api.Attack -= OnAttack;
			Api.ChatMessage -= OnChatMessage;
			Timer.Elapsed -= OnTimerElapsed;
			Timer.Stop();
		}

		public void War() 
		{
			Console.WriteLine("War()");
		}

		public void Job()
		{
			Console.WriteLine("Job()");
		}

		public void Map()
		{
			// Todo: free explore missing regions (nearest * random)
				// Model.Memory.Region.FromPosition(World.Me.Position);
				// foreach(Memory.Regions
			Console.WriteLine("Map()");
		}

		private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Todo: (Cache results) // А как быть, если нужно пересчитывать каждый раз, но за исключением некоторых вложенных вычислений
			// Todo: Рандомизировать только в ограниченном диапазоне, т.е. если одно действие имеет вес 0.1, а другое 0.9, шанс первого должен быть очень мал. Также если вес имеет 0, действие абсолютно недоступно для выбора.
				// Нужна ли рандомизация? Да. Например чтоб среди действий с одинаковым весом (напр. 0.5) выбиралось случайное. Но её нужно очень хорошо продумать. 
			var s = new List<Tuple<double, Action>>();
			s.Add(new Tuple<double, Action>(1 - Memory.Nature.Peaceness, War)); // Todo: Split to: Hunting, Attackers, PvP & PK
			s.Add(new Tuple<double, Action>(Memory.Nature.Peaceness, Job)); // Todo: Split to: Courier, Private trade, Quests
			s.Add(new Tuple<double, Action>(Memory.Nature.Curiosity, Map));
			s.Sort((x, y) => x.Item1.CompareTo(y.Item1));
			s[s.Count - 1].Item2();

			// todo if (calculation size > n) Api.UseSocialActionAsync(Lineage.SocialAction.Unaware);
		}

		private void OnAttack(Model.Creature creature)
		{
			if (!Memory.Attackers.Contains(creature))
			{
				Memory.Attackers.Add(creature); // Todo LastAttackTime & AttackersTimer::OnElapsed => Attackers.remove(a.lastAttackTime > n & Nature.aggro...)
				// Todo clear damage
			}

			/*if(Attackers.Select(a => a.Danger).Sum() > current_danger_limit)
				escape();
			else
				// Если уже сражаемся и AP умный, то не меняем цель (может если цель слабая то переключаемся на неё?)
				// Иначе входим в режим сражения*/
			// Todo Кеширование результатов анализа с удалением по времени и ручным удалением при некой кондиции (например при связанном событии)
		}

		// Todo: OnCreatureInfo => Memory.AddNpc|AddPlayer
		
		private void OnChatMessage(Lineage.Channel channel, string message, string from, Model.Creature author)
		{
			// Todo: Speech
		}

		private Server.Script.Api Api;
		private Model.World World;
		private Memory Memory;
		private Timer Timer;
	}
}
