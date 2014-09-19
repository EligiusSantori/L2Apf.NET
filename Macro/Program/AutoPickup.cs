using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Apf.Program
{
	public sealed class AutoPickup : Base
	{
		public AutoPickup(Server.Game.Api gsApi)
			: base(gsApi)
		{
			gsApi.ItemInfo += (Model.Item item) =>
			{
				if (Enabled)
				{
					if (OnlyMy && item.DroppedBy == gsApi.World.Me.TargetId)
						List.Add(item);
					else if (!OnlyMy)
						List.Add(item);

					Next();
				}
			};

			gsApi.ObjectRemoved += (Model.Object obj) =>
			{
				var item = obj as Model.Item;
				if (item != null)
					Pickup(item);
			};

			gsApi.Pickup += (Model.Item item, Model.Creature creature, Library.Point position) =>
			{
				Pickup(item);
			};
		}

		public void Start(bool onlyMy = true)
		{
			OnlyMy = onlyMy;
			Enabled = true;

			if (!OnlyMy)
			{
				foreach (var obj in gsApi.World)
				{
					var item = obj as Model.Item;
					if (item != null && !item.InInventory)
						List.Add(item);
				}

				Next();
			}
		}

		public override void Dispose()
		{
			Enabled = false;
		}

		private void Next()
		{
			if (List.Count > 0 && Performed == null)
			{
				var item = List.ByMin(_item => Library.Interval.Distance(gsApi.World.Me.Position, _item.Position.Value));
				gsApi.Interact(item);
				Performed = item;
			}
		}

		private void Pickup(Model.Item item)
		{
			if (Enabled)
			{
				if (item == Performed)
					Performed = null;
				if (List.Contains(item))
					List.Remove(item);

				Next();
			}
		}

		public bool OnlyMy { get; set; }
		private Model.Item Performed = null;
		private List<Model.Item> List = new List<Model.Item>();
	}
}
