using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;

namespace AMud.SignalR
{
	public class MessageHub : Hub
	{
		private static Lazy<IClientProvider> _clientProvider = new Lazy<IClientProvider>(GetClientProvider);
		private static Lazy<Area> _area = new Lazy<Area>(GetArea);

		private static IClientProvider GetClientProvider()
		{
			return (IClientProvider) System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IClientProvider));
		}

		private static Area GetArea()
		{
			var r1 = new Room { Name = "Waterfall", Description = "A large waterfall sprays water everywhere." };
			var r2 = new Room { Name = "Cave", Description = "An entrance to a dark cave." };
			var r3 = new Room { Name = "Cross-roads", Description = "A sign-post sticks in the ground." };
			var r4 = new Room { Name = "Small town", Description = "A small town bustles" };
			var r5 = new Room { Name = "A river", Description = "A fast-moving river" };
			r3
				.Connect(MoveDirection.North, r1)
				.Connect(MoveDirection.East, r2)
				.Connect(MoveDirection.South, r4)
				.Connect(MoveDirection.West, r5)
				;

			return new Area(new[] { r1, r2, r3, r4, r5 }) { StartingRoom = r3 };
		}

		private static IClientProvider ClientProvider
		{
			get { return _clientProvider.Value; }
		}

		private static Area Area
		{
			get { return _area.Value; }
		}

		public Guid Connect()
		{
			var client = ClientProvider.Connect(Context.ConnectionId);
			client.Player.ChangeRoom(Area.StartingRoom);

			Groups.Add(client.ConnectionId, Area.StartingRoom.Name);
			Clients.OthersInGroup(Area.StartingRoom.Name).sendMessage(string.Format("{0} appears from nowhere!", client.Player.Name));
			Clients.Caller.sendMessage("Hello " + client.Player.Name);

			return client.Id;
		}

		public void Move(MoveDirection direction)
		{
			var client = ClientProvider.GetClient(Context.ConnectionId);
			var previousRoom = client.Player.Room;

			var newRoom = previousRoom.Move(direction);
			if ( newRoom == null )
				return;

			client.Player.ChangeRoom(newRoom);

			Clients.OthersInGroup(previousRoom.Name).sendMessage(string.Format("{0} leaves to the {1}", client.Player.Name, direction));
			Clients.Caller.sendMessage(string.Format("You leave to the {0}", direction));

			Groups.Remove(client.ConnectionId, previousRoom.Name);
			Groups.Add(client.ConnectionId, newRoom.Name);

			Clients.OthersInGroup(newRoom.Name).sendMessage(string.Format("{0} appears from the {1}", client.Player.Name, direction.Opposite()));
			Clients.Caller.sendMessage("You arrive at " + newRoom.Name);
		}
	}

	public class Area
	{
		private List<Room> _rooms;

		public IEnumerable<Room> Rooms { get { return _rooms; } }
		public Room StartingRoom { get; set; }

		public Area(IEnumerable<Room> rooms)
		{
			_rooms = new List<Room>(rooms);
		}


	}

	public enum MoveDirection
	{
		North,
		East,
		South,
		West
	}

	public interface IClientProvider
	{
		Client Connect(string connectionId);
		Client GetClient(string connectionId);
	}

	public class Client
	{
		public Guid Id { get; set; }
		public string ConnectionId { get; set; }

		public Player Player { get; set; }
	}

	[DebuggerDisplay("{Name}")]
	public class Room
	{
		private readonly Dictionary<MoveDirection, Room> _connected;
		private readonly HashSet<Player> _players;

		public string Name { get; set; }
		public string Description { get; set; }

		public HashSet<Player> Players
		{
			get { return _players; }
		}

		public Room()
		{
			_connected = new Dictionary<MoveDirection, Room>();
			_players = new HashSet<Player>();			
		}

		public bool Add(Player player)
		{
			return _players.Add(player);
		}

		public void Remove(Player player)
		{
			_players.Remove(player);
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

	public static class MoveDirectionExtensions
	{
		public static MoveDirection Opposite(this MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.North:
					return MoveDirection.South;

				case MoveDirection.East:
					return MoveDirection.West;

				case MoveDirection.South:
					return MoveDirection.North;

				case MoveDirection.West:
					return MoveDirection.East;

				default:
					throw new ArgumentOutOfRangeException("direction");
			}
		}
	}

	public class Player
	{
		private Room _room;

		public Room Room
		{
			get { return _room; }
		}

		public string Name { get; set; }

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
	}

	public class ClientProvider : IClientProvider
	{
		private readonly ConcurrentDictionary<string, Client> _connectedClients;

		public ClientProvider()
		{
			_connectedClients = new ConcurrentDictionary<string, Client>();
		}

		public Client Connect(string connectionId)
		{
			return _connectedClients.GetOrAdd(connectionId, CreateNewClient);
		}

		private Client CreateNewClient(string connectionId)
		{
			return new Client
			{
				Id = Guid.NewGuid(),
				ConnectionId = connectionId,
				Player = new Player { Name = "Tall dark stranger", }
			};
		}

		public Client GetClient(string connectionId)
		{
			Client client;
			return _connectedClients.TryGetValue(connectionId, out client)
				       ? client
				       : null;
		}
	}
}