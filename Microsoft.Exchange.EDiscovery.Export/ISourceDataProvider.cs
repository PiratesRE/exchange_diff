using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface ISourceDataProvider
	{
		string GetRootFolder(string mailboxEmailAddress, bool isArchive);

		List<ItemId> SearchMailboxes(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, ref string pageItemReference, out List<ErrorRecord> failedMailboxes, bool isArchive, string searchName = null);

		List<ItemId> SearchMailboxes(string mailboxEmailAddress, string query, string language, IEnumerable<string> mailboxIds, bool isUnsearchable, ref string pageItemReference, out List<ErrorRecord> failedMailboxes, out bool newSchemaSearchSucceeded, bool isArchive, string searchConfiguration = null);

		List<ItemInformation> ExportItems(string mailboxEmailAddress, IList<ItemId> messageIds, bool isDocIdHintFlighted = false);

		void GetAllFolders(string mailboxEmailAddress, string parentFolderId, bool isDeepTraversal, bool isArchive, Dictionary<string, string> resultFolderInformations);

		List<UnsearchableItemId> GetUnsearchableItems(string mailboxEmailAddress, string mailboxId, ref string pageItemReference);

		void FillInUnsearchableItemIds(string mailboxEmailAddress, IList<UnsearchableItemId> itemIds);

		List<SourceInformation.SourceConfiguration> RetrieveSearchConfiguration(string searchName, out string language, string mailboxEmailAddress = null);

		void UpdateDGExpansion(string mailboxEmailAddress, IList<ItemId> itemIds);

		string GetPhysicalPartitionIdentifier(ItemId itemId);

		object GetItemHashObject(string itemHash);
	}
}
