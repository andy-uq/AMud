using System;

namespace Amud.Core
{
	public class Client
	{
		public Guid Id { get; set; }
		public string ConnectionId { get; set; }

		public Player Player { get; set; }
	}
}