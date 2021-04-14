using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFastDocumentHelper
	{
		void PopulateFastDocumentForFolderUpdate(IFastDocument fastDocument, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity, string folderEntryId);

		void PopulateFastDocumentForWatermarkUpdate(IFastDocument fastDocument, long fastId, long watermark, bool recrawlMailbox);

		void PopulateFastDocumentForDelete(IFastDocument fastDocument, Guid mailboxGuid, long documentId);

		void PopulateFastDocumentForDeleteSelection(IFastDocument fastDocument, Guid mailboxGuid);

		void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, bool isMoveDestination, bool isLocalMdb, long documentId, IIdentity identity);

		void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity);

		void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity, int errorCode, int attemptCount);

		void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, bool isMoveDestination, bool isLocalMdb, long documentId, string identity, int errorCode, int attemptCount);

		void ValidateDocumentConsistency(IFastDocument fastDocument, string context);
	}
}
