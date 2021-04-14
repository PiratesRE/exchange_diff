using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IStartStop : IDisposable
	{
		IAsyncResult BeginPrepareToStart(AsyncCallback callback, object context);

		void EndPrepareToStart(IAsyncResult asyncResult);

		IAsyncResult BeginStart(AsyncCallback callback, object context);

		void EndStart(IAsyncResult asyncResult);

		IAsyncResult BeginStop(AsyncCallback callback, object context);

		void EndStop(IAsyncResult asyncResult);
	}
}
