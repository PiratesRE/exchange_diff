using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationDataProvider : IDisposable
	{
		IMigrationADProvider ADProvider { get; }

		string TenantName { get; }

		string MailboxName { get; }

		Guid MdbGuid { get; }

		IMigrationStoreObject Folder { get; }

		ADObjectId OwnerId { get; }

		OrganizationId OrganizationId { get; }

		IMigrationRunspaceProxy RunspaceProxy { get; }

		IEnumerable<StoreObjectId> FindMessageIds(MigrationEqualityFilter primaryFilter, PropertyDefinition[] filterColumns, SortBy[] additionalSorts, MigrationRowSelector rowSelectorPredicate, int? maxCount);

		IEnumerable<StoreObjectId> FindMessageIds(QueryFilter filter, PropertyDefinition[] properties, SortBy[] sortBy, MigrationRowSelector rowSelectorPredicate, int? maxCount);

		IMigrationMessageItem FindMessage(StoreObjectId messageId, PropertyDefinition[] properties);

		IMigrationStoreObject GetRootFolder(PropertyDefinition[] properties);

		int CountMessages(QueryFilter filter, SortBy[] sortBy);

		object[] QueryRow(QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] propertyDefinitions);

		IEnumerable<object[]> QueryRows(QueryFilter filter, SortBy[] sortBy, PropertyDefinition[] propertyDefinitions, int pageSize);

		IMigrationMessageItem CreateMessage();

		IMigrationEmailMessageItem CreateEmailMessage();

		void RemoveMessage(StoreObjectId messageId);

		bool MoveMessageItems(StoreObjectId[] itemsToMove, MigrationFolderName folderName);

		IMigrationDataProvider GetProviderForFolder(MigrationFolderName folderName);

		Uri GetEcpUrl();

		void FlushReport(ReportData reportData);

		void LoadReport(ReportData reportData);

		void DeleteReport(ReportData reportData);
	}
}
