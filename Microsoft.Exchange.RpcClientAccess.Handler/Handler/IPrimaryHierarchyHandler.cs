using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPrimaryHierarchyHandler
	{
		StoreId CreateFolder(string folderName, string description, StoreId parentFolderId, CreateFolderFlags flags, out Guid contentMailboxGuid);

		void DeleteFolder(StoreId parentFolderId, StoreId folderId, DeleteFolderFlags flags);

		void MoveFolder(StoreId parentFolderId, StoreId destinationFolderId, StoreId sourceFolderId, string folderName);

		PropertyProblem[] SetProperties(StoreId folderId, PropertyValue[] propertyValues, out Guid contentMailboxGuid);

		PropertyProblem[] DeleteProperties(StoreId folderId, PropertyTag[] propertyTags, out Guid contentMailboxGuid);

		void ModifyPermissions(CoreFolder coreFolder, IModifyTable permissionsTable, IEnumerable<ModifyTableRow> modifyTableRows, ModifyTableOptions options, bool shouldReplaceAllRows);
	}
}
