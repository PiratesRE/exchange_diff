using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAsyncOperationExecutor
	{
		void BeginOperation(bool useSameThread);

		void EndOperation();

		bool IsCompleted { get; }

		void WaitForStopped();

		void GetProgressInfo(out object progressToken, out ProgressInfo progressInfo);
	}
}
