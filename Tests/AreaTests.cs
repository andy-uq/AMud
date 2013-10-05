using System.Linq;
using AMud.SignalR;
using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class AreaTests
	{
		private Room _north;
		private Room _east;
		private Room _hub;
		private Room _south;
		private Room _west;

		[Test]
		public void CreateArea()
		{
			var area = CreateTestArea();
			area.Rooms.Should().NotBeEmpty();
		}

		[Test]
		public void AreaHasStartingRoom()
		{
			var area = CreateTestArea();
			area.StartingRoom.Should().Be(_hub);
		}

		[Test]
		public void RoomsAreConnected()
		{
			var area = CreateTestArea();
			_hub.Move(MoveDirection.North).Should().Be(_north);
			_hub.Move(MoveDirection.South).Should().Be(_south);
			_hub.Move(MoveDirection.East).Should().Be(_east);
			_hub.Move(MoveDirection.West).Should().Be(_west);

			_north.Move(MoveDirection.South).Should().Be(_hub);
			_south.Move(MoveDirection.North).Should().Be(_hub);
			_east.Move(MoveDirection.West).Should().Be(_hub);
			_west.Move(MoveDirection.East).Should().Be(_hub);
		}

		private Area CreateTestArea()
		{
			_north = new Room { Name = "Waterfall", Description = "A large waterfall sprays water everywhere." };
			_east = new Room { Name = "Cave", Description = "An entrance to a dark cave." };
			_hub = new Room { Name = "Cross-roads", Description = "A sign-post sticks in the ground." };
			_south = new Room { Name = "Small town", Description = "A small town bustles" };
			_west = new Room { Name = "A river", Description = "A fast-moving river" };
			_hub
				.Connect(MoveDirection.North, _north)
				.Connect(MoveDirection.East, _east)
				.Connect(MoveDirection.South, _south)
				.Connect(MoveDirection.West, _west)
				;

			return new Area(new[] { _north, _east, _hub, _south, _west }) { StartingRoom = _hub };
		}
	}
}