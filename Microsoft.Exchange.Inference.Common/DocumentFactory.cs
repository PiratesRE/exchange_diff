using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Common
{
	internal class DocumentFactory
	{
		protected DocumentFactory()
		{
		}

		internal static Hookable<DocumentFactory> Instance
		{
			get
			{
				return DocumentFactory.instance;
			}
		}

		internal static DocumentFactory Current
		{
			get
			{
				return DocumentFactory.instance.Value;
			}
		}

		internal IDocument CreateDocument(IIdentity documentId, DocumentOperation documentOperation)
		{
			return this.CreateDocument(documentId, documentOperation, null);
		}

		internal IDocument CreateDocument(IIdentity documentId, DocumentOperation documentOperation, IDocumentAdapter documentAdapter)
		{
			return this.CreateDocument(documentId, documentOperation, documentAdapter, false);
		}

		internal IDocument CreateDocument(IIdentity documentId, DocumentOperation documentOperation, IDocumentAdapter documentAdapter, bool initializeNestedDocuments)
		{
			Util.ThrowOnNullArgument(documentId, "documentId");
			return new Document(documentId, documentOperation, documentAdapter, initializeNestedDocuments);
		}

		private static readonly Hookable<DocumentFactory> instance = Hookable<DocumentFactory>.Create(true, new DocumentFactory());
	}
}
