using System;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal sealed class DocumentContext
	{
		internal DocumentContext(IDocument document, AsyncResult asyncResult)
		{
			Util.ThrowOnNullArgument(document, "document");
			Util.ThrowOnNullArgument(asyncResult, "asyncResult");
			this.document = document;
			this.asyncResult = asyncResult;
		}

		internal IDocument Document
		{
			get
			{
				return this.document;
			}
		}

		internal AsyncResult AsyncResult
		{
			get
			{
				return this.asyncResult;
			}
		}

		private readonly IDocument document;

		private readonly AsyncResult asyncResult;
	}
}
