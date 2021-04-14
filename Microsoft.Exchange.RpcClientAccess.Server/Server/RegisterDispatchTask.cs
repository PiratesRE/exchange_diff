using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RegisterDispatchTask : RpcHttpConnectionRegistrationDispatchTask
	{
		public RegisterDispatchTask(RpcHttpConnectionRegistrationDispatch rpcHttpConnectionRegistrationDispatch, CancelableAsyncCallback asyncCallback, object asyncState, Guid associationGroupId, string token, string serverTarget, string sessionCookie, string clientIp, Guid requestId) : base("RegisterDispatchTask", rpcHttpConnectionRegistrationDispatch, asyncCallback, asyncState)
		{
			this.associationGroupId = associationGroupId;
			this.token = token;
			this.serverTarget = serverTarget;
			this.sessionCookie = sessionCookie;
			this.clientIp = clientIp;
			this.requestId = requestId;
		}

		internal override int? InternalExecute()
		{
			return new int?(base.RpcHttpConnectionRegistrationDispatch.EcRegister(this.associationGroupId, this.token, this.serverTarget, this.sessionCookie, this.clientIp, this.requestId, out this.failureMessage, out this.failureDetails));
		}

		public int End(out string failureMessage, out string failureDetails)
		{
			int result = base.CheckCompletion();
			failureMessage = this.failureMessage;
			failureDetails = this.failureDetails;
			return result;
		}

		private readonly Guid associationGroupId;

		private readonly string token;

		private readonly string serverTarget;

		private readonly string sessionCookie;

		private readonly string clientIp;

		private readonly Guid requestId;

		private string failureMessage;

		private string failureDetails;
	}
}
