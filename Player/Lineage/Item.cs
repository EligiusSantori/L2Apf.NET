using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Lineage
{
	enum Item
	{
		Adena, // Нет шанса выставления на продажу
		AncientAdena, // Небольшой шанс выставления на продажу
		SealStone, // Есть шанс выставления на продажу, если с деньгами туго и эвент проигран
		Weapon,
		Armor,
		HpPotion,
		MpPotion,
		ScrollOfEscape,
		ScrollOfResurrection,
	}
}
