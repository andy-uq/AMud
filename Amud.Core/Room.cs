using System.Collections.Generic;
using System.Diagnostics;

namespace Amud.Core
{
	[DebuggerDisplay("{Name}")]
	public class Room
	{
		private readonly Dictionary<MoveDirection, Room> _connected;
		private readonly HashSet<Player> _players;
		private readonly HashSet<Item> _contents;

		public string Name { get; set; }
		public string Description { get; set; }

		public HashSet<Player> Players
		{
			get { return _players; }
		}

		public IEnumerable<Item> Contents
		{
			get { return _contents; }
		}

		public Room()
		{
			_connected = new Dictionary<MoveDirection, Room>();
			_players = new HashSet<Player>();			
			_contents = new HashSet<Item>();
		}

		public bool Add(Player player)
		{
			return _players.Add(player);
		}

		public void Add(Item item)
		{
			_contents.Add(item);
		}

		public void Remove(Player player)
		{
			_players.Remove(player);
		}

		public void Remove(Item item)
		{
			_contents.Remove(item);
		}

		public Room Connect(MoveDirection direction, Room room)
		{
			room._connected[direction.Opposite()] = this;
			_connected[direction] = room;

			return this;
		}

		public Room Move(MoveDirection direction)
		{
			Room newRoom;
			if ( _connected.TryGetValue(direction, out newRoom) )
				return newRoom;

			return null;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}