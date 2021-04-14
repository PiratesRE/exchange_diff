using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NspiModPropsClientAsyncOperation : ClientAsyncOperation
	{
		public NspiModPropsClientAsyncOperation(ClientSessionContext context, CancelableAsyncCallback asyncCallback, object asyncState) : base(context, asyncCallback, asyncState)
		{
		}

		protected override string RequestType
		{
			get
			{
				return "ModProps";
			}
		}

		public void Begin(NspiModPropsRequest modPropsRequest)
		{
			base.InternalBegin(new Action<Writer>(modPropsRequest.Serialize));
		}

		public ErrorCode End(out NspiModPropsResponse modPropsResponse)
		{
			NspiModPropsResponse localModPropsResponse = null;
			ErrorCode result = base.InternalEnd(delegate(Reader reader)
			{
				localModPropsResponse = new NspiModPropsResponse(reader);
				return (int)localModPropsResponse.ReturnCode;
			});
			modPropsResponse = localModPropsResponse;
			return result;
		}
	}
}
