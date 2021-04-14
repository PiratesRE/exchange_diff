using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EmsmdbDummyClientAsyncOperation : ClientAsyncOperation
	{
		public EmsmdbDummyClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "EcDoDummy";
			}
		}

		public void Begin(EmsmdbDummyRequest request)
		{
			base.InternalBegin(new Action<Writer>(request.Serialize));
		}

		public ErrorCode End(out EmsmdbDummyResponse response)
		{
			EmsmdbDummyResponse localResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localResponse = new EmsmdbDummyResponse(reader);
				return (int)localResponse.ReturnCode;
			});
			response = localResponse;
			return result;
		}
	}
}
