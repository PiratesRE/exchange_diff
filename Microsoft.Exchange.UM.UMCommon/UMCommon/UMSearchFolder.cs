using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class UMSearchFolder : DisposableBase
	{
		protected UMSearchFolder(MailboxSession itemStore)
		{
			this.itemStore = itemStore;
		}

		internal StoreObjectId SearchFolderId
		{
			get
			{
				if (this.searchFolder == null)
				{
					return this.GetSearchFolderId();
				}
				return this.searchFolder.Id.ObjectId;
			}
		}

		internal int UnreadCount
		{
			get
			{
				return (int)this.SearchFolder.GetProperties(new PropertyDefinition[]
				{
					FolderSchema.UnreadCount
				})[0];
			}
		}

		protected internal SearchFolder SearchFolder
		{
			get
			{
				if (this.searchFolder == null)
				{
					this.CreateSearchFolder();
				}
				return this.searchFolder;
			}
			protected set
			{
				this.searchFolder = value;
			}
		}

		protected internal MailboxSession ItemStore
		{
			get
			{
				return this.itemStore;
			}
			protected set
			{
				this.itemStore = value;
			}
		}

		internal static UMSearchFolder Get(MailboxSession itemStore, UMSearchFolder.Type folderType)
		{
			UMSearchFolder result = null;
			switch (folderType)
			{
			case UMSearchFolder.Type.VoiceMail:
				result = new VoiceMailSearchFolder(itemStore);
				break;
			case UMSearchFolder.Type.Fax:
				result = new FaxSearchFolder(itemStore);
				break;
			}
			return result;
		}

		internal void CreateSearchFolder()
		{
			if (this.SearchFolderId != null)
			{
				try
				{
					this.searchFolder = SearchFolder.Bind(this.itemStore, this.SearchFolderId);
					return;
				}
				catch (ObjectNotFoundException arg)
				{
					ExTraceGlobals.UtilTracer.TraceError<ObjectNotFoundException>(0L, "Could not bind to UM search folder. Attempting to repair folder. Exception='{0}'", arg);
					if (!this.InternalTryRepairSearchFolder())
					{
						throw;
					}
					return;
				}
			}
			this.InternalCreateSearchFolder();
		}

		internal void DeleteSearchFolder()
		{
			if (this.SearchFolderId != null)
			{
				this.InternalDeleteSearchFolder();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.searchFolder != null)
			{
				this.searchFolder.Dispose();
				this.searchFolder = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMSearchFolder>(this);
		}

		protected abstract StoreObjectId GetSearchFolderId();

		protected abstract void InternalCreateSearchFolder();

		protected abstract void InternalDeleteSearchFolder();

		protected abstract bool InternalTryRepairSearchFolder();

		private SearchFolder searchFolder;

		private MailboxSession itemStore;

		internal enum Type
		{
			VoiceMail,
			Fax
		}
	}
}
