using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiUnbindClientAsyncOperation : ClientAsyncOperation
	{
		public NspiUnbindClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "Unbind";
			}
		}

		public void Begin(NspiUnbindRequest unbindRequest)
		{
			base.InternalBegin(new Action<Writer>(unbindRequest.Serialize));
		}

		public ErrorCode End(out NspiUnbindResponse unbindResponse)
		{
			NspiUnbindResponse localUnbindResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localUnbindResponse = new NspiUnbindResponse(reader);
				return (int)localUnbindResponse.ReturnCode;
			});
			unbindResponse = localUnbindResponse;
			return result;
		}
	}
}
