using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DefaultFolderInfo
	{
		internal static DefaultFolderInfo[] Instance
		{
			get
			{
				return DefaultFolderInfo.defaultFolderInfos;
			}
		}

		internal static StorePropertyDefinition[] MailboxProperties
		{
			get
			{
				return DefaultFolderInfo.mailboxProperties;
			}
		}

		internal static StorePropertyDefinition[] InboxOrConfigurationFolderProperties
		{
			get
			{
				return DefaultFolderInfo.inboxOrConfigurationFolderProperties;
			}
		}

		internal static int DefaultFolderTypeCount
		{
			get
			{
				return DefaultFolderInfo.defaultFolderTypeCount;
			}
		}

		internal StoreObjectType StoreObjectType
		{
			get
			{
				return this.storeObjectType;
			}
		}

		internal EntryIdStrategy EntryIdStrategy
		{
			get
			{
				return this.entryIdStrategy;
			}
		}

		internal DefaultFolderCreator FolderCreator
		{
			get
			{
				return this.folderCreator;
			}
		}

		internal DefaultFolderBehavior Behavior
		{
			get
			{
				return this.behavior;
			}
		}

		internal DefaultFolderValidator FolderValidationStrategy
		{
			get
			{
				return this.folderValidationStrategy;
			}
		}

		internal DefaultFolderLocalization Localizable
		{
			get
			{
				return this.localizable;
			}
		}

		internal LocalizedString LocalizableDisplayName
		{
			get
			{
				return this.localizableDisplayName;
			}
		}

		internal DefaultFolderType DefaultFolderType
		{
			get
			{
				return this.defaultFolderType;
			}
		}

		internal CorruptDataRecoveryStrategy CorruptDataRecoveryStrategy
		{
			get
			{
				return this.corruptDataRecoveryStrategy;
			}
		}

		private DefaultFolderInfo(DefaultFolderType defaultFolderType, StoreObjectType storeObjectType, LocalizedString localizableDisplayName, EntryIdStrategy entryIdStrategy, DefaultFolderCreator folderCreator, DefaultFolderValidator folderValidationStrategy, DefaultFolderLocalization localizable, DefaultFolderBehavior behavior, CorruptDataRecoveryStrategy recovery)
		{
			this.defaultFolderType = defaultFolderType;
			this.storeObjectType = storeObjectType;
			this.entryIdStrategy = entryIdStrategy;
			this.folderCreator = folderCreator;
			this.folderValidationStrategy = folderValidationStrategy;
			this.localizable = localizable;
			this.localizableDisplayName = localizableDisplayName;
			this.behavior = behavior;
			this.corruptDataRecoveryStrategy = recovery;
		}

		public override string ToString()
		{
			return this.DefaultFolderType.ToString();
		}

		private static DefaultFolderInfo[] CreateDefaultFolderInfos()
		{
			DefaultFolderInfo.defaultFolderInfos = new DefaultFolderInfo[DefaultFolderInfo.DefaultFolderTypeCount];
			DefaultFolderInfo.defaultFolderInfos[0] = new DefaultFolderInfo(DefaultFolderType.None, StoreObjectType.Folder, LocalizedString.Empty, EntryIdStrategy.NoEntryId, DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[34] = new DefaultFolderInfo(DefaultFolderType.Root, StoreObjectType.Folder, ClientStrings.Root, new FreeEntryIdStrategy(new FreeEntryIdStrategy.GetFreeIdDelegate(FreeEntryIdStrategy.GetRootIdDelegate)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[35] = new DefaultFolderInfo(DefaultFolderType.Configuration, StoreObjectType.Folder, LocalizedString.Empty, new FreeEntryIdStrategy(new FreeEntryIdStrategy.GetFreeIdDelegate(FreeEntryIdStrategy.GetConfigurationIdDelegate)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[5] = new DefaultFolderInfo(DefaultFolderType.Inbox, StoreObjectType.Folder, ClientStrings.Inbox, new FreeEntryIdStrategy(new FreeEntryIdStrategy.GetFreeIdDelegate(FreeEntryIdStrategy.GetInboxIdDelegate)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[9] = new DefaultFolderInfo(DefaultFolderType.Outbox, StoreObjectType.Folder, ClientStrings.Outbox, new LocationEntryIdStrategy(MailboxSchema.OutboxEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[10] = new DefaultFolderInfo(DefaultFolderType.SentItems, StoreObjectType.Folder, ClientStrings.SentItems, new LocationEntryIdStrategy(MailboxSchema.SentItemsEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[3] = new DefaultFolderInfo(DefaultFolderType.DeletedItems, StoreObjectType.Folder, ClientStrings.DeletedItems, new LocationEntryIdStrategy(MailboxSchema.DeletedItemsEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[17] = new DefaultFolderInfo(DefaultFolderType.RssSubscription, StoreObjectType.Folder, ClientStrings.RssSubscriptions, new Ex12RenEntryIdStrategy(InternalSchema.AdditionalRenEntryIdsEx, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), Ex12RenEntryIdStrategy.PersistenceId.RssSubscriptions), DefaultFolderCreator.NoCreator, new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchContainerClass("IPF.Note.OutlookHomepage"),
					new MatchMapiFolderType(FolderType.Generic)
				})
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[19] = new DefaultFolderInfo(DefaultFolderType.ToDoSearch, StoreObjectType.SearchFolder, new LocalizedString("To-Do Search"), new Ex12RenEntryIdStrategy(InternalSchema.AdditionalRenEntryIdsEx, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), Ex12RenEntryIdStrategy.PersistenceId.ToDoSearch), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new ToDoSearchValidation(), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)35, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[13] = new DefaultFolderInfo(DefaultFolderType.Conflicts, StoreObjectType.Folder, ClientStrings.Conflicts, new Ex12MultivalueEntryIdStrategy(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), 0), DefaultFolderCreator.NoCreator, DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[55] = new DefaultFolderInfo(DefaultFolderType.DocumentSyncIssues, StoreObjectType.Folder, ClientStrings.DocumentSyncIssues, new Ex12MultivalueEntryIdStrategyForDocumentSyncIssue(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[14] = new DefaultFolderInfo(DefaultFolderType.SyncIssues, StoreObjectType.Folder, ClientStrings.SyncIssues, new Ex12MultivalueEntryIdStrategyForSyncIssue(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[15] = new DefaultFolderInfo(DefaultFolderType.LocalFailures, StoreObjectType.Folder, ClientStrings.LocalFailures, new Ex12MultivalueEntryIdStrategy(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), 2), DefaultFolderCreator.NoCreator, DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[16] = new DefaultFolderInfo(DefaultFolderType.ServerFailures, StoreObjectType.Folder, ClientStrings.ServerFailures, new Ex12MultivalueEntryIdStrategy(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), 3), DefaultFolderCreator.NoCreator, DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[6] = new DefaultFolderInfo(DefaultFolderType.JunkEmail, StoreObjectType.Folder, ClientStrings.JunkEmail, new Ex12MultivalueEntryIdStrategy(InternalSchema.AdditionalRenEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), 4), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)195, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[1] = new DefaultFolderInfo(DefaultFolderType.Calendar, StoreObjectType.CalendarFolder, ClientStrings.Calendar, new LocationEntryIdStrategy(InternalSchema.CalendarFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Appointment")
				})
			}), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)67, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[2] = new DefaultFolderInfo(DefaultFolderType.Contacts, StoreObjectType.ContactsFolder, ClientStrings.Contacts, new LocationEntryIdStrategy(InternalSchema.ContactsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact")
				})
			}), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)67, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[4] = new DefaultFolderInfo(DefaultFolderType.Drafts, StoreObjectType.Folder, ClientStrings.Drafts, new LocationEntryIdStrategy(InternalSchema.DraftsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)67, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[11] = new DefaultFolderInfo(DefaultFolderType.Tasks, StoreObjectType.TasksFolder, ClientStrings.Tasks, new LocationEntryIdStrategy(InternalSchema.TasksFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Task")
				})
			}), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)67, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[7] = new DefaultFolderInfo(DefaultFolderType.Journal, StoreObjectType.JournalFolder, ClientStrings.Journal, new LocationEntryIdStrategy(InternalSchema.JournalFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), DefaultFolderCreator.NoCreator, new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Journal")
				})
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[8] = new DefaultFolderInfo(DefaultFolderType.Notes, StoreObjectType.NotesFolder, ClientStrings.Notes, new LocationEntryIdStrategy(InternalSchema.NotesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), DefaultFolderCreator.NoCreator, new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.StickyNote")
				})
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[21] = new DefaultFolderInfo(DefaultFolderType.CommunicatorHistory, StoreObjectType.Folder, ClientStrings.CommunicatorHistoryFolderName, new LocationEntryIdStrategy(InternalSchema.CommunicatorHistoryFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), DefaultFolderValidator.FolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[52] = new DefaultFolderInfo(DefaultFolderType.LegacyArchiveJournals, StoreObjectType.Folder, ClientStrings.LegacyArchiveJournals, new LocationEntryIdStrategy(InternalSchema.LegacyArchiveJournalsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), DefaultFolderValidator.FolderGenericTypeValidator, DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[20] = new DefaultFolderInfo(DefaultFolderType.ElcRoot, StoreObjectType.Folder, ClientStrings.ElcRoot, new LocationEntryIdStrategy(InternalSchema.ElcRootFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.Provisioned | ELCFolderFlags.Protected | ELCFolderFlags.ELCRoot)
				})
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanHideFolderFromOutlook, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[32] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, new LocalizedString("Recoverable Items"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsRootFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[40] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsDeletions, StoreObjectType.Folder, new LocalizedString("Deletions"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsDeletionsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[33] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsVersions, StoreObjectType.Folder, new LocalizedString("Versions"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsVersionsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[41] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsPurges, StoreObjectType.Folder, new LocalizedString("Purges"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsPurgesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[46] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsDiscoveryHolds, StoreObjectType.Folder, new LocalizedString("DiscoveryHolds"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsDiscoveryHoldsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[59] = new DefaultFolderInfo(DefaultFolderType.RecoverableItemsMigratedMessages, StoreObjectType.Folder, new LocalizedString("MigratedMessages"), new LocationEntryIdStrategy(InternalSchema.RecoverableItemsMigratedMessagesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[57] = new DefaultFolderInfo(DefaultFolderType.CalendarLogging, StoreObjectType.Folder, new LocalizedString("Calendar Logging"), new LocationEntryIdStrategy(InternalSchema.CalendarLoggingFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.LegalHold);
			DefaultFolderInfo.defaultFolderInfos[22] = new DefaultFolderInfo(DefaultFolderType.SyncRoot, StoreObjectType.Folder, new LocalizedString("ExchangeSyncData"), new LocationEntryIdStrategy(InternalSchema.SyncRootFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic)
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[23] = new DefaultFolderInfo(DefaultFolderType.UMVoicemail, StoreObjectType.OutlookSearchFolder, ClientStrings.UMVoiceMailFolderName, new LocationEntryIdStrategy(InternalSchema.UMVoicemailFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.SearchFolders, StoreObjectType.OutlookSearchFolder, true), new UMVoiceMailValidation(), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[30] = new DefaultFolderInfo(DefaultFolderType.UMFax, StoreObjectType.OutlookSearchFolder, ClientStrings.UMFaxFolderName, new LocationEntryIdStrategy(InternalSchema.UMFaxFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.SearchFolders, StoreObjectType.OutlookSearchFolder, true), new UMFaxValidation(), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[12] = new DefaultFolderInfo(DefaultFolderType.Reminders, StoreObjectType.SearchFolder, ClientStrings.RemindersSearchFolderName(string.Empty), new LocationEntryIdStrategy(InternalSchema.RemindersSearchFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new RemindersSearchFolderValidation(), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[24] = new DefaultFolderInfo(DefaultFolderType.AllItems, StoreObjectType.SearchFolder, new LocalizedString("AllItems"), new LocationEntryIdStrategy(InternalSchema.AllItemsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new AllItemsFolderValidation(), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[37] = new DefaultFolderInfo(DefaultFolderType.FreeBusyData, StoreObjectType.Folder, new LocalizedString("Freebusy Data"), new Ex12MultivalueEntryIdNoMoveStampStrategy(InternalSchema.FreeBusyEntryIds, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), 3), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic)
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CreateIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[18] = new DefaultFolderInfo(DefaultFolderType.CommonViews, StoreObjectType.Folder, ClientStrings.CommonViews, new LocationEntryIdStrategy(InternalSchema.CommonViewsEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[36] = new DefaultFolderInfo(DefaultFolderType.SearchFolders, StoreObjectType.Folder, ClientStrings.SearchFolders, new LocationEntryIdStrategy(InternalSchema.FinderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[25] = new DefaultFolderInfo(DefaultFolderType.DeferredActionFolder, StoreObjectType.Folder, new LocalizedString("Deferred Actions"), new LocationEntryIdStrategy(MailboxSchema.DeferredActionFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.Throw);
			DefaultFolderInfo.defaultFolderInfos[26] = new DefaultFolderInfo(DefaultFolderType.LegacySpoolerQueue, StoreObjectType.SearchFolder, new LocalizedString("Spooler Queue"), new FreeEntryIdStrategy(new FreeEntryIdStrategy.GetFreeIdDelegate(FreeEntryIdStrategy.GetSpoolerQueueIdDelegate)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[27] = new DefaultFolderInfo(DefaultFolderType.LegacySchedule, StoreObjectType.Folder, new LocalizedString("Schedule"), new LocationEntryIdStrategy(MailboxSchema.LegacyScheduleFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[29] = new DefaultFolderInfo(DefaultFolderType.LegacyShortcuts, StoreObjectType.Folder, new LocalizedString("Shortcuts"), new LocationEntryIdStrategy(MailboxSchema.LegacyShortcutsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[28] = new DefaultFolderInfo(DefaultFolderType.LegacyViews, StoreObjectType.Folder, new LocalizedString("Views"), new LocationEntryIdStrategy(MailboxSchema.LegacyViewsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag)), DefaultFolderCreator.NoCreator, DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.None, CorruptDataRecoveryStrategy.DoNothing);
			DefaultFolderInfo.defaultFolderInfos[31] = new DefaultFolderInfo(DefaultFolderType.ConversationActions, StoreObjectType.Folder, new LocalizedString("Conversation Action Settings"), new Ex12RenEntryIdStrategy(InternalSchema.AdditionalRenEntryIdsEx, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), Ex12RenEntryIdStrategy.PersistenceId.ConversationActions), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new ConversationActionsValidator(), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)195, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[38] = new DefaultFolderInfo(DefaultFolderType.Sharing, StoreObjectType.Folder, new LocalizedString("Sharing"), new LocationEntryIdStrategy(InternalSchema.SharingFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.MessageFolderGenericTypeValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CreateIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[45] = new DefaultFolderInfo(DefaultFolderType.Location, StoreObjectType.Folder, new LocalizedString("Location"), new LocationEntryIdStrategy(InternalSchema.LocationFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.FolderGenericTypeValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CreateIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[39] = new DefaultFolderInfo(DefaultFolderType.System, StoreObjectType.Folder, new LocalizedString("System"), new LocationEntryIdStrategy(InternalSchema.SystemFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.Configuration, new IValidator[]
			{
				new MatchIsSystemFolder()
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CreateIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[42] = new DefaultFolderInfo(DefaultFolderType.CalendarVersionStore, StoreObjectType.SearchFolder, new LocalizedString("Calendar Version Store"), new LocationEntryIdStrategy(InternalSchema.CalendarVersionStoreFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new CalendarVersionStoreValidation(), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[43] = new DefaultFolderInfo(DefaultFolderType.AdminAuditLogs, StoreObjectType.Folder, new LocalizedString("AdminAuditLogs"), new LocationEntryIdStrategy(InternalSchema.AdminAuditLogsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[44] = new DefaultFolderInfo(DefaultFolderType.Audits, StoreObjectType.Folder, new LocalizedString("Audits"), new LocationEntryIdStrategy(InternalSchema.AuditsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.RecoverableItemsRoot, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.RecoverableItemsRoot, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchAdminFolderFlags(ELCFolderFlags.DumpsterFolder)
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.AlwaysDeferInitialization, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[47] = new DefaultFolderInfo(DefaultFolderType.AllContacts, StoreObjectType.SearchFolder, new LocalizedString("AllContacts"), new LocationEntryIdStrategy(InternalSchema.AllContactsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new AllContactsFolderValidation(), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[54] = new DefaultFolderInfo(DefaultFolderType.MyContacts, StoreObjectType.SearchFolder, ClientStrings.MyContactsFolderName, new LocationEntryIdStrategy(InternalSchema.MyContactsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new MyContactsFolderValidation(ContactsSearchFolderCriteria.MyContacts), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[58] = new DefaultFolderInfo(DefaultFolderType.MyContactsExtended, StoreObjectType.SearchFolder, new LocalizedString("MyContactsExtended"), new LocationEntryIdStrategy(InternalSchema.MyContactsExtendedFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new MyContactsFolderValidation(ContactsSearchFolderCriteria.MyContactsExtended), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[48] = new DefaultFolderInfo(DefaultFolderType.RecipientCache, StoreObjectType.ContactsFolder, new LocalizedString("Recipient Cache"), new LocationEntryIdStrategy(InternalSchema.RecipientCacheFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Contacts, StoreObjectType.ContactsFolder, true), new DefaultSubFolderValidator(DefaultFolderType.Contacts, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsHidden(true),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact.RecipientCache")
				})
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[49] = new DefaultFolderInfo(DefaultFolderType.PeopleConnect, StoreObjectType.Folder, new LocalizedString("PeopleConnect"), new LocationEntryIdStrategy(InternalSchema.PeopleConnectFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.FolderGenericTypeValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CreateIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[50] = new DefaultFolderInfo(DefaultFolderType.QuickContacts, StoreObjectType.ContactsFolder, new LocalizedString("{06967759-274D-40B2-A3EB-D7F9E73727D7}"), new Ex12RenEntryIdStrategy(InternalSchema.AdditionalRenEntryIdsEx, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), Ex12RenEntryIdStrategy.PersistenceId.QuickContacts), new QuickContactsDefaultFolderCreator(), new DefaultSubFolderValidator(DefaultFolderType.Contacts, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsClientReadOnly(),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact.MOC.QuickContacts")
				})
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)163, CorruptDataRecoveryStrategy.Repair);
			DefaultFolderInfo.defaultFolderInfos[51] = new DefaultFolderInfo(DefaultFolderType.ImContactList, StoreObjectType.ContactsFolder, new LocalizedString("{A9E2BC46-B3A0-4243-B315-60D991004455}"), new Ex12RenEntryIdStrategy(InternalSchema.AdditionalRenEntryIdsEx, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag), Ex12RenEntryIdStrategy.PersistenceId.ImContactList), new MessageClassBasedDefaultFolderCreator(DefaultFolderType.Contacts, "IPF.Contact.MOC.ImContactList", true), new DefaultSubFolderValidator(DefaultFolderType.Contacts, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsHidden(true),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact.MOC.ImContactList")
				})
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)167, CorruptDataRecoveryStrategy.Repair);
			DefaultFolderInfo.defaultFolderInfos[53] = new DefaultFolderInfo(DefaultFolderType.OrganizationalContacts, StoreObjectType.ContactsFolder, new LocalizedString("Organizational Contacts"), new LocationEntryIdStrategy(InternalSchema.OrganizationalContactsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Contacts, StoreObjectType.ContactsFolder, true), new DefaultSubFolderValidator(DefaultFolderType.Contacts, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsHidden(true),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact.OrganizationalContacts")
				})
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)167, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[62] = new DefaultFolderInfo(DefaultFolderType.MailboxAssociation, StoreObjectType.Folder, new LocalizedString("MailboxAssociations"), new LocationEntryIdStrategy(InternalSchema.MailboxAssociationFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[56] = new DefaultFolderInfo(DefaultFolderType.PushNotificationRoot, StoreObjectType.Folder, new LocalizedString("Notification Subscriptions"), new LocationEntryIdStrategy(InternalSchema.PushNotificationFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic)
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[60] = new DefaultFolderInfo(DefaultFolderType.GroupNotifications, StoreObjectType.Folder, new LocalizedString("GroupNotifications"), new LocationEntryIdStrategy(InternalSchema.GroupNotificationsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.FolderGenericTypeValidator, DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[61] = new DefaultFolderInfo(DefaultFolderType.Favorites, StoreObjectType.SearchFolder, ClientStrings.FavoritesFolderName, new LocationEntryIdStrategy(InternalSchema.FavoritesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new FavoritesFolderValidation(), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[63] = new DefaultFolderInfo(DefaultFolderType.FromFavoriteSenders, StoreObjectType.SearchFolder, ClientStrings.FromFavoriteSendersFolderName, new LocationEntryIdStrategy(InternalSchema.FromFavoriteSendersFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new FromFavoriteSendersFolderValidation(), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[73] = new DefaultFolderInfo(DefaultFolderType.FromPeople, StoreObjectType.SearchFolder, new LocalizedString("From People"), new LocationEntryIdStrategy(InternalSchema.FromPeopleFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new FromPeopleFolderValidation(), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[64] = new DefaultFolderInfo(DefaultFolderType.OutlookService, StoreObjectType.Folder, new LocalizedString("OutlookService"), new LocationEntryIdStrategy(InternalSchema.OutlookServiceFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic)
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[65] = new DefaultFolderInfo(DefaultFolderType.GalContacts, StoreObjectType.ContactsFolder, new LocalizedString("GAL Contacts"), new LocationEntryIdStrategy(InternalSchema.GalContactsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Contacts, StoreObjectType.ContactsFolder, true), new DefaultSubFolderValidator(DefaultFolderType.Contacts, new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsHidden(true),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Contact.GalContacts")
				})
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[66] = new DefaultFolderInfo(DefaultFolderType.UserActivity, StoreObjectType.Folder, new LocalizedString("UserActivity"), new LocationEntryIdStrategy(InternalSchema.UserActivityFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic)
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.CanHideFolderFromOutlook | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[67] = new DefaultFolderInfo(DefaultFolderType.WorkingSet, StoreObjectType.Folder, new LocalizedString("Working Set"), new LocationEntryIdStrategy(InternalSchema.WorkingSetFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new MatchIsHidden(true)
			}), DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[68] = new DefaultFolderInfo(DefaultFolderType.Clutter, StoreObjectType.Folder, ClientStrings.ClutterFolderName, new LocationEntryIdStrategy(InternalSchema.ClutterFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultSubFolderValidator(DefaultFolderType.Root, new IValidator[]
			{
				new MatchMapiFolderType(FolderType.Generic),
				new MatchContainerClassOrThrow("IPF.Note")
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[72] = new DefaultFolderInfo(DefaultFolderType.BirthdayCalendar, StoreObjectType.CalendarFolder, ClientStrings.BirthdayCalendarFolderName, new LocationEntryIdStrategy(InternalSchema.BirthdayCalendarFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new BirthdayCalendarFolderCreator(), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.Appointment.Birthday")
				})
			}), DefaultFolderLocalization.CanLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanHideFolderFromOutlook | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[69] = new DefaultFolderInfo(DefaultFolderType.ParkedMessages, StoreObjectType.Folder, new LocalizedString("ParkedMessages"), new LocationEntryIdStrategy(InternalSchema.ParkedMessagesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[70] = new DefaultFolderInfo(DefaultFolderType.UnifiedInbox, StoreObjectType.SearchFolder, ClientStrings.UnifiedInbox, new LocationEntryIdStrategy(InternalSchema.UnifiedInboxFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.SearchFolder, true), new UnifiedInboxFolderValidation(), DefaultFolderLocalization.CanLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[71] = new DefaultFolderInfo(DefaultFolderType.TemporarySaves, StoreObjectType.Folder, new LocalizedString("TemporarySaves"), new LocationEntryIdStrategy(InternalSchema.TemporarySavesFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)135, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[74] = new DefaultFolderInfo(DefaultFolderType.SnackyApps, StoreObjectType.Folder, new LocalizedString("SnackyApps"), new LocationEntryIdStrategy(InternalSchema.SnackyAppsFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Configuration, StoreObjectType.Folder, true), DefaultFolderValidator.NullValidator, DefaultFolderLocalization.CanNotLocalize, (DefaultFolderBehavior)39, CorruptDataRecoveryStrategy.Recreate);
			DefaultFolderInfo.defaultFolderInfos[75] = new DefaultFolderInfo(DefaultFolderType.SmsAndChatsSync, StoreObjectType.Folder, new LocalizedString("SmsAndChatsSync"), new LocationEntryIdStrategy(InternalSchema.SmsAndChatsSyncFolderEntryId, new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag)), new DefaultFolderCreator(DefaultFolderType.Root, StoreObjectType.Folder, true), new DefaultFolderValidator(new IValidator[]
			{
				new CompositeValidator(new IValidator[]
				{
					new MatchIsHidden(true),
					new MatchMapiFolderType(FolderType.Generic),
					new MatchContainerClass("IPF.SmsAndChatsSync")
				})
			}), DefaultFolderLocalization.CanNotLocalize, DefaultFolderBehavior.CanCreate | DefaultFolderBehavior.CanNotRename | DefaultFolderBehavior.CanHideFolderFromOutlook | DefaultFolderBehavior.RefreshIfMissing, CorruptDataRecoveryStrategy.Recreate);
			return DefaultFolderInfo.defaultFolderInfos;
		}

		private static StorePropertyDefinition[] GetDependentProperties(LocationEntryIdStrategy.GetLocationPropertyBagDelegate location)
		{
			List<StorePropertyDefinition> list = new List<StorePropertyDefinition>(DefaultFolderInfo.Instance.Length + 1);
			foreach (DefaultFolderInfo defaultFolderInfo in DefaultFolderInfo.Instance)
			{
				defaultFolderInfo.EntryIdStrategy.GetDependentProperties(location, list);
			}
			if (location == new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag))
			{
				list.Add(InternalSchema.SystemFolderFlags);
			}
			return list.ToArray();
		}

		public const string RecipientCacheFolderName = "Recipient Cache";

		public const string GALContactsFolderName = "GAL Contacts";

		public const string QuickContactsFolderName = "{06967759-274D-40B2-A3EB-D7F9E73727D7}";

		public const string ImContactListFolderName = "{A9E2BC46-B3A0-4243-B315-60D991004455}";

		private const string ToDoSearchFolderName = "To-Do Search";

		private const string SyncRootFolderName = "ExchangeSyncData";

		private const string FreeBusyFolderName = "Freebusy Data";

		private const string ExternalIdentitiesFolderName = "External Identities";

		private const string SharingFolderName = "Sharing";

		private const string ConversationActionsFolderName = "Conversation Action Settings";

		internal const string RecoverableItemsRootFolderName = "Recoverable Items";

		internal const string RecoverableItemsDeletionsFolderName = "Deletions";

		internal const string RecoverableItemsVersionsFolderName = "Versions";

		internal const string RecoverableItemsPurgesFolderName = "Purges";

		internal const string RecoverableItemsDiscoveryHoldsFolderName = "DiscoveryHolds";

		internal const string RecoverableItemsMigratedMessagesFolderName = "MigratedMessages";

		private const string SystemFolderName = "System";

		internal const string AdminAuditLogsFolderName = "AdminAuditLogs";

		private const string CalendarVersionStoreFolderName = "Calendar Version Store";

		internal const string AuditsFolderName = "Audits";

		internal const string AllContactsFolderName = "AllContacts";

		internal const string MyContactsExtendedFolderName = "MyContactsExtended";

		private const string NotificationRootFolderName = "Notification Subscriptions";

		private const string CalendarLoggingFolderName = "Calendar Logging";

		private const string MailboxAssociationFolderName = "MailboxAssociations";

		private const string DeferredActionsFolderName = "Deferred Actions";

		private const string LegacySpoolerQueueFolderName = "Spooler Queue";

		private const string LegacyScheduleFolderName = "Schedule";

		private const string LegacyShortcutsFolderName = "Shortcuts";

		private const string LegacyViewsFolderName = "Views";

		private const string LocationFolderName = "Location";

		private const string PeopleConnectFolderName = "PeopleConnect";

		private const string OrganizationalContactsFolderName = "Organizational Contacts";

		private const string GroupNotificationsFolderName = "GroupNotifications";

		private const string FromPeopleFolderName = "From People";

		private const string OutlookServiceFolderName = "OutlookService";

		private const string UserActivityFolderName = "UserActivity";

		private const string WorkingSetFolderName = "Working Set";

		private const string ParkedMessagesFolderName = "ParkedMessages";

		private const string TemporarySavesFolderName = "TemporarySaves";

		private const string SnackyAppsFolderName = "SnackyApps";

		private const string SmsAndChatsSyncFolderName = "SmsAndChatsSync";

		private static readonly int defaultFolderTypeCount = Enum.GetValues(typeof(DefaultFolderType)).Length;

		private static DefaultFolderInfo[] defaultFolderInfos = DefaultFolderInfo.CreateDefaultFolderInfos();

		private static readonly StorePropertyDefinition[] mailboxProperties = DefaultFolderInfo.GetDependentProperties(new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetMailboxPropertyBag));

		private static readonly StorePropertyDefinition[] inboxOrConfigurationFolderProperties = DefaultFolderInfo.GetDependentProperties(new LocationEntryIdStrategy.GetLocationPropertyBagDelegate(LocationEntryIdStrategy.GetInboxOrConfigurationFolderPropertyBag));

		private readonly DefaultFolderValidator folderValidationStrategy;

		private readonly DefaultFolderLocalization localizable;

		private readonly LocalizedString localizableDisplayName;

		private readonly DefaultFolderType defaultFolderType;

		private readonly EntryIdStrategy entryIdStrategy;

		private readonly StoreObjectType storeObjectType;

		private readonly DefaultFolderCreator folderCreator;

		private readonly DefaultFolderBehavior behavior;

		private readonly CorruptDataRecoveryStrategy corruptDataRecoveryStrategy;
	}
}
