using System;
using System.Collections.Generic;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IEwsClient : ISourceDataProvider
	{
		void GetSearchResultEstimation(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out int mailboxesSearchedCount, out long itemCount, out long totalSize, out List<ErrorRecord> failedMailboxes, string searchConfiguration = null);

		void GetSearchResultEstimation(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out int mailboxesSearchedCount, bool isUnsearchable, out long itemCount, out long totalSize, out List<ErrorRecord> failedMailboxes, out bool newSchemaSearchSucceeded, string searchConfiguration = null);

		List<KeywordStatisticsSearchResultType> GetKeywordStatistics(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, out List<ErrorRecord> failedMailboxes, string searchConfiguration = null);

		BaseFolderType GetFolderById(string mailboxEmailAddress, BaseFolderIdType folderId);

		BaseFolderType GetFolderByName(string mailboxEmailAddress, BaseFolderIdType parentFolderId, string folderDisplayName);

		List<BaseFolderType> CreateFolder(string mailboxEmailAddress, BaseFolderIdType parentFolderId, BaseFolderType[] folders);

		void DeleteFolder(string mailboxEmailAddress, BaseFolderIdType[] folderIds);

		void MoveFolder(string mailboxEmailAddress, BaseFolderIdType targetFolderId, BaseFolderIdType[] folderIds);

		List<ItemType> RetrieveItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, BasePathToElementType[] additionalProperties, RestrictionType restriction, bool isAssociated, int? pageItemCount, int offset);

		List<ItemType> CreateItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, ItemType[] items);

		List<ItemType> UpdateItems(string mailboxEmailAddress, BaseFolderIdType parentFolderId, ItemChangeType[] itemChanges);

		void DeleteItems(string mailboxEmailAddress, BaseItemIdType[] itemIds);

		List<ItemInformation> UploadItems(string mailboxEmailAddress, FolderIdType parentFolderId, IList<ItemInformation> items, bool alwaysCreateNew);

		long GetUnsearchableItemStatistics(string mailboxEmailAddress, string mailboxId);

		ItemType GetItem(string mailboxEmailAddress, string itemId);

		void SendEmails(string mailboxEmailAddress, MessageType[] emails);

		AttachmentType GetAttachment(string mailboxEmailAddress, string attachmentId);

		List<AttachmentType> CreateAttachments(string mailboxEmailAddress, string itemId, IList<AttachmentType> attachments);

		void DeleteAttachments(string mailboxEmailAddress, IList<string> attachmentIds);
	}
}
