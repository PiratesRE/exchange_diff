using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PendingRequestEventData
	{
		internal PendingRequestEventData(OwaAsyncResult asyncResult, ChunkedHttpResponse response)
		{
			this.asyncResult = asyncResult;
			this.response = response;
		}

		internal OwaAsyncResult AsyncResult
		{
			get
			{
				return this.asyncResult;
			}
		}

		internal ChunkedHttpResponse Response
		{
			get
			{
				return this.response;
			}
		}

		private OwaAsyncResult asyncResult;

		private ChunkedHttpResponse response;
	}
}
