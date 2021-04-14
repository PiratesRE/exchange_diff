using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractPublicFolderSession : AbstractStoreSession, IPublicFolderSession, IStoreSession, IDisposable
	{
		public virtual bool IsPrimaryHierarchySession
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IExchangePrincipal MailboxPrincipal
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual StoreObjectId GetPublicFolderRootId()
		{
			throw new NotImplementedException();
		}

		public virtual StoreObjectId GetTombstonesRootFolderId()
		{
			throw new NotImplementedException();
		}

		public virtual StoreObjectId GetAsyncDeleteStateFolderId()
		{
			throw new NotImplementedException();
		}
	}
}
