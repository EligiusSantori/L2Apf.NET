using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Apf.Model
{
    public sealed class World : ICollection<Model.Object>
    {
		public Model.Object this[int objectId]
        {
            get
            {
				lock (this)
				{
					if (objectId != 0 && Objects.ContainsKey(objectId))
						return Objects[objectId];
					else
						return null;
				}
            }
        }

		// Todo: Дополнительная коллекция с хешированием по имени.
		public Model.Character Find(string name)
		{
			lock(this)
				foreach (var obj in Objects.Values)
					if(obj is Model.Character)
					{
						var character = (Model.Character)obj;
						if(string.Compare(character.Name, name, true) == 0)
							return character;
					}

			return null;
		}

        public void Add(Model.Object obj)
        {
			lock(this)
				Objects.Add(obj.ObjectId, obj);
        }

		public void Clear()
		{
			lock(this)
				Objects.Clear();
		}

		public bool Contains(int objectId)
		{
			return Objects.ContainsKey(objectId);
		}

		public bool Remove(int objectId)
		{
			lock (this)
				return Objects.Remove(objectId);
		}

		public bool Remove(Model.Object obj)
		{
			lock (this)
				return Objects.Remove(obj.ObjectId);
		}

		public IEnumerator<Model.Object> GetEnumerator()
		{
			return Objects.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Objects.Values.GetEnumerator();
		}

		public bool Contains(Model.Object obj)
		{
			throw new NotSupportedException();
		}

		public void CopyTo(Model.Object[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count { get { return Objects.Count; } }
		public bool IsReadOnly { get { return false; } }


		public Model.Player Me = null;
		public Model.Party Party = null; // To Model.Player?
		//public Dictionary<int, Model.Skill> Skills = new Dictionary<int, Skill>(); // ToDo: Move to Player if no actually
		public Dictionary<int, Model.Clan> Clans = new Dictionary<int, Model.Clan>();
		public Model.Shortcut[,] Shortcut = new Model.Shortcut[Model.Shortcut.PAGE_COUNT, Model.Shortcut.ICON_COUNT]; // ToDo Move out
		private Dictionary<int, Model.Object> Objects = new Dictionary<int, Object>();
	}
}
