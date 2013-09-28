using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MvcWebRole1.SignalR
{
	public class MessageHub : Hub
	{
		public static Lazy<IClientProvider> _clientProvider = new Lazy<IClientProvider>(GetClientProvider);

		private static IClientProvider GetClientProvider()
		{
			return (IClientProvider) System.Web.Mvc.DependencyResolver.Current.GetService(typeof(IClientProvider));
		}

		private static IClientProvider ClientProvider
		{
			get { return _clientProvider.Value; }
		}

		public Guid Connect()
		{
			var client = ClientProvider.Connect(Context.ConnectionId);
			Clients.Others.sendMessage(string.Format("{0} arrives from the south", client.Id));
			Clients.Caller.sendMessage("Hello " + client.Id);
			return client.Id;
		}

		public void Move(MoveDirection direction)
		{
			var client = ClientProvider.GetClient(Context.ConnectionId);
			Clients.Others.sendMessage(string.Format("{0} leaves to the {1}", client.Id, direction));
			Clients.Caller.sendMessage(string.Format("You leave to the {0}", direction));
		}
	}

	public enum MoveDirection
	{
		North,
		East,
		South,
		West
	}

	public interface IClientProvider
	{
		Client Connect(string connectionId);
		Client GetClient(string connectionId);
	}

	public class Client
	{
		public Guid Id { get; set; }
		public string ConnectionId { get; set; }
	}

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
				ConnectionId = connectionId
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