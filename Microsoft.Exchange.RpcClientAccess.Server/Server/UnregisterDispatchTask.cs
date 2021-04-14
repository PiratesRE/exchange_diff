using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class UnregisterDispatchTask : RpcHttpConnectionRegistrationDispatchTask
	{
		public UnregisterDispatchTask(RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, CancelableAsyncCallback asyncCallback, object asyncState, Guid associationGroupId, Guid requestId) : base("UnregisterDispatchTask", rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState)
		{
			this.associationGroupId = associationGroupId;
			this.requestId = requestId;
		}

		internal override int? InternalExecute()
		{
			return new int?(base.RpcHttpConnectionRegistrationDispatch.EcUnregister(this.associationGroupId, this.requestId));
		}

		public int End()
		{
			return base.CheckCompletion();
		}

		private readonly Guid associationGroupId;

		private readonly Guid requestId;
	}
}
