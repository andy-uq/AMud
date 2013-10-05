using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
	public class ItemTests
	{
		[Test]
		public void Create()
		{
			var item = new Item { Name = "1234" };
			item.Owner.Should().BeNull();
			item.Room.Should().BeNull();

			item.ToString().Should().Be("1234");
		}

		[Test]
		public void CreatePlayerItem()
		{
			var p1 = new Player();
			var item = new Item(p1) { Name = "1234" };

			item.Room.Should().BeNull();
			item.Owner.Should().Be(p1);
			p1.Inventory.Should().Contain(item);
		}

		[Test]
		public void CreateRoomItem()
		{
			var room = new Room();
			var item = new Item(room) { Name = "1234" };

			item.Owner.Should().BeNull();
			item.Room.Should().Be(room);
			room.Contents.Should().Contain(item);
		}

		[Test]
		public void PlaceInRoom()
		{
			var room = new Room();
			var item = new Item();
			item.PlaceInRoom(room);

			item.Room.Should().Be(room);
			room.Contents.Should().Contain(item);
		}

		[Test]
		public void PickUp()
		{
			var room = new Room();
			var player = new Player();

			var item = new Item();
			item.PlaceInRoom(room);

			item.PickedUp(player);
			item.Owner.Should().Be(player);
			player.Inventory.Should().Contain(item);

			item.Room.Should().BeNull();
			room.Contents.Should().NotContain(item);
		}

		[Test]
		public void Drop()
		{
			var room = new Room();
			var player = new Player();

			var item = new Item(player);

			item.Drop(room);

			item.Owner.Should().BeNull();
			player.Inventory.Should().NotContain(item);

			item.Room.Should().Be(room);
			room.Contents.Should().Contain(item);
		}

		[Test]
		public void CannotPickUpWhenInInventory()
		{
			var p1 = new Player();
			var p2 = new Player();
			var item = new Item { Owner = p1 };

			item.PickedUp(p2);
			item.Owner.Should().Be(p1);
		}

		[Test]
		public void Give()
		{
			var p1 = new Player();
			var p2 = new Player();
			var item = new Item(p1);
			
			item.Give(p2);

			p1.Inventory.Should().NotContain(item);

			item.Owner.Should().Be(p2);
			p2.Inventory.Should().Contain(item);
		}
	}
}