using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetPropListClientAsyncOperation : ClientAsyncOperation
	{
		public NspiGetPropListClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "GetPropList";
			}
		}

		public void Begin(NspiGetPropListRequest getPropListRequest)
		{
			base.InternalBegin(new Action<Writer>(getPropListRequest.Serialize));
		}

		public ErrorCode End(out NspiGetPropListResponse getPropListResponse)
		{
			NspiGetPropListResponse localGetPropListResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localGetPropListResponse = new NspiGetPropListResponse(reader);
				return (int)localGetPropListResponse.ReturnCode;
			});
			getPropListResponse = localGetPropListResponse;
			return result;
		}
	}
}
