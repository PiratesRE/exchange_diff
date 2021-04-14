using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DefaultFolderContext
	{
		internal DefaultFolderContext(MailboxSession session, DefaultFolder[] defaultFolders)
		{
			this.mailboxSession = session;
			this.defaultFolders = defaultFolders;
			this.folderDataDictionary = null;
		}

		internal MailboxSession Session
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal Dictionary<string, DefaultFolderManager.FolderData> FolderDataDictionary
		{
			get
			{
				return this.folderDataDictionary;
			}
			set
			{
				this.folderDataDictionary = value;
			}
		}

		internal bool DeferFolderIdInit
		{
			get
			{
				return this.deferFolderIdInit;
			}
			set
			{
				this.deferFolderIdInit = value;
			}
		}

		internal bool IgnoreForcedFolderInit { get; set; }

		internal StoreObjectId this[DefaultFolderType defaultFolderType]
		{
			get
			{
				EnumValidator.AssertValid<DefaultFolderType>(defaultFolderType);
				DefaultFolder defaultFolder = this.defaultFolders[(int)defaultFolderType];
				if (defaultFolder != null)
				{
					StoreObjectId result;
					defaultFolder.TryGetFolderId(out result);
					return result;
				}
				return null;
			}
		}

		internal PropertyBag GetMailboxPropertyBag()
		{
			MemoryPropertyBag memoryPropertyBag = this.mailboxPropertyBag;
			if (memoryPropertyBag == null)
			{
				memoryPropertyBag = DefaultFolderContext.SaveLocationContainer(this.Session.Mailbox, DefaultFolderInfo.MailboxProperties);
				if (this.isSessionOpenStage)
				{
					this.mailboxPropertyBag = memoryPropertyBag;
				}
			}
			return memoryPropertyBag;
		}

		internal PropertyBag GetInboxOrConfigurationFolderPropertyBag()
		{
			MemoryPropertyBag memoryPropertyBag = this.inboxConfigurationPropertyBag;
			if (memoryPropertyBag == null)
			{
				using (Folder folder = this.OpenInboxOrConfigurationFolder())
				{
					memoryPropertyBag = DefaultFolderContext.SaveLocationContainer(folder, DefaultFolderInfo.InboxOrConfigurationFolderProperties);
				}
				if (this.isSessionOpenStage)
				{
					this.inboxConfigurationPropertyBag = memoryPropertyBag;
				}
			}
			return memoryPropertyBag;
		}

		internal void DoneDefaultFolderInitialization()
		{
			if (!this.isSessionOpenStage)
			{
				throw new InvalidOperationException("Not expected to be called twice");
			}
			this.isSessionOpenStage = false;
			this.mailboxSession.SharedDataManager.DefaultFoldersInitialized = true;
			this.folderDataDictionary = null;
			this.mailboxPropertyBag = null;
			this.inboxConfigurationPropertyBag = null;
		}

		private static MemoryPropertyBag SaveLocationContainer(IStorePropertyBag storeObject, StorePropertyDefinition[] properties)
		{
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			memoryPropertyBag.PreLoadStoreProperty<StorePropertyDefinition>(properties, storeObject.GetProperties(properties));
			return memoryPropertyBag;
		}

		private Folder OpenInboxOrConfigurationFolder()
		{
			StoreObjectId storeObjectId = this[DefaultFolderType.Configuration];
			StoreObjectId storeObjectId2 = this[DefaultFolderType.Inbox];
			if (storeObjectId == null)
			{
				throw new InvalidOperationException("Wrong order of default folders' initialization - No configuration folder information.");
			}
			if (storeObjectId2 == null)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string>(-1L, "DefaultFolderContext::OpenInboxOrConfigurationFolder.  Unable to find StoreObjectId for inboxId. Wrong order of default folders' initialization - Inbox for {0} should be loaded before other non-free ones.", this.Session.DisplayName);
			}
			else
			{
				try
				{
					return Folder.Bind(this.Session, storeObjectId2, DefaultFolderInfo.InboxOrConfigurationFolderProperties);
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string>(-1L, "DefaultFolderContext::OpenInboxOrConfigurationFolder. We cannot bind to the Inbox of Mailbox = {0}.", this.Session.DisplayName);
				}
			}
			return Folder.Bind(this.Session, storeObjectId, DefaultFolderInfo.InboxOrConfigurationFolderProperties);
		}

		private readonly MailboxSession mailboxSession;

		private readonly DefaultFolder[] defaultFolders;

		private Dictionary<string, DefaultFolderManager.FolderData> folderDataDictionary;

		private MemoryPropertyBag inboxConfigurationPropertyBag;

		private MemoryPropertyBag mailboxPropertyBag;

		private bool isSessionOpenStage = true;

		private bool deferFolderIdInit;
	}
}
