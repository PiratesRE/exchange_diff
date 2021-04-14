using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAnchorDataProvider : IDisposable
	{
		IAnchorADProvider ADProvider { get; }

		string TenantName { get; }

		string MailboxName { get; }

		Guid MdbGuid { get; }

		IAnchorStoreObject Folder { get; }

		ADObjectId OwnerId { get; }

		OrganizationId OrganizationId { get; }

		AnchorContext AnchorContext { get; }

		IAnchorStoreObject GetFolderByName(string folderName, PropertyDefinition[] properties);

		IAnchorMessageItem CreateMessage();

		IAnchorEmailMessageItem CreateEmailMessage();

		void RemoveMessage(StoreObjectId messageId);

		bool MoveMessageItems(StoreObjectId[] itemsToMove, string folderName);

		IAnchorDataProvider GetProviderForFolder(AnchorContext context, string folderName);

		IEnumerable<StoreObjectId> FindMessageIds(QueryFilter queryFilter, PropertyDefinition[] properties, SortBy[] sortBy, AnchorRowSelector rowSelector, int? maxCount);

		IAnchorMessageItem FindMessage(StoreObjectId messageId, PropertyDefinition[] properties);
	}
}
