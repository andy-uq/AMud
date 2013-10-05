using System;

namespace Amud.Core
{
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
}