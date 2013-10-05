using System;
using Amud.Core;
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
}