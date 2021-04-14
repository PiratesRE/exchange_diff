using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbConnectClientAsyncOperation : ClientAsyncOperation
	{
		public EmsmdbConnectClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "Connect";
			}
		}

		public void Begin(EmsmdbConnectRequest request)
		{
			base.InternalBegin(new Action<Writer>(request.Serialize));
		}

		public ErrorCode End(out EmsmdbConnectResponse response)
		{
			EmsmdbConnectResponse localResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localResponse = new EmsmdbConnectResponse(reader);
				return (int)localResponse.ReturnCode;
			});
			response = localResponse;
			return result;
		}
	}
}
