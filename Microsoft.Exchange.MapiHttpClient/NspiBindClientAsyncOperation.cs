using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiBindClientAsyncOperation : ClientAsyncOperation
	{
		public NspiBindClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "Bind";
			}
		}

		public void Begin(NspiBindRequest bindRequest)
		{
			base.InternalBegin(new Action<Writer>(bindRequest.Serialize));
		}

		public ErrorCode End(out NspiBindResponse bindResponse)
		{
			NspiBindResponse localBindResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localBindResponse = new NspiBindResponse(reader);
				return (int)localBindResponse.ReturnCode;
			});
			bindResponse = localBindResponse;
			return result;
		}
	}
}
