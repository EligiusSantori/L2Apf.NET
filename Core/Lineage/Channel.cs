using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Lineage
{
	public enum Channel
	{
		All = 0,
		Shout = 1, //!
		Tell = 2,
		Party = 3, //#
		Clan = 4,  //@
		GameMaster = 5,    
		PetitionPlayer = 6, // used for petition
		PetitionGameMaster = 7, //* used for petition
		Trade = 8, //+
		Alliance = 9, //$
		Announce = 10,
		Boat = 11,
		PartyMember = 15, //(yellow)
		PartyLeader = 16, //(blue)
		HeroVoice = 17,
		Friend = -1
	}
}
