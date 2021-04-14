using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface INotificationSession
	{
		int RpcContext { get; }

		Guid UserGuid { get; }
	}
}
