using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
	// Нельзя использовать как конечный класс для других игроков, у класса игроков могут быть личные свойства и методы
	public abstract class Character : Creature
	{
		public Character()
		{
			IsRunning = true;
			IsSitting = false;
			Cubics = new List<int>();
		}

		public bool IsRunning { get; set; }
		public bool IsSitting { get; set; }

		public int Race { get; set; } // ToDo enum?
		public int ClassId { get; set; } // ToDo enum?
		public int BaseClassId { get; set; } // ToDo enum?
		public int HairStyle { get; set; } // ToDo enum
		public int HairColor { get; set; } // ToDo enum
		public int FaceType { get; set; } // ToDo enum
		public Lineage.Gender Gender { get; set; }

		public Equipment Equipment;

		public int Sp { get; set; }
		public long Xp { get; set; }
		public int Karma { get; set; }
		public int Load { get; set; }
		public int MaxLoad { get; set; }

		public int PDef { get; set; }
		public int MDef { get; set; }
		public int PAtk { get; set; }
		public int MAtk { get; set; }
		public int Accuracy { get; set; }
		public int Focus { get; set; }
		public int Evasion { get; set; }
		public bool InPvP { get; set; } // ToDo enum InPvP {1, 2}

		public int ClanId { get; set; }
		public int AllyId { get; set; }
		public int ClanCrestId { get; set; } // ToDo remove and use clan.Crest
		public int AllyCrestId { get; set; } // ToDo remove and use ally.Crest
		public int LargeCrestId { get; set; } // ToDo remove and use clan.Large
		public bool IsClanLeader { get; set; } // ToDo if (clan.Leader == character)
		public Lineage.MountType MountType { get; set; }
		public Lineage.PrivateStore PrivateStore { get; set; }
		public byte HasDwarfCraft { get; set; } // ToDo bool/enum
		public int Pk { get; set; }
		public int PvP { get; set; }

		public bool IsInvisible { get; set; }
		public bool IsFindParty { get; set; }
		public int SpecialEffects { get; set; }
		public int AbnormalEffects { get; set; }

		public int RecLeft { get; set; }
		public int RecAmount { get; set; } //0 = white | 255 = blue
		public byte Enchant { get; set; }
		public byte TeamCircle { get; set; } //1= Blue, 2 = red // ToDo enum
		
		//public int TitleColor { get; set; } // ToDo color

		//public int ClanPrivileges { get; set; }
		//public int ClanClass { get; set; }
		//public int SiegeFlags { get; set; }
		//public uint WarState { get; set; }

		public bool IsHeroIcon { get; set; }
		public bool IsHeroGlow { get; set; }

		public bool IsFishing { get; set; }
		public Library.Point Fish { get; set; }
		public List<int> Cubics { get; set; }
	}
}
