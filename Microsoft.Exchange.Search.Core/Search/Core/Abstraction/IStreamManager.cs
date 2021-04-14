using System;
using System.IO;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IStreamManager
	{
		int ListenPort { get; set; }

		TimeSpan CacheTimeout { get; set; }

		TimeSpan ConnectionTimeout { get; set; }

		void StartListening();

		void StopListening();

		Stream WaitForConnection(Guid contextId);

		ICancelableAsyncResult BeginWaitForConnection(Guid contextId, AsyncCallback callback, object state);

		Stream EndWaitForConnection(IAsyncResult asyncResult);

		Stream Connect(int port, Guid contextId);

		ICancelableAsyncResult BeginConnect(int port, Guid contextId, AsyncCallback callback, object state);

		Stream EndConnect(IAsyncResult asyncResult);

		void CancelPendingOperation(Guid contextId);

		void CheckIn(Stream channel);

		Stream CheckOut(Guid contextId);
	}
}
