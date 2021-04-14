using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class FolderHierarchyUtils
	{
		public static List<WellKnownFolder> DiscoverWellKnownFolders(IMailbox mailbox, int flags)
		{
			Dictionary<WellKnownFolderType, byte[]> dictionary = new Dictionary<WellKnownFolderType, byte[]>();
			bool flag = ((FolderHierarchyFlags)flags).HasFlag(FolderHierarchyFlags.PublicFolderMailbox);
			FolderHierarchyUtils.FindWellKnownFolders(mailbox, flag ? FolderHierarchyUtils.PublicFolderMailboxRefs : FolderHierarchyUtils.MailboxRefs, (PropTag[] pta) => mailbox.GetProps(pta), dictionary);
			if (!flag)
			{
				FolderHierarchyUtils.FindWellKnownFoldersWithinInbox(mailbox, dictionary);
			}
			List<WellKnownFolder> list = new List<WellKnownFolder>();
			foreach (KeyValuePair<WellKnownFolderType, byte[]> keyValuePair in dictionary)
			{
				list.Add(new WellKnownFolder((int)keyValuePair.Key, keyValuePair.Value));
			}
			return list;
		}

		internal static WellKnownFolderMapping FindWKFMapping(WellKnownFolderType wkfType, int flags)
		{
			if (wkfType == WellKnownFolderType.None)
			{
				return null;
			}
			MailboxRootFolderMapping[] array = ((FolderHierarchyFlags)flags).HasFlag(FolderHierarchyFlags.PublicFolderMailbox) ? FolderHierarchyUtils.PublicFolderMailboxRefs : FolderHierarchyUtils.MailboxRefs;
			foreach (MailboxRootFolderMapping mailboxRootFolderMapping in array)
			{
				if (mailboxRootFolderMapping.WKFType == wkfType)
				{
					return mailboxRootFolderMapping;
				}
			}
			foreach (InboxFolderMapping inboxFolderMapping in FolderHierarchyUtils.InboxRefs)
			{
				if (inboxFolderMapping.WKFType == wkfType)
				{
					return inboxFolderMapping;
				}
			}
			foreach (NamedFolderMapping namedFolderMapping in FolderHierarchyUtils.NamedFolderRefs)
			{
				if (namedFolderMapping.WKFType == wkfType)
				{
					return namedFolderMapping;
				}
			}
			return null;
		}

		internal static List<PropTag> GetInboxReferencePtags(NamedPropMapper namedPropMapper, int? mailboxVersion)
		{
			List<PropTag> list = new List<PropTag>();
			foreach (InboxFolderMapping inboxFolderMapping in FolderHierarchyUtils.InboxRefs)
			{
				InboxNamedPropFolderMapping inboxNamedPropFolderMapping = inboxFolderMapping as InboxNamedPropFolderMapping;
				if (inboxNamedPropFolderMapping != null)
				{
					list.Add(namedPropMapper.MapNamedProp(inboxNamedPropFolderMapping.NamedPropData, PropType.Binary));
				}
				else
				{
					list.Add(inboxFolderMapping.Ptag);
				}
			}
			if (mailboxVersion == null || mailboxVersion.Value < Server.E14SP1MinVersion)
			{
				list.Add((PropTag)907804930U);
			}
			list.Add((PropTag)920191234U);
			return list;
		}

		private static void FindWellKnownFolders(IMailbox mailbox, PropTagFolderMapping[] map, FolderHierarchyUtils.GetPropsDelegate getProps, Dictionary<WellKnownFolderType, byte[]> wellKnownFolders)
		{
			NamedPropMapper namedPropMapper = CommonUtils.CreateNamedPropMapper(mailbox);
			PropTag[] array = new PropTag[map.Length];
			for (int i = 0; i < map.Length; i++)
			{
				InboxNamedPropFolderMapping inboxNamedPropFolderMapping = map[i] as InboxNamedPropFolderMapping;
				if (inboxNamedPropFolderMapping != null)
				{
					namedPropMapper.ByNamedProp.AddKey(inboxNamedPropFolderMapping.NamedPropData);
				}
			}
			for (int j = 0; j < map.Length; j++)
			{
				InboxNamedPropFolderMapping inboxNamedPropFolderMapping2 = map[j] as InboxNamedPropFolderMapping;
				if (inboxNamedPropFolderMapping2 != null)
				{
					array[j] = namedPropMapper.MapNamedProp(inboxNamedPropFolderMapping2.NamedPropData, PropType.Binary);
				}
				else
				{
					array[j] = map[j].Ptag;
				}
			}
			PropValueData[] a = getProps(array);
			PropValue[] native = DataConverter<PropValueConverter, PropValue, PropValueData>.GetNative(a);
			for (int k = 0; k < map.Length; k++)
			{
				byte[] array2 = null;
				if (!native[k].IsNull() && !native[k].IsError())
				{
					InboxIndexedFolderMapping inboxIndexedFolderMapping = map[k] as InboxIndexedFolderMapping;
					if (inboxIndexedFolderMapping == null)
					{
						array2 = native[k].GetBytes();
					}
					else
					{
						byte[][] bytesArray = native[k].GetBytesArray();
						if (bytesArray != null && bytesArray.Length > inboxIndexedFolderMapping.EntryIndex)
						{
							array2 = bytesArray[inboxIndexedFolderMapping.EntryIndex];
						}
						else
						{
							array2 = null;
						}
					}
				}
				if (array2 != null)
				{
					wellKnownFolders[map[k].WKFType] = array2;
				}
			}
		}

		private static void FindWellKnownFoldersWithinInbox(IMailbox mailbox, Dictionary<WellKnownFolderType, byte[]> wellKnownFolders)
		{
			byte[] receiveFolderEntryId = mailbox.GetReceiveFolderEntryId("IPM");
			if (receiveFolderEntryId != null)
			{
				using (IFolder inbox = CommonUtils.GetFolder(mailbox, receiveFolderEntryId))
				{
					if (inbox != null)
					{
						FolderHierarchyUtils.FindWellKnownFolders(mailbox, FolderHierarchyUtils.InboxRefs, (PropTag[] pta) => inbox.GetProps(pta), wellKnownFolders);
					}
				}
			}
		}

		internal const string SourceEntryID = "SourceEntryID";

		internal const string SourceWellKnownFolderType = "SourceWellKnownFolderType";

		internal const string SourceLastModifiedTimestamp = "SourceLastModifiedTimestamp";

		internal const string SourceMessageClass = "SourceMessageClass";

		internal const string SourceSyncAccountName = "SourceSyncAccountName";

		internal const string SourceSyncMessageId = "SourceSyncMessageId";

		internal const string SourceSyncFolderId = "SourceSyncFolderId";

		internal const int SharingInstanceGuidPropertyId = 35356;

		internal static readonly MailboxRootFolderMapping[] MailboxRefs = new MailboxRootFolderMapping[]
		{
			new MailboxRootFolderMapping(WellKnownFolderType.IpmSubtree, PropTag.IpmSubtreeEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.NonIpmSubtree, PropTag.NonIpmSubtreeEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.EFormsRegistry, PropTag.EFormsRegistryEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.FreeBusy, PropTag.FreeBusyEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.OfflineAddressBook, PropTag.OfflineAddressBookEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.LocalSiteFreeBusy, PropTag.LocalSiteFreeBusyEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.LocalSiteOfflineAddressBook, PropTag.LocalSiteOfflineAddressBookEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.SentItems, PropTag.IpmSentMailEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.Outbox, PropTag.IpmOutboxEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.DeletedItems, PropTag.IpmWasteBasketEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.CommonViews, PropTag.CommonViewsEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.Finder, PropTag.FinderEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.DeferredActions, PropTag.DeferredActionFolderEntryID),
			new MailboxRootFolderMapping(WellKnownFolderType.LegacySchedule, ExtraPropTag.ScheduleFolderEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.LegacyShortcuts, ExtraPropTag.ShortcutsFolderEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.LegacyViews, PropTag.ViewsEntryId)
		};

		internal static readonly MailboxRootFolderMapping[] PublicFolderMailboxRefs = new MailboxRootFolderMapping[]
		{
			new MailboxRootFolderMapping(WellKnownFolderType.IpmSubtree, PropTag.DeferredActionFolderEntryID),
			new MailboxRootFolderMapping(WellKnownFolderType.NonIpmSubtree, PropTag.SpoolerQueueEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.EFormsRegistry, PropTag.IpmSubtreeEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.PublicFolderDumpsterRoot, PropTag.IpmInboxEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.PublicFolderTombstonesRoot, PropTag.IpmOutboxEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.PublicFolderAsyncDeleteState, PropTag.IpmSentMailEntryId),
			new MailboxRootFolderMapping(WellKnownFolderType.PublicFolderInternalSubmission, PropTag.IpmWasteBasketEntryId)
		};

		internal static readonly InboxFolderMapping[] InboxRefs = new InboxFolderMapping[]
		{
			new InboxFolderMapping(WellKnownFolderType.Inbox, PropTag.EntryId),
			new InboxFolderMapping(WellKnownFolderType.Calendar, PropTag.CalendarFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Contacts, PropTag.ContactsFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Drafts, PropTag.DraftsFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Journal, PropTag.JournalFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Tasks, PropTag.TasksFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Notes, PropTag.NotesFolderEntryId),
			new InboxNamedPropFolderMapping(WellKnownFolderType.CalendarLogging, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "CalendarLoggingEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.Dumpster, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "DumpsterEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.DumpsterDeletions, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsDeletionsEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.DumpsterVersions, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsVersionsEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.DumpsterPurges, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "RecoverableItemsPurgesEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.DumpsterAdminAuditLogs, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "AdminAuditLogsFolderEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.DumpsterAudits, new NamedPropData(WellKnownNamedPropertyGuid.Elc, "AuditsFolderEntryId")),
			new InboxFolderMapping(WellKnownFolderType.CommunicatorHistory, ExtraPropTag.CommunicatorHistoryFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.ELC, ExtraPropTag.ElcRootFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.SyncRoot, ExtraPropTag.SyncRootFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.UMVoicemail, ExtraPropTag.UMVoicemailFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.UMFax, ExtraPropTag.UMFaxFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Reminders, ExtraPropTag.RemOnlineEntryId),
			new InboxFolderMapping(WellKnownFolderType.AllItems, ExtraPropTag.AllItemsFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.Sharing, PropTag.SharingFolderEntryId),
			new InboxFolderMapping(WellKnownFolderType.System, ExtraPropTag.AdminDataFolderEntryId),
			new InboxIndexedFolderMapping(WellKnownFolderType.Conflicts, ExtraPropTag.AdditionalRenEntryIds, 0),
			new InboxIndexedFolderMapping(WellKnownFolderType.SyncIssues, ExtraPropTag.AdditionalRenEntryIds, 1),
			new InboxIndexedFolderMapping(WellKnownFolderType.LocalFailures, ExtraPropTag.AdditionalRenEntryIds, 2),
			new InboxIndexedFolderMapping(WellKnownFolderType.ServerFailures, ExtraPropTag.AdditionalRenEntryIds, 3),
			new InboxIndexedFolderMapping(WellKnownFolderType.JunkEmail, ExtraPropTag.AdditionalRenEntryIds, 4),
			new InboxIndexedFolderMapping(WellKnownFolderType.FreeBusyData, ExtraPropTag.FreeBusyEntryIds, 3),
			new InboxNamedPropFolderMapping(WellKnownFolderType.PeopleConnect, new NamedPropData(WellKnownNamedPropertyGuid.Address, "PeopleConnectFolderEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.Location, new NamedPropData(WellKnownNamedPropertyGuid.Location, "LocationFolderEntryId")),
			new InboxNamedPropFolderMapping(WellKnownFolderType.MailboxAssociations, new NamedPropData(WellKnownNamedPropertyGuid.Common, "MailboxAssociationFolderEntryId"))
		};

		internal static readonly NamedFolderMapping[] NamedFolderRefs = new NamedFolderMapping[]
		{
			new NamedFolderMapping(WellKnownFolderType.TransportQueue, WellKnownFolderType.Root, "Transport Queue"),
			new NamedFolderMapping(WellKnownFolderType.SpoolerQueue, WellKnownFolderType.Root, "Spooler Queue"),
			new NamedFolderMapping(WellKnownFolderType.ConversationActionSettings, WellKnownFolderType.IpmSubtree, "Conversation Action Settings"),
			new NamedFolderMapping(WellKnownFolderType.MRSSyncStates, WellKnownFolderType.NonIpmSubtree, "MailboxReplicationService SyncStates"),
			new NamedFolderMapping(WellKnownFolderType.MRSMoveHistory, WellKnownFolderType.Root, MoveHistoryEntryInternal.MHEFolderName),
			new NamedFolderMapping(WellKnownFolderType.EventsRoot, WellKnownFolderType.NonIpmSubtree, "Events Root"),
			new NamedFolderMapping(WellKnownFolderType.SchemaRoot, WellKnownFolderType.NonIpmSubtree, "Schema-Root"),
			new NamedFolderMapping(WellKnownFolderType.WorkingSet, WellKnownFolderType.IpmSubtree, "Working Set"),
			new NamedFolderMapping(WellKnownFolderType.ParkedMessages, WellKnownFolderType.Root, "ParkedMessages"),
			new NamedFolderMapping(WellKnownFolderType.TemporarySaves, WellKnownFolderType.Root, "TemporarySaves")
		};

		internal static readonly Guid MRSPropsGuid = new Guid("9137a2fd-2fa5-4409-91aa-2c3ee697350a");

		internal static readonly NamedPropData SourceEntryIDNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceEntryID");

		internal static readonly NamedPropData SourceWKFTypeNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceWellKnownFolderType");

		internal static readonly NamedPropData SourceLastModifiedTimestampNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceLastModifiedTimestamp");

		internal static readonly NamedPropData SourceMessageClassNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceMessageClass");

		internal static readonly NamedPropData SourceSyncAccountNameNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceSyncAccountName");

		internal static readonly NamedPropData SourceSyncMessageIdNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceSyncMessageId");

		internal static readonly NamedPropData SourceSyncFolderIdNamedProp = new NamedPropData(FolderHierarchyUtils.MRSPropsGuid, "SourceSyncFolderId");

		internal static readonly NamedPropData CloudIdNamedProp = new NamedPropData(WellKnownNamedPropertyGuid.Messaging, ItemSchema.CloudId.Name);

		internal static readonly NamedPropData SharingInstanceGuidNamedProp = new NamedPropData(WellKnownNamedPropertyGuid.Sharing, 35356);

		private delegate PropValueData[] GetPropsDelegate(PropTag[] pta);
	}
}
