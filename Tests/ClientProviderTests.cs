using Amud.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class ClientProviderTests
	{
		[Test]
		public void Create()
		{
			new ClientProvider();
		}

		[Test]
		public void CreateClient()
		{
			var provider = new ClientProvider();
			var client = provider.Connect("1234");

			client.Should().NotBeNull();
		}

		[Test]
		public void GetClient()
		{
			var provider = new ClientProvider();
			var client = provider.Connect("1234");

			provider.GetClient("1234").Should().Be(client);
		}

		[Test]
		public void GetClientWithBadConnectionId()
		{
			var provider = new ClientProvider();
			provider.GetClient("1234").Should().BeNull();
		}
	}
}