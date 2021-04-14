using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractMailboxSession : AbstractStoreSession, IMailboxSession, IStoreSession, IDisposable
	{
		public virtual DefaultFolderType IsDefaultFolderType(StoreId folderId)
		{
			throw new NotImplementedException();
		}

		public virtual IUserConfigurationManager UserConfigurationManager
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual StoreObjectId GetDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			throw new NotImplementedException();
		}

		public virtual StoreObjectId CreateDefaultFolder(DefaultFolderType defaultFolderType)
		{
			throw new NotImplementedException();
		}

		public virtual string ClientInfoString
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual CultureInfo PreferedCulture
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsMailboxOof()
		{
			throw new NotImplementedException();
		}

		public virtual bool IsGroupMailbox()
		{
			throw new NotImplementedException();
		}

		public ContactFolders ContactFolders
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void DeleteDefaultFolder(DefaultFolderType defaultFolderType, DeleteItemFlags deleteItemFlags)
		{
			throw new NotImplementedException();
		}

		public CumulativeRPCPerformanceStatistics GetStoreCumulativeRPCStats()
		{
			throw new NotImplementedException();
		}

		public virtual bool TryFixDefaultFolderId(DefaultFolderType defaultFolderType, out StoreObjectId id)
		{
			throw new NotImplementedException();
		}
	}
}
