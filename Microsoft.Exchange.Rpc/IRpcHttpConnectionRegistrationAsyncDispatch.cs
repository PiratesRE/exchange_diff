using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc
{
	internal interface IRpcHttpConnectionRegistrationAsyncDispatch
	{
		ICancelableAsyncResult BeginRegister(Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndRegister(ICancelableAsyncResult result, out string failureMessage, out string failureDetails);

		ICancelableAsyncResult BeginUnregister(Guid associationGroupId, Guid requestId, CancelableAsyncCallback asyncCallback, object asyncState);

		int EndUnregister(ICancelableAsyncResult result);

		ICancelableAsyncResult BeginClear(CancelableAsyncCallback asyncCallback, object asyncState);

		int EndClear(ICancelableAsyncResult result);
	}
}
