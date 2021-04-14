using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDocument : IPropertyBag, IReadOnlyPropertyBag
	{
		IIdentity Identity { get; }

		DocumentOperation Operation { get; }

		ICollection<DocumentFailureDescription> Failures { get; }

		ReadOnlyCollection<IDocument> NestedDocuments { get; }

		void AddDocument(IDocument document);

		int RemoveDocuments(ICollection<IDocument> documentsToRemove);
	}
}
