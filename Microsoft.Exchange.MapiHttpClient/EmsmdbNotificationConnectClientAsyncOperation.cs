using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbNotificationConnectClientAsyncOperation : ClientAsyncOperation
	{
		public EmsmdbNotificationConnectClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return string.Empty;
			}
		}

		public void Begin()
		{
			base.InvokeCallback();
		}

		public ErrorCode End()
		{
			return ErrorCode.None;
		}
	}
}
