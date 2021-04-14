using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RfriGetAddressBookUrlClientAsyncOperation : ClientAsyncOperation
	{
		public RfriGetAddressBookUrlClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "GetNspiUrl";
			}
		}

		public void Begin(RfriGetAddressBookUrlRequest getAddressBookUrlRequest)
		{
			base.InternalBegin(new Action<Writer>(getAddressBookUrlRequest.Serialize));
		}

		public ErrorCode End(out RfriGetAddressBookUrlResponse getAddressBookUrlResponse)
		{
			RfriGetAddressBookUrlResponse localGetAddressBookUrlResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localGetAddressBookUrlResponse = new RfriGetAddressBookUrlResponse(reader);
				return (int)localGetAddressBookUrlResponse.ReturnCode;
			});
			getAddressBookUrlResponse = localGetAddressBookUrlResponse;
			return result;
		}
	}
}
