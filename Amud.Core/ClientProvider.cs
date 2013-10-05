using System;
using System.Collections.Concurrent;

namespace Amud.Core
{
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