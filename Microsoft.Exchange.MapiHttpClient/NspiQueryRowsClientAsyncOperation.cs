using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiQueryRowsClientAsyncOperation : ClientAsyncOperation
	{
		public NspiQueryRowsClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "QueryRows";
			}
		}

		public void Begin(NspiQueryRowsRequest request)
		{
			base.InternalBegin(new Action<Writer>(request.Serialize));
		}

		public ErrorCode End(out NspiQueryRowsResponse response)
		{
			NspiQueryRowsResponse localResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localResponse = new NspiQueryRowsResponse(reader);
				return (int)localResponse.ReturnCode;
			});
			response = localResponse;
			return result;
		}
	}
}
