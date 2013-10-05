namespace Amud.Core
{
	public interface IClientProvider
	{
		Client Connect(string connectionId);
		Client GetClient(string connectionId);
	}
}