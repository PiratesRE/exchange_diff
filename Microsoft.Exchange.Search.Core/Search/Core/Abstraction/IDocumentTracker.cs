using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IDocumentTracker : IDiagnosable
	{
		int PoisonDocumentsCount { get; }

		void Initialize(IFailedItemStorage failedItemStorage);

		void RecordDocumentProcessing(Guid instance, Guid correlationId, long docId);

		void RecordDocumentProcessingComplete(Guid correlationId, long docId, bool isTrackedAsPoison);

		void MarkCurrentlyTrackedDocumentsAsPoison();

		void MarkDocumentAsPoison(long docId);

		void MarkDocumentAsRetriablePoison(long docId);

		int ShouldDocumentBeStampedWithError(long docId);

		bool ShouldDocumentBeSkipped(long docId);
	}
}
