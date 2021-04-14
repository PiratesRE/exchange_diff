using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface ISubmitDocument : IDisposable
	{
		TimeSpan SubmissionTimeout { get; set; }

		IFastDocumentHelper DocumentHelper { get; }

		IDocumentTracker Tracker { get; set; }

		string IndexSystemName { get; set; }

		void Initialize();

		ICancelableAsyncResult BeginSubmitDocument(IFastDocument document, AsyncCallback callback, object state);

		bool EndSubmitDocument(IAsyncResult asyncResult);

		bool TryCompleteSubmitDocument(IAsyncResult asyncResult);

		IFastDocument CreateFastDocument(DocumentOperation operation);
	}
}
