using System;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal struct ProgressInfo
	{
		public bool IsCompleted;

		public uint CompletedTaskCount;

		public uint TotalTaskCount;

		public Func<object, IProgressResultFactory, RopResult> CreateCompleteResult;

		public Func<object, ProgressResultFactory, RopResult> CreateCompleteResultForProgress;
	}
}
