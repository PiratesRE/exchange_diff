using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriGetMailboxUrlClientAsyncOperation : ClientAsyncOperation
	{
		public RfriGetMailboxUrlClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "GetMailboxUrl";
			}
		}

		public void Begin(RfriGetMailboxUrlRequest getMailboxUrlRequest)
		{
			base.InternalBegin(new Action<Writer>(getMailboxUrlRequest.Serialize));
		}

		public ErrorCode End(out RfriGetMailboxUrlResponse getGetMailboxUrlResponse)
		{
			RfriGetMailboxUrlResponse localGetMailboxUrlResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localGetMailboxUrlResponse = new RfriGetMailboxUrlResponse(reader);
				return (int)localGetMailboxUrlResponse.ReturnCode;
			});
			getGetMailboxUrlResponse = localGetMailboxUrlResponse;
			return result;
		}
	}
}
