using System;
using System.Linq;
using Amud.Core;
using Microsoft.AspNet.SignalR;

namespace AMud.SignalR
{
	public class AreaGenerator
	{
		public AreaGenerator()
		{
			CreateRooms();
			CreateItems();
			ConnectRooms();
		}

		private void CreateItems()
		{
			_sword = new Item(_hub)
			{
				Name = "Sword"
			};
		}

		private void CreateRooms()
		{
			_north = new Room
			{
				Name = "Waterfall",
				Description = "A large waterfall sprays water everywhere."
			};

			_east = new Room
			{
				Name = "Cave",
				Description = "An entrance to a dark cave."
			};

			_hub = new Room
			{
				Name = "Cross-roads",
				Description = "A sign-post sticks in the ground."
			};

			_south = new Room
			{
				Name = "Small town",
				Description = "A small town bustles"
			};

			_west = new Room
			{
				Name = "A river",
				Description = "A fast-moving river"
			};
		}

		private void ConnectRooms()
		{
			_hub
				.Connect(MoveDirection.North, _north)
				.Connect(MoveDirection.East, _east)
				.Connect(MoveDirection.South, _south)
				.Connect(MoveDirection.West, _west)
				;
		}

		private Room _north;
		private Room _east;
		private Room _hub;
		private Room _south;
		private Room _west;

		private Item _sword;

		public Area Build()
		{
			return new Area(new[] { _north, _east, _hub, _south, _west }) { StartingRoom = _hub };
		}
	}

	public static class MessageExtensions
	{
		public static object ToPlayerMessage(this Player player)
		{
			return new
			{
				player.Name,
				Inventory = player.Inventory.Select(i => new { i.Name })
			};
		}

		public static object ToRoomMessage(this Room room)
		{
			return new
			{
				room.Name,
				room.Description,
				Players = room.Players.Select(p => new { p.Name }),
				Contents = room.Contents.Select(i => new { i.Name })
			};
		}
	}

	public class MessageHub : Hub
	{
		private static readonly Lazy<IClientProvider> _clientProvider = new Lazy<IClientProvider>(GetClientProvider);
		private static readonly Lazy<Area> _area = new Lazy<Area>(GetArea);

		private static Area GetArea()
		{
			var generator = new AreaGenerator();
			return generator.Build();
		}

		private static IClientProvider GetClientProvider()
		{
			return (IClientProvider) System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IClientProvider));
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
			var player = client.Player;

			player.ChangeRoom(Area.StartingRoom);

			Groups.Add(client.ConnectionId, Area.StartingRoom.Name);
			Clients.OthersInGroup(Area.StartingRoom.Name).sendMessage(string.Format("{0} appears from nowhere!", player.Name));
			
			Clients.Caller.sendMessage("Hello " + player.Name);
			Clients.Caller.sendRoom(player.Room.ToRoomMessage());
			Clients.Caller.sendPlayer(player.ToPlayerMessage());

			return client.Id;
		}

		public void PickupItem(string itemName)
		{
			var client = ClientProvider.GetClient(Context.ConnectionId);
			var player = client.Player;
			var room = player.Room;
			var item = room.Contents.SingleOrDefault(i => i.Name == itemName);
			if ( item == null )
				return;

			item.PickedUp(player);
			if (item.Owner != player) 
				return;

			Clients.Caller.sendMessage("You pick up the " + itemName);
			Clients.Caller.sendPlayer(player.ToPlayerMessage());
			Clients.OthersInGroup(room.Name).sendMessage(string.Format("{0} picks up the {1}!", player.Name, itemName));
			Clients.Group(room.Name).sendRoom(room.ToRoomMessage());
		}

		public void DropItem(string itemName)
		{
			var client = ClientProvider.GetClient(Context.ConnectionId);
			var player = client.Player;
			var room = player.Room;
			var item = player.Inventory.SingleOrDefault(i => i.Name == itemName);
			if ( item == null )
				return;

			item.Drop(player.Room);

			Clients.Caller.sendMessage("You drop the " + itemName);
			Clients.Caller.sendPlayer(player.ToPlayerMessage());
			Clients.OthersInGroup(room.Name).sendMessage(string.Format("{0} drops the {1}!", player.Name, itemName));
			Clients.Group(room.Name).sendRoom(room.ToRoomMessage());
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
			Clients.Caller.sendRoom(newRoom.ToRoomMessage());
		}
	}
}