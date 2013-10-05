namespace Amud.Core
{
	public class Item
	{
		public Item()
		{}

		public Item(Player owner)
		{
			Owner = owner;
			Owner.Add(this);
		}

		public Item(Room room)
		{
			Room = room;
			Room.Add(this);
		}

		public string Name { get; set; }
		public Player Owner { get; set; }
		public Room Room { get; set; }

		public void PlaceInRoom(Room room)
		{
			Room = room;
			Room.Add(this);
		}

		public void PickedUp(Player player)
		{
			if ( Room == null )
				return;

			Room.Remove(this);
			Room = null;

			Owner = player;
			Owner.Add(this);
		}

		public void Give(Player newOwner)
		{
			if ( Owner == null )
				return;

			Owner.Remove(this);
			
			Owner = newOwner;
			newOwner.Add(this);
		}

		public void Drop(Room room)
		{
			if ( Owner == null )
				return;

			Room = room;
			Room.Add(this);

			Owner.Remove(this);
			Owner =null;
		}

		public override string ToString()
		{
			return Name ?? "Item";
		}
	}
}