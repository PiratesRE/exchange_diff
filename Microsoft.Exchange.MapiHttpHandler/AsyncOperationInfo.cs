using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AsyncOperationInfo
	{
		public AsyncOperationInfo(string requestType, string requestId, string sequenceCookie, string sourceCafeServer, string cafeActivityId, string clientAddress)
		{
			this.StartTime = ExDateTime.UtcNow;
			this.RequestType = requestType;
			this.RequestId = requestId;
			this.SequenceCookie = sequenceCookie;
			this.SourceCafeServer = sourceCafeServer;
			this.CafeActivityId = cafeActivityId;
			this.ClientAddress = clientAddress;
			this.LastPendingTime = null;
			this.PendingCount = 0;
			this.FailureException = null;
			this.EndTime = null;
		}

		public ExDateTime? LastPendingTime { get; private set; }

		public int PendingCount { get; private set; }

		public Exception FailureException { get; private set; }

		public ExDateTime? EndTime { get; private set; }

		public void OnPendingSent()
		{
			this.PendingCount++;
			this.LastPendingTime = new ExDateTime?(ExDateTime.UtcNow);
		}

		public void OnComplete(Exception failureException)
		{
			this.FailureException = failureException;
			this.EndTime = new ExDateTime?(ExDateTime.UtcNow);
		}

		public readonly ExDateTime StartTime;

		public readonly string RequestType;

		public readonly string RequestId;

		public readonly string SequenceCookie;

		public readonly string SourceCafeServer;

		public readonly string CafeActivityId;

		public readonly string ClientAddress;
	}
}
