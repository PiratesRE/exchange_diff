using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PingClientAsyncOperation : ClientAsyncOperation
	{
		public PingClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "PING";
			}
		}

		public void Begin()
		{
			base.InternalBegin(null);
		}

		public ErrorCode End()
		{
			return base.InternalEnd(null);
		}
	}
}
