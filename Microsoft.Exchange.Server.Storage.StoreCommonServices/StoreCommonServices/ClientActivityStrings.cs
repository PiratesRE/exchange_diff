using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class ClientActivityStrings
	{
		public static uint Admin
		{
			get
			{
				return ClientActivityStrings.adminId;
			}
		}

		public static uint Task
		{
			get
			{
				return ClientActivityStrings.taskId;
			}
		}

		internal static void Initialize()
		{
			ClientActivityStrings.knownStringIds = new LockFreeDictionary<uint, string>(from s in ClientActivityStrings.knownStrings
			select new KeyValuePair<uint, string>(ClientActivityStrings.CalculateStringId(s), s));
		}

		internal static void WriteReferenceData()
		{
			IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.ReferenceData);
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return;
			}
			foreach (string text in ClientActivityStrings.knownStrings)
			{
				using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.ActivityInfo, true, false, (int)ClientActivityStrings.CalculateStringId(text), text))
				{
					logger.TryWrite(traceBuffer);
				}
			}
		}

		internal static uint GetStringId(string str)
		{
			uint num = ClientActivityStrings.CalculateStringId(str);
			if (ClientActivityStrings.knownStringIds.TryAdd(num, str))
			{
				IBinaryLogger logger = LoggerManager.GetLogger(LoggerType.ReferenceData);
				if (logger != null && logger.IsLoggingEnabled)
				{
					using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.ActivityInfo, true, false, (int)num, str))
					{
						if (!logger.TryWrite(traceBuffer))
						{
							ClientActivityStrings.knownStringIds.Remove(num);
						}
					}
				}
			}
			return num;
		}

		internal static string GetString(uint id)
		{
			if (ClientActivityStrings.knownStringIds != null && ClientActivityStrings.knownStringIds.ContainsKey(id))
			{
				return ClientActivityStrings.knownStringIds[id];
			}
			return string.Empty;
		}

		private static uint CalculateStringId(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return 0U;
			}
			return (uint)str.GetHashCode();
		}

		private const string AdminActivity = "Admin";

		private const string TaskActivity = "Task";

		private static LockFreeDictionary<uint, string> knownStringIds;

		private static uint taskId = ClientActivityStrings.CalculateStringId("Task");

		private static uint adminId = ClientActivityStrings.CalculateStringId("Admin");

		private static string[] knownStrings = new string[]
		{
			".",
			".CopyItem",
			".DeleteItem",
			".FolderSync",
			".GetItem",
			".ItemOperations",
			".MailboxReplicationProxyService",
			".MoveItem",
			".MoveItems",
			".Ping",
			".Settings",
			".Sync",
			".UpdateItem",
			"ActiveSync.FolderCreate",
			"ActiveSync.FolderDelete",
			"ActiveSync.FolderSync",
			"ActiveSync.FolderUpdate",
			"ActiveSync.GetItemEstimate",
			"ActiveSync.Invalid",
			"ActiveSync.ItemOperations",
			"ActiveSync.MeetingResponse",
			"ActiveSync.MoveItems",
			"ActiveSync.Ping",
			"ActiveSync.Provision",
			"ActiveSync.ResolveRecipients",
			"ActiveSync.Search",
			"ActiveSync.SendMail",
			"ActiveSync.Settings",
			"ActiveSync.SmartForward",
			"ActiveSync.SmartReply",
			"ActiveSync.Sync",
			"ActiveSync.ValidateCert",
			"ApprovalAssistant.",
			"CalendarAssistant.",
			"CalendarNotificationAssistant.",
			"CalendarRepairAssistant.",
			"CalendarSyncAssistant.",
			"ConversationsAssistant.",
			"DirectoryProcessorAssistant.",
			"ELCAssistant.",
			"ElcEventBasedAssistant.",
			"Ews.AddDelegate",
			"Ews.AddDistributionGroupToImList",
			"Ews.AddImContactToGroup",
			"Ews.AddImGroup",
			"Ews.AddNewImContactToGroup",
			"Ews.CopyItem",
			"Ews.CreateAttachment",
			"Ews.CreateFolder",
			"Ews.CreateItem",
			"Ews.CreateUMCallDataRecord",
			"Ews.CreateUserConfiguration",
			"Ews.DeleteAttachment",
			"Ews.DeleteFolder",
			"Ews.DeleteItem",
			"Ews.ExpandDL",
			"Ews.ExportItems",
			"Ews.FindConversation",
			"Ews.FindFolder",
			"Ews.FindItem",
			"Ews.FindPeople",
			"Ews.GetAppManifests",
			"Ews.GetAppMarketplaceUrl",
			"Ews.GetAttachment",
			"Ews.GetClientAccessToken",
			"Ews.GetClientExtension",
			"Ews.GetClientIntent",
			"Ews.GetDelegate",
			"Ews.GetEncryptionConfiguration",
			"Ews.GetFolder",
			"Ews.GetImItemList",
			"Ews.GetImItems",
			"Ews.GetInboxRules",
			"Ews.GetItem",
			"Ews.GetMailTips",
			"Ews.GetServiceConfiguration",
			"Ews.GetSharingFolder",
			"Ews.GetSharingMetadata",
			"Ews.GetUserAvailability",
			"Ews.GetUserConfiguration",
			"Ews.GetUserOofSettings",
			"Ews.GetUserPhoto",
			"Ews.InitUMMailbox",
			"Ews.MarkAsJunk",
			"Ews.MoveFolder",
			"Ews.MoveItem",
			"Ews.RefreshSharingFolder",
			"Ews.RemoveContactFromImList",
			"Ews.RemoveDelegate",
			"Ews.RemoveImContactFromGroup",
			"Ews.ResolveNames",
			"Ews.SaveUMPin",
			"Ews.SearchMailboxes",
			"Ews.SendItem",
			"Ews.SetImGroup",
			"Ews.SetUserOofSettings",
			"Ews.Subscribe",
			"Ews.SyncFolderHierarchy",
			"Ews.SyncFolderItems",
			"Ews.UpdateFolder",
			"Ews.UpdateInboxRules",
			"Ews.UpdateItem",
			"Ews.UpdateItemInRecoverableItems",
			"Ews.UpdateUserConfiguration",
			"Ews.UploadItems",
			"Ews.ValidateUMPin",
			"IMAP4.append",
			"IMAP4.authenticate",
			"IMAP4.capability",
			"IMAP4.check",
			"IMAP4.close",
			"IMAP4.copy",
			"IMAP4.create",
			"IMAP4.delete",
			"IMAP4.examine",
			"IMAP4.expunge",
			"IMAP4.fetch",
			"IMAP4.idle",
			"IMAP4.list",
			"IMAP4.lsub",
			"IMAP4.noop",
			"IMAP4.rename",
			"IMAP4.search",
			"IMAP4.select",
			"IMAP4.status",
			"IMAP4.store",
			"IMAP4.subscribe",
			"IMAP4.uid",
			"IMAP4.uid+copy",
			"IMAP4.uid+fetch",
			"IMAP4.uid+search",
			"IMAP4.uid+store",
			"IMAP4.uid+expunge",
			"IMAP4.unsubscribe",
			"InferenceDataCollectionAssistant.",
			"InferenceTrainingAssistant.",
			"JunkEmailOptionsAssistant.",
			"JunkEmailOptionsCommitterAssistant.",
			"MailboxReplicationService.IncrementalMergeJob",
			"MailboxReplicationService.LocalMoveJob",
			"MailboxReplicationService.RemoteMoveJob",
			"MailboxTransportSubmissionAssist.",
			"MailboxTransportSubmissionAssistant.",
			"MailboxTransportSubmissionAssistant.Approval Submit Agent",
			"MailboxTransportSubmissionAssistant.Meeting Forward Notification Agent",
			"MailItemDeliver.",
			"MailItemDeliver.Approval Processing Agent",
			"MailItemDeliver.Content Aggregation Agent",
			"MailItemDeliver.Conversations Processing Agent",
			"MailItemDeliver.Group Escalation Agent",
			"MailItemDeliver.Index Delivery Agent",
			"MailItemDeliver.Inference Classification Agent",
			"MailItemDeliver.Mailbox Rules Agent",
			"MailItemDeliver.Meeting Message Processing Agent",
			"MailItemDeliver.Message Records Management Delivery Agent",
			"MailItemDeliver.People I Know Classification Agent",
			"MailItemDeliver.UnJournal Delivery Agent",
			"MessageWaitingIndicatorAssistant.",
			"OABGeneratorAssistant.",
			"Owa.AddSharedCalendarCommand",
			"Owa.AddSharedFolders",
			"Owa.AddTrustedSender",
			"Owa.ApplyConversationAction",
			"Owa.ConnectedAccountsNotification",
			"Owa.CopyItemBatch",
			"Owa.CreateAttachment",
			"Owa.CreateAttachmentFromForm",
			"Owa.CreateCalendarCommand",
			"Owa.CreateFolder",
			"Owa.CreateItem",
			"Owa.CreateOnlineMeeting",
			"Owa.CreatePersonaCommand",
			"Owa.CreateResendDraft",
			"Owa.DeleteAttachment",
			"Owa.DeleteCalendarCommand",
			"Owa.DeleteFolder",
			"Owa.DeleteItem",
			"Owa.DeleteItemBatch",
			"Owa.DeletePersonaCommand",
			"Owa.EmptyFolder",
			"Owa.FindConversation",
			"Owa.FindFolder",
			"Owa.FindItem",
			"Owa.FindPeople",
			"Owa.FindPlaces",
			"Owa.GetAttachment",
			"Owa.GetCalendarFolderConfigurationCommand",
			"Owa.GetCalendarFoldersCommand",
			"Owa.GetCalendarSharingPermissionsCommand",
			"Owa.GetClientAccessToken",
			"Owa.GetComplianceConfiguration",
			"Owa.GetConversationItems",
			"Owa.GetDaysUntilPasswordExpiration",
			"Owa.GetDlpPolicyTipsCommand",
			"Owa.GetEmailSignature",
			"Owa.GetExtensibilityContext",
			"Owa.GetFavorites",
			"Owa.GetFolder",
			"Owa.GetFolderMruConfiguration",
			"Owa.GetGroupCommand",
			"Owa.GetItem",
			"Owa.GetMailTips",
			"Owa.GetOptionSummary",
			"Owa.GetOtherMailboxConfiguration",
			"Owa.GetOwaUserConfiguration",
			"Owa.GetPeopleFilters",
			"Owa.GetPeopleIKnowGraphCommand",
			"Owa.GetPersona",
			"Owa.GetPersonaNotesCommand",
			"Owa.GetPersonaPhoto",
			"Owa.GetPersonaSuggestionsCommand",
			"Owa.GetReminders",
			"Owa.GetTaskFoldersCommand",
			"Owa.GetTimeZone",
			"Owa.GetUserAvailabilityInternalCommand",
			"Owa.GetUserOofSettings",
			"Owa.GetUserPhoto",
			"Owa.GetWacIframeUrl",
			"Owa.InstantMessageSignIn",
			"Owa.InstantMessageSignOut",
			"Owa.LogDatapoint",
			"Owa.MarkAllItemsAsRead",
			"Owa.MarkAsJunk",
			"Owa.MoveFolder",
			"Owa.MoveItem",
			"Owa.MoveItemBatch",
			"Owa.PerformReminderAction",
			"Owa.Provision",
			"Owa.SendCalendarSharingInviteCommand",
			"Owa.SendReadReceipt",
			"Owa.SetCalendarColorCommand",
			"Owa.SetCalendarSharingPermissionsCommand",
			"Owa.SetFolderMruConfiguration",
			"Owa.SetLayoutSettings",
			"Owa.SetTheme",
			"Owa.SetUserOofSettings",
			"Owa.SubscribeInternalCalendarCommand",
			"Owa.SubscribeToNotification",
			"Owa.SubscribeToPushNotification",
			"Owa.SyncCalendar",
			"Owa.SyncConversation",
			"Owa.SyncFolderHierarchy",
			"Owa.SyncFolderItems",
			"Owa.SyncPeople",
			"Owa.UnsubscribeToNotification",
			"Owa.UpdateFavoriteFolder",
			"Owa.UpdateFolder",
			"Owa.UpdateItem",
			"Owa.UpdateItemIsReadBatch",
			"Owa.UpdateMasterCategoryList",
			"Owa.UpdatePersonaCommand",
			"Owa.UpdateUserConfiguration",
			"Owa.UploadPhoto",
			"PeopleCentricTriageAssistant.",
			"PeopleRelevanceAssistant.",
			"POP3.auth",
			"POP3.dele",
			"POP3.list",
			"POP3.noop",
			"POP3.retr",
			"POP3.rset",
			"POP3.stat",
			"POP3.top",
			"POP3.uidl",
			"POP3.user",
			"Powershell.Get-InboxRule",
			"Powershell.Test-MAPIConnectivity",
			"ProvisioningAssistant.",
			"PublicFolderAssistant.",
			"PublicFolderSync.",
			"PublicFolderSync.ClearFolderProperties",
			"PublicFolderSync.CommitBatch",
			"PublicFolderSync.CreateFolder",
			"PublicFolderSync.DeleteFolder",
			"PublicFolderSync.EnumerateHierarchyChanges",
			"PublicFolderSync.FxCopyProperties",
			"PublicFolderSync.GetChangeManifestInitializeSyncContext",
			"PublicFolderSync.GetChangeManifestPersistSyncContext",
			"PublicFolderSync.GetDestinationFolderIdSet",
			"PublicFolderSync.GetDestinationMailboxFolder",
			"PublicFolderSync.GetDestinationSessionSpecificEntryId",
			"PublicFolderSync.GetFolderRec",
			"PublicFolderSync.GetSourceFolderIdSet",
			"PublicFolderSync.GetSourceMailboxFolder",
			"PublicFolderSync.GetSourceSessionSpecificEntryId",
			"PublicFolderSync.MapSourceToDestinationFolderId",
			"PublicFolderSync.MoveFolder",
			"PublicFolderSync.ProcessNextBatch",
			"PublicFolderSync.SetIcsState",
			"PublicFolderSync.SetSecurityDescriptor",
			"PublicFolderSync.UpdateDumpsterId",
			"PushNotificationAssistant.",
			"RCA/Mailbox.",
			"RCA/Mailbox.AbortSubmit",
			"RCA/Mailbox.AddressTypes",
			"RCA/Mailbox.CollapseRow",
			"RCA/Mailbox.CommitStream",
			"RCA/Mailbox.CopyProperties",
			"RCA/Mailbox.CopyTo",
			"RCA/Mailbox.CreateAttachment",
			"RCA/Mailbox.CreateBookmark",
			"RCA/Mailbox.CreateFolder",
			"RCA/Mailbox.CreateMessage",
			"RCA/Mailbox.DeleteAttachment",
			"RCA/Mailbox.DeleteFolder",
			"RCA/Mailbox.DeleteMessages",
			"RCA/Mailbox.DeleteProperties",
			"RCA/Mailbox.DeletePropertiesNoReplicate",
			"RCA/Mailbox.EmptyFolder",
			"RCA/Mailbox.ExpandRow",
			"RCA/Mailbox.FastTransferDestinationCopyOperationConfigure",
			"RCA/Mailbox.FastTransferDestinationPutBuffer",
			"RCA/Mailbox.FastTransferDestinationPutBufferExtended",
			"RCA/Mailbox.FastTransferGetIncrementalState",
			"RCA/Mailbox.FastTransferSourceCopyFolder",
			"RCA/Mailbox.FastTransferSourceCopyProperties",
			"RCA/Mailbox.FastTransferSourceCopyTo",
			"RCA/Mailbox.FastTransferSourceGetBuffer",
			"RCA/Mailbox.FindRow",
			"RCA/Mailbox.GetCollapseState",
			"RCA/Mailbox.GetContentsTable",
			"RCA/Mailbox.GetHierarchyTable",
			"RCA/Mailbox.GetIdsFromNames",
			"RCA/Mailbox.GetLocalReplicationIds",
			"RCA/Mailbox.GetMessageStatus",
			"RCA/Mailbox.GetNamesFromIDs",
			"RCA/Mailbox.GetPerUserGuid",
			"RCA/Mailbox.GetPerUserLongTermIds",
			"RCA/Mailbox.GetPropertiesAll",
			"RCA/Mailbox.GetPropertiesSpecific",
			"RCA/Mailbox.GetPropertyList",
			"RCA/Mailbox.GetReceiveFolder",
			"RCA/Mailbox.GetReceiveFolderTable",
			"RCA/Mailbox.GetSearchCriteria",
			"RCA/Mailbox.GetStreamSize",
			"RCA/Mailbox.HardDeleteMessages",
			"RCA/Mailbox.IdFromLongTermId",
			"RCA/Mailbox.ImportDelete",
			"RCA/Mailbox.ImportHierarchyChange",
			"RCA/Mailbox.ImportMessageChange",
			"RCA/Mailbox.ImportMessageMove",
			"RCA/Mailbox.ImportReads",
			"RCA/Mailbox.IncrementalConfig",
			"RCA/Mailbox.Logon",
			"RCA/Mailbox.LongTermIdFromId",
			"RCA/Mailbox.ModifyPermissions",
			"RCA/Mailbox.ModifyRules",
			"RCA/Mailbox.MoveCopyMessages",
			"RCA/Mailbox.MoveFolder",
			"RCA/Mailbox.OpenAttachment",
			"RCA/Mailbox.OpenCollector",
			"RCA/Mailbox.OpenEmbeddedMessage",
			"RCA/Mailbox.OpenFolder",
			"RCA/Mailbox.OpenMessage",
			"RCA/Mailbox.OpenStream",
			"RCA/Mailbox.PublicFolderIsGhosted",
			"RCA/Mailbox.QueryColumnsAll",
			"RCA/Mailbox.QueryPosition",
			"RCA/Mailbox.QueryRows",
			"RCA/Mailbox.ReadPerUserInformation",
			"RCA/Mailbox.ReadStream",
			"RCA/Mailbox.RegisterNotification",
			"RCA/Mailbox.Release",
			"RCA/Mailbox.ReloadCachedInformation",
			"RCA/Mailbox.Restrict",
			"RCA/Mailbox.SaveChangesAttachment",
			"RCA/Mailbox.SaveChangesMessage",
			"RCA/Mailbox.SeekRow",
			"RCA/Mailbox.SeekRowBookmark",
			"RCA/Mailbox.SeekStream",
			"RCA/Mailbox.SetCollapseState",
			"RCA/Mailbox.SetColumns",
			"RCA/Mailbox.SetLocalReplicaMidsetDeleted",
			"RCA/Mailbox.SetProperties",
			"RCA/Mailbox.SetReadFlag",
			"RCA/Mailbox.SetReadFlags",
			"RCA/Mailbox.SetSearchCriteria",
			"RCA/Mailbox.SetSizeStream",
			"RCA/Mailbox.SetSpooler",
			"RCA/Mailbox.SetTransport",
			"RCA/Mailbox.SortTable",
			"RCA/Mailbox.SpoolerLockMessage",
			"RCA/Mailbox.SubmitMessage",
			"RCA/Mailbox.TransportSend",
			"RCA/Mailbox.UpdateDeferredActionMessages",
			"RCA/Mailbox.WritePerUserInformation",
			"RCA/Mailbox.WriteStream",
			"RecipientDLExpansionEventBasedAssistant.",
			"RemindersAssistant.",
			"ResourceBookingAssistant.",
			"SharePointSignalStoreAssistant.",
			"SharingFolderAssistant.",
			"SharingPolicyAssistant.",
			"SiteMailboxAssistant.",
			"TopNAssistant.",
			"Task",
			"Admin"
		};
	}
}
