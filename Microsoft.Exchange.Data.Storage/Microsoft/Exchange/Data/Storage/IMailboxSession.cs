using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxSession : IStoreSession, IDisposable
	{
		DefaultFolderType IsDefaultFolderType(StoreId folderId);

		StoreObjectId GetDefaultFolderId(DefaultFolderType defaultFolderType);

		StoreObjectId CreateDefaultFolder(DefaultFolderType defaultFolderType);

		string ClientInfoString { get; }

		CultureInfo PreferedCulture { get; }

		IUserConfigurationManager UserConfigurationManager { get; }

		ContactFolders ContactFolders { get; }

		bool IsMailboxOof();

		bool IsGroupMailbox();

		void DeleteDefaultFolder(DefaultFolderType defaultFolderType, DeleteItemFlags deleteItemFlags);

		CumulativeRPCPerformanceStatistics GetStoreCumulativeRPCStats();

		bool TryFixDefaultFolderId(DefaultFolderType defaultFolderType, out StoreObjectId id);
	}
}
