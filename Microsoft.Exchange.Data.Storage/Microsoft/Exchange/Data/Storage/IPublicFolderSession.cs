using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPublicFolderSession : IStoreSession, IDisposable
	{
		bool IsPrimaryHierarchySession { get; }

		IExchangePrincipal MailboxPrincipal { get; }

		StoreObjectId GetPublicFolderRootId();

		StoreObjectId GetTombstonesRootFolderId();

		StoreObjectId GetAsyncDeleteStateFolderId();
	}
}
