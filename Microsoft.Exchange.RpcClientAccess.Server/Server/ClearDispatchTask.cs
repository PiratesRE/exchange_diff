using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class ClearDispatchTask : RpcHttpConnectionRegistrationDispatchTask
	{
		public ClearDispatchTask(RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, CancelableAsyncCallback asyncCallback, object asyncState) : base("ClearDispatchTask", rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState)
		{
		}

		internal override int? InternalExecute()
		{
			return new int?(base.RpcHttpConnectionRegistrationDispatch.EcClear());
		}

		public int End()
		{
			return base.CheckCompletion();
		}
	}
}
