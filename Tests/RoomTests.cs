using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMud.SignalR;
using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
	public class RoomTests
    {
	    [Test]
		public void CreateRoom()
	    {
		    var room = new Room();
	    }

		[Test]
		public void RoomHasName()
		{
			var room = new Room()
			{
				Name = "1234"
			};
			room.ToString().Should().Be("1234");
		}

		[Test]
		public void AddPlayer()
		{
			var room = new Room();
			var player = new Player();

			room.Add(player).Should().BeTrue();
		}

		[Test]
		public void RemovePlayer()
		{
			var room = new Room();
			var player = new Player();

			room.Add(player).Should().BeTrue();
			room.Remove(player);
		}

		[Test]
	    public void RoomShouldBeNullWhenMoveInBadDirection()
	    {
			var x = new Room();
		    x.Move(MoveDirection.North).Should().BeNull();
	    }

		[Test]
		public void ConnectRooms()
		{
			var x = new Room();
			var y = new Room();

			x.Connect(MoveDirection.North, y);
			x.Move(MoveDirection.North).Should().Be(y);

		}
    }
}
