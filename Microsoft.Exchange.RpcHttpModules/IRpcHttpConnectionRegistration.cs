using System;

namespace Microsoft.Exchange.RpcHttpModules
{
	public interface IRpcHttpConnectionRegistration
	{
		int Register(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, out string failureMessage, out string failureDetails);

		void Unregister(Guid associationGroupId, Guid requestId);

		void Clear();
	}
}
