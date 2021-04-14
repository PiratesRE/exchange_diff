using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiResortRestrictionClientAsyncOperation : ClientAsyncOperation
	{
		public NspiResortRestrictionClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "ResortRestriction";
			}
		}

		public void Begin(NspiResortRestrictionRequest request)
		{
			base.InternalBegin(new Action<Writer>(request.Serialize));
		}

		public ErrorCode End(out NspiResortRestrictionResponse response)
		{
			NspiResortRestrictionResponse localResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localResponse = new NspiResortRestrictionResponse(reader);
				return (int)localResponse.ReturnCode;
			});
			response = localResponse;
			return result;
		}
	}
}
