using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbExecuteClientAsyncOperation : ClientAsyncOperation
	{
		public EmsmdbExecuteClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "Execute";
			}
		}

		public void Begin(EmsmdbExecuteRequest request)
		{
			base.InternalBegin(new Action<Writer>(request.Serialize));
		}

		public ErrorCode End(out EmsmdbExecuteResponse response)
		{
			EmsmdbExecuteResponse localResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localResponse = new EmsmdbExecuteResponse(reader);
				return (int)localResponse.ReturnCode;
			});
			response = localResponse;
			return result;
		}
	}
}
