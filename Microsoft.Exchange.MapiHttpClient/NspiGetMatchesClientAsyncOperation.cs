using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetMatchesClientAsyncOperation : ClientAsyncOperation
	{
		public NspiGetMatchesClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "GetMatches";
			}
		}

		public void Begin(NspiGetMatchesRequest getMatchesRequest)
		{
			base.InternalBegin(new Action<Writer>(getMatchesRequest.Serialize));
		}

		public ErrorCode End(out NspiGetMatchesResponse getMatchesResponse)
		{
			NspiGetMatchesResponse localGetMatchesResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localGetMatchesResponse = new NspiGetMatchesResponse(reader);
				return (int)localGetMatchesResponse.ReturnCode;
			});
			getMatchesResponse = localGetMatchesResponse;
			return result;
		}
	}
}
