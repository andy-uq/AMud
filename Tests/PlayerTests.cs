using AMud.SignalR;
using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class PlayerTests
	{
		[Test]
		public void CreatePlayer()
		{
			var player = new Player();
		}

		[Test]
		public void CreatePlayerInRoom()
		{
			var player = new Player();
			player.ChangeRoom(new Room()).Should().BeNull();
		}

		[Test]
		public void PlayerChangeRoom()
		{
			var player = new Player();
			var startRoom = new Room();
			player.ChangeRoom(startRoom);

			var newRoom = new Room();
			player.ChangeRoom(newRoom).Should().Be(startRoom);
		}
	}
}