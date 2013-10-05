using System.Collections.Generic;

namespace Amud.Core
{
	public class Player
	{
		private Room _room;
		private HashSet<Item> _inventory;

		public Player()
		{
			_inventory = new HashSet<Item>();
		}

		public Room Room
		{
			get { return _room; }
		}

		public string Name { get; set; }

		public IEnumerable<Item> Inventory
		{
			get { return _inventory; }
		}

		public Room ChangeRoom(Room newRoom)
		{
			Room oldRoom = _room;

			if (newRoom.Add(this))
			{
				_room = newRoom;

				if (oldRoom != null)
				{
					oldRoom.Remove(this);
					return oldRoom;
				}
			}

			return null;
		}

		public void Add(Item item)
		{
			_inventory.Add(item);
		}

		public void Remove(Item item)
		{
			_inventory.Remove(item);
		}
	}
}