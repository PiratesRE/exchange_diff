using System;

namespace Microsoft.Exchange.Rpc
{
	public interface IRpcAsyncResult : IAsyncResult
	{
		void Cancel();
	}
}
