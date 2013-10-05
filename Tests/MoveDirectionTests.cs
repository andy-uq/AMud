using AMud.SignalR;
using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class MoveDirectionTests
	{
		[Test]
		 public void Opposites()
		 {
			 MoveDirection.North.Opposite().Should().Be(MoveDirection.South);
			 MoveDirection.South.Opposite().Should().Be(MoveDirection.North);
			 MoveDirection.East.Opposite().Should().Be(MoveDirection.West);
			 MoveDirection.West.Opposite().Should().Be(MoveDirection.East);
		 }
	}
}