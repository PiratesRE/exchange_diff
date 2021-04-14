using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDocumentProcessor
	{
		void ProcessDocument(IDocument document, object context);

		IAsyncResult BeginProcess(IDocument document, AsyncCallback callback, object context);

		void EndProcess(IAsyncResult asyncResult);
	}
}
