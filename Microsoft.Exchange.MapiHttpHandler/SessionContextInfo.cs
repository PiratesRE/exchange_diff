using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionContextInfo
	{
		public SessionContextInfo(ExDateTime creationTime, SessionRundownReason? rundownReason, ExDateTime rundownTime, int activityCount, ExDateTime lastActivityTime, string contextCookie, AsyncOperationInfo[] activeAsyncOperations, AsyncOperationInfo[] completedAsyncOperations, AsyncOperationInfo[] failedAsyncOperations)
		{
			this.CreationTime = creationTime;
			this.RundownReason = rundownReason;
			this.RundownTime = rundownTime;
			this.ActivityCount = activityCount;
			this.LastActivityTime = lastActivityTime;
			this.ContextCookie = contextCookie;
			this.ActiveAsyncOperations = activeAsyncOperations;
			this.CompletedAsyncOperations = completedAsyncOperations;
			this.FailedAsyncOperations = failedAsyncOperations;
		}

		public readonly ExDateTime CreationTime;

		public readonly SessionRundownReason? RundownReason;

		public readonly ExDateTime RundownTime;

		public readonly int ActivityCount;

		public readonly ExDateTime LastActivityTime;

		public readonly string ContextCookie;

		public readonly AsyncOperationInfo[] ActiveAsyncOperations;

		public readonly AsyncOperationInfo[] CompletedAsyncOperations;

		public readonly AsyncOperationInfo[] FailedAsyncOperations;
	}
}
