using System;

namespace Microsoft.Exchange.Net
{
	public interface ICancelableAsyncResult : IAsyncResult
	{
		bool IsCanceled { get; }

		void Cancel();
	}
}
