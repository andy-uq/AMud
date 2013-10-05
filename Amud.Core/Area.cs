using System.Collections.Generic;

namespace Amud.Core
{
	public class Area
	{
		private readonly List<Room> _rooms;

		public IEnumerable<Room> Rooms { get { return _rooms; } }
		public Room StartingRoom { get; set; }

		public Area(IEnumerable<Room> rooms)
		{
			_rooms = new List<Room>(rooms);
		}


	}
}