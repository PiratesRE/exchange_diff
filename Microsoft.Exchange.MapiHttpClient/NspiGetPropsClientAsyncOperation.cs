using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiGetPropsClientAsyncOperation : ClientAsyncOperation
	{
		public NspiGetPropsClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "GetProps";
			}
		}

		public void Begin(NspiGetPropsRequest getPropsRequest)
		{
			base.InternalBegin(new Action<Writer>(getPropsRequest.Serialize));
		}

		public ErrorCode End(out NspiGetPropsResponse getPropsResponse)
		{
			NspiGetPropsResponse localGetPropsResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localGetPropsResponse = new NspiGetPropsResponse(reader);
				return (int)localGetPropsResponse.ReturnCode;
			});
			getPropsResponse = localGetPropsResponse;
			return result;
		}
	}
}
