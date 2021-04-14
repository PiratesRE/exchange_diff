using System;

namespace Microsoft.Exchange.Rpc
{
	internal interface IRpcHttpConnectionRegistrationDispatch
	{
		int Register(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, out string failureMessage, out string failureDetails);

		int Unregister(Guid associationGroupId, Guid requestId);

		int Clear();
	}
}
