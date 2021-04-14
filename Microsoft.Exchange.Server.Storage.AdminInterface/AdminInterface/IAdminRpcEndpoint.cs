using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public interface IAdminRpcEndpoint
	{
		bool StartInterface(Guid? instanceGuid, bool isLocalOnly);

		void StopInterface();
	}
}
