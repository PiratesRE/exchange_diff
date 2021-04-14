using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUserConfigurationManager
	{
		IMailboxSession MailboxSession { get; }

		IReadableUserConfiguration GetReadOnlyMailboxConfiguration(string configName, UserConfigurationTypes freefetchDataTypes);

		IReadableUserConfiguration GetReadOnlyFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId);

		IUserConfiguration GetMailboxConfiguration(string configName, UserConfigurationTypes freefetchDataTypes);

		IUserConfiguration GetFolderConfiguration(string configName, UserConfigurationTypes freefetchDataTypes, StoreId folderId);

		OperationResult DeleteMailboxConfigurations(params string[] configurationNames);

		OperationResult DeleteFolderConfigurations(StoreId folderId, params string[] configurationNames);

		IUserConfiguration CreateMailboxConfiguration(string configurationName, UserConfigurationTypes dataTypes);

		IUserConfiguration CreateFolderConfiguration(string configurationName, UserConfigurationTypes dataTypes, StoreId folderId);

		IList<IStorePropertyBag> FetchAllConfigurations(IFolder folder, SortBy[] sorts, int maxRow, params PropertyDefinition[] columns);
	}
}
