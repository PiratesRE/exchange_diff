using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class MdbInferenceFactory
	{
		protected MdbInferenceFactory()
		{
		}

		public static MdbInferenceFactory Current
		{
			get
			{
				return MdbInferenceFactory.instance.Value;
			}
		}

		internal static Hookable<MdbInferenceFactory> Instance
		{
			get
			{
				return MdbInferenceFactory.instance;
			}
		}

		internal IDocument CreateTrainingSubDocument(IEnumerable<MdbCompositeItemIdentity> itemIterator, int maxDocumentCount, Guid mailboxGuid, Guid mdbGuid)
		{
			return this.CreateTrainingSubDocument(itemIterator, maxDocumentCount, mailboxGuid, mdbGuid, null);
		}

		internal IDocument CreateTrainingSubDocument(IEnumerable<MdbCompositeItemIdentity> itemIterator, int maxDocumentCount, Guid mailboxGuid, Guid mdbGuid, IEnumerable<Tuple<PropertyDefinition, object>> propertiesForAllNestedDocuments)
		{
			DocumentFactory documentFactory = DocumentFactory.Current;
			IDocument document = (Document)documentFactory.CreateDocument(new SimpleIdentity<Guid>(mailboxGuid), DocumentOperation.Insert, null, true);
			foreach (MdbCompositeItemIdentity mdbCompositeItemIdentity in itemIterator)
			{
				Document document2 = (Document)documentFactory.CreateDocument(new MdbCompositeItemIdentity(mdbGuid, mailboxGuid, mdbCompositeItemIdentity.ItemId, 1), DocumentOperation.Insert);
				if (propertiesForAllNestedDocuments != null)
				{
					foreach (Tuple<PropertyDefinition, object> tuple in propertiesForAllNestedDocuments)
					{
						document2.SetProperty(tuple.Item1, tuple.Item2);
					}
				}
				document.AddDocument(document2);
				if (document.NestedDocuments.Count == maxDocumentCount)
				{
					break;
				}
			}
			return document;
		}

		internal Document CreateDocument(Guid mailboxGuid)
		{
			return this.CreateDocument(mailboxGuid, false);
		}

		internal Document CreateDocument(Guid mailboxGuid, bool initializeNestedDocuments)
		{
			Util.ThrowOnNullArgument(mailboxGuid, "mailboxGuid");
			DocumentFactory documentFactory = DocumentFactory.Current;
			return (Document)documentFactory.CreateDocument(new SimpleIdentity<Guid>(mailboxGuid), DocumentOperation.Insert, null, initializeNestedDocuments);
		}

		internal MdbDocument CreateFullDocument(IDocument miniDocument, DocumentProcessingContext documentProcessingContext, MdbPropertyMap propertyMap, PropertyDefinition[] propertySet)
		{
			Util.ThrowOnNullArgument(miniDocument, "miniDocument");
			Util.ThrowOnMismatchType<Document>(miniDocument, "miniDocument");
			Util.ThrowOnNullArgument(miniDocument.Identity, "miniDocument.Identity");
			Util.ThrowOnMismatchType<MdbCompositeItemIdentity>(miniDocument.Identity, "miniDocument.Identity");
			Util.ThrowOnNullArgument(documentProcessingContext, "documentProcessingContext");
			Util.ThrowOnNullArgument(documentProcessingContext.Session, "documentProcessingContext.Session");
			MdbDocument mdbDocument = new MdbDocument((MdbCompositeItemIdentity)miniDocument.Identity, propertySet, documentProcessingContext.Session, propertyMap, miniDocument.Operation);
			((Document)miniDocument).CopyPropertiesTo(mdbDocument);
			return mdbDocument;
		}

		private static readonly Hookable<MdbInferenceFactory> instance = Hookable<MdbInferenceFactory>.Create(true, new MdbInferenceFactory());
	}
}
