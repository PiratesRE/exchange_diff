using System;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Fast
{
	internal class FastDocumentHelper : IFastDocumentHelper
	{
		public void PopulateFastDocumentForFolderUpdate(IFastDocument fastDocument, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity, string folderEntryId)
		{
			fastDocument.FlowOperation = "FolderUpdate";
			fastDocument.TenantId = mailboxGuid;
			fastDocument.IsMoveDestination = isMoveDestination;
			fastDocument.IsLocalMdb = isLocalMdb;
			fastDocument.CompositeItemId = identity.ToString();
			fastDocument.IndexId = IndexId.CreateIndexId(mailboxNumber, documentId);
			fastDocument.FolderId = folderEntryId;
			this.ValidateDocumentConsistency(fastDocument, "FastDocumentHelper.PopulateFastDocumentForFolderUpdate");
		}

		public void PopulateFastDocumentForWatermarkUpdate(IFastDocument fastDocument, long fastId, long watermark, bool recrawlMailbox)
		{
			fastDocument.FlowOperation = "WatermarkUpdate";
			fastDocument.IndexId = fastId;
			fastDocument.TenantId = WatermarkStorageId.FastWatermarkTenantId;
			fastDocument.MailboxGuid = WatermarkStorageId.FastWatermarkTenantId;
			fastDocument.Watermark = watermark;
			if (recrawlMailbox)
			{
				fastDocument.ErrorCode = 203;
			}
		}

		public void PopulateFastDocumentForDelete(IFastDocument fastDocument, Guid mailboxGuid, long documentId)
		{
			fastDocument.FlowOperation = "Delete";
			fastDocument.TenantId = mailboxGuid;
			fastDocument.IndexId = documentId;
		}

		public void PopulateFastDocumentForDeleteSelection(IFastDocument fastDocument, Guid mailboxGuid)
		{
			fastDocument.FlowOperation = "DeleteSelection";
			fastDocument.TenantId = mailboxGuid;
			fastDocument.IndexId = -1L;
		}

		public void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, bool isMoveDestination, bool isLocalMdb, long documentId, IIdentity identity)
		{
			this.PopulateFastDocumentForIndexing(fastDocument, version, mailboxGuid, isMoveDestination, isLocalMdb, documentId, identity.ToString(), 0, 0);
		}

		public void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity)
		{
			this.PopulateFastDocumentForIndexing(fastDocument, version, mailboxGuid, isMoveDestination, isLocalMdb, IndexId.CreateIndexId(mailboxNumber, documentId), identity.ToString(), 0, 0);
		}

		public void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, int mailboxNumber, bool isMoveDestination, bool isLocalMdb, int documentId, IIdentity identity, int errorCode, int attemptCount)
		{
			this.PopulateFastDocumentForIndexing(fastDocument, version, mailboxGuid, isMoveDestination, isLocalMdb, IndexId.CreateIndexId(mailboxNumber, documentId), identity.ToString(), errorCode, attemptCount);
		}

		public void PopulateFastDocumentForIndexing(IFastDocument fastDocument, int version, Guid mailboxGuid, bool isMoveDestination, bool isLocalMdb, long documentId, string identity, int errorCode, int attemptCount)
		{
			fastDocument.FeedingVersion = version;
			fastDocument.FlowOperation = "Indexing";
			fastDocument.TenantId = mailboxGuid;
			fastDocument.IsMoveDestination = isMoveDestination;
			fastDocument.IsLocalMdb = isLocalMdb;
			fastDocument.MailboxGuid = mailboxGuid;
			fastDocument.CompositeItemId = identity;
			fastDocument.DocumentId = documentId;
			fastDocument.IndexId = documentId;
			if (errorCode != 0)
			{
				fastDocument.ErrorCode = errorCode;
			}
			if (attemptCount != 0)
			{
				fastDocument.AttemptCount = attemptCount;
			}
			this.ValidateDocumentConsistency(fastDocument, "FastDocumentHelper.PopulateFastDocumentForIndexing");
		}

		public void ValidateDocumentConsistency(IFastDocument fastDocument, string context)
		{
			if (fastDocument.FlowOperation != null && fastDocument.FlowOperation != "WatermarkUpdate" && !string.IsNullOrEmpty(fastDocument.IndexSystemName))
			{
				if (IndexId.IsWatermarkIndexId(fastDocument.IndexId))
				{
					throw new DocumentValidationException(string.Format("FastDocument with FlowOperation type: {0} has a Watermark Id: {1}, Context: {2}", fastDocument.FlowOperation, fastDocument.IndexId, context));
				}
				if (fastDocument.MailboxGuid.Equals(WatermarkStorageId.FastWatermarkTenantId))
				{
					throw new DocumentValidationException(string.Format("FastDocument with FlowOperation type: {0} has the WatermarkTenantId as it's MailboxGuid. Context: {1}", fastDocument.FlowOperation, context));
				}
			}
		}
	}
}
