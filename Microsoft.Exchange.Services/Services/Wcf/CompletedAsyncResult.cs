using System;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CompletedAsyncResult : AsyncResultBase
	{
		public CompletedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
		{
			base.Complete(true);
		}

		public static void End(IAsyncResult result)
		{
			AsyncResultBase.End<CompletedAsyncResult>(result);
		}
	}
}
