using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IExecutable : IDiagnosable, IDisposable
	{
		string InstanceName { get; }

		ICancelableAsyncResult AsyncResult { get; }

		IAsyncResult BeginExecute(AsyncCallback callback, object state);

		void EndExecute(IAsyncResult asyncResult);

		void CancelExecute();
	}
}
