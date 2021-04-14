using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal static class AirSyncCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (AirSyncCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in AirSyncCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange ActiveSync";

		public static readonly ExPerformanceCounter CurrentNumberOfRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Current Requests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfIncomingProxyRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Incoming Proxy Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfOutgoingProxyRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Outgoing Proxy Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfWrongCASProxyRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Wrong CAS Proxy Requests Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter PID = new ExPerformanceCounter("MSExchange ActiveSync", "PID", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRequestTime = new ExPerformanceCounter("MSExchange ActiveSync", "Average Request Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageHangingTime = new ExPerformanceCounter("MSExchange ActiveSync", "Average Hang Time", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Requests Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfRequests
		});

		private static readonly ExPerformanceCounter RateOfGetHierarchy = new ExPerformanceCounter("MSExchange ActiveSync", "Get Hierarchy Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGetHierarchy = new ExPerformanceCounter("MSExchange ActiveSync", "Get Hierarchy Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfGetHierarchy
		});

		private static readonly ExPerformanceCounter RateOfMoveItems = new ExPerformanceCounter("MSExchange ActiveSync", "Move Items Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMoveItems = new ExPerformanceCounter("MSExchange ActiveSync", "Move Items Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMoveItems
		});

		private static readonly ExPerformanceCounter RateOfMeetingResponse = new ExPerformanceCounter("MSExchange ActiveSync", "Meeting Response Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMeetingResponse = new ExPerformanceCounter("MSExchange ActiveSync", "Meeting Response Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMeetingResponse
		});

		private static readonly ExPerformanceCounter RateOfFolderSyncsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Sync Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFolderSyncs = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Sync Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfFolderSyncsCurrent
		});

		private static readonly ExPerformanceCounter RateOfFolderUpdatesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Update Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFolderUpdates = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Update Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfFolderUpdatesCurrent
		});

		private static readonly ExPerformanceCounter RateOfFolderCreatesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Create Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFolderCreates = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Create Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfFolderCreatesCurrent
		});

		private static readonly ExPerformanceCounter RateOfFolderDeletesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Delete Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfFolderDeletes = new ExPerformanceCounter("MSExchange ActiveSync", "Folder Delete Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfFolderDeletesCurrent
		});

		private static readonly ExPerformanceCounter RateOfOptionsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Options Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfOptions = new ExPerformanceCounter("MSExchange ActiveSync", "Options Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfOptionsCurrent
		});

		private static readonly ExPerformanceCounter RateOfSyncRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Sync Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSyncRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Sync Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSyncRequests
		});

		private static readonly ExPerformanceCounter RateOfRecoverySyncRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Recovery Sync Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfRecoverySyncRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Recovery Sync Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfRecoverySyncRequests
		});

		private static readonly ExPerformanceCounter RateOfItemEstimateRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Get Item Estimate Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfItemEstimateRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Get Item Estimate Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfItemEstimateRequests
		});

		private static readonly ExPerformanceCounter RateOfCreateCollectionsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Create Collection Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfCreateCollections = new ExPerformanceCounter("MSExchange ActiveSync", "Create Collection Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfCreateCollectionsCurrent
		});

		private static readonly ExPerformanceCounter RateOfMoveCollectionsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Move Collection Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMoveCollections = new ExPerformanceCounter("MSExchange ActiveSync", "Move Collection Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMoveCollectionsCurrent
		});

		private static readonly ExPerformanceCounter RateOfDeleteCollectionsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Delete Collection Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfDeleteCollections = new ExPerformanceCounter("MSExchange ActiveSync", "Delete Collection Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfDeleteCollectionsCurrent
		});

		private static readonly ExPerformanceCounter RateOfGetAttachmentsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Get Attachment Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGetAttachments = new ExPerformanceCounter("MSExchange ActiveSync", "Get Attachment Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfGetAttachmentsCurrent
		});

		private static readonly ExPerformanceCounter RateOfIRMMailsDownloadsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "IRM-protected Message Downloads/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfIRMMailsDownloads = new ExPerformanceCounter("MSExchange ActiveSync", "IRM-protected Message Downloads - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfIRMMailsDownloadsCurrent
		});

		private static readonly ExPerformanceCounter RateOfSendIRMMailsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Send IRM-protected Messages/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSendIRMMails = new ExPerformanceCounter("MSExchange ActiveSync", "Send IRM-protected Messages - Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSendIRMMailsCurrent
		});

		private static readonly ExPerformanceCounter RateOfSendMailsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Send Mail Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSendMails = new ExPerformanceCounter("MSExchange ActiveSync", "Send Mail Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSendMailsCurrent
		});

		private static readonly ExPerformanceCounter RateOfSmartReplysCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Smart Reply Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSmartReplys = new ExPerformanceCounter("MSExchange ActiveSync", "Smart Reply Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSmartReplysCurrent
		});

		private static readonly ExPerformanceCounter RateOfSmartForwardsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Smart Forward Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSmartForwards = new ExPerformanceCounter("MSExchange ActiveSync", "Smart Forward Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSmartForwardsCurrent
		});

		private static readonly ExPerformanceCounter RateOfSearchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Search Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSearches = new ExPerformanceCounter("MSExchange ActiveSync", "Search Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSearchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfGALSearchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "GAL Searches/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfGALSearches = new ExPerformanceCounter("MSExchange ActiveSync", "GAL Search Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfGALSearchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfMailboxSearchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Searches/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxSearches = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Search Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMailboxSearchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfDocumentLibrarySearchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Document Library Searches/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfDocumentLibrarySearches = new ExPerformanceCounter("MSExchange ActiveSync", "Document Library Search Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfDocumentLibrarySearchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfItemOperationsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Item Operations Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfItemOperations = new ExPerformanceCounter("MSExchange ActiveSync", "Item Operations Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfItemOperationsCurrent
		});

		private static readonly ExPerformanceCounter RateOfDocumentLibraryFetchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Document Library Fetch Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfDocumentLibraryFetches = new ExPerformanceCounter("MSExchange ActiveSync", "Document Library Fetch Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfDocumentLibraryFetchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfMailboxItemFetchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Item Fetch Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxItemFetches = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Item Fetch Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMailboxItemFetchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfEmptyFolderContentsCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Empty Folder Contents/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfEmptyFolderContents = new ExPerformanceCounter("MSExchange ActiveSync", "Empty Folder Contents Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfEmptyFolderContentsCurrent
		});

		private static readonly ExPerformanceCounter RateOfMailboxAttachmentFetchesCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Attachment Fetch Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfMailboxAttachmentFetches = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Attachment Fetch Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfMailboxAttachmentFetchesCurrent
		});

		private static readonly ExPerformanceCounter RateOfSettingsRequestCurrent = new ExPerformanceCounter("MSExchange ActiveSync", "Settings Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSettingsRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Settings Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfSettingsRequestCurrent
		});

		private static readonly ExPerformanceCounter RateOfPing = new ExPerformanceCounter("MSExchange ActiveSync", "Ping Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfPing = new ExPerformanceCounter("MSExchange ActiveSync", "Ping Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfPing
		});

		public static readonly ExPerformanceCounter CurrentlyPendingPing = new ExPerformanceCounter("MSExchange ActiveSync", "Ping Commands Pending", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfDroppedPing = new ExPerformanceCounter("MSExchange ActiveSync", "Ping Commands Dropped/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfDroppedPing = new ExPerformanceCounter("MSExchange ActiveSync", "Ping Dropped Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfDroppedPing
		});

		public static readonly ExPerformanceCounter HeartbeatInterval = new ExPerformanceCounter("MSExchange ActiveSync", "Heartbeat Interval", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfProvisionRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Provision Commands/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfProvisionRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Provision Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfProvisionRequests
		});

		public static readonly ExPerformanceCounter NumberOfServerItemConversionFailure = new ExPerformanceCounter("MSExchange ActiveSync", "Failed Item Conversion Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfBadItemReportsGenerated = new ExPerformanceCounter("MSExchange ActiveSync", "Bad Item Reports Generated Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfProxyLoginSent = new ExPerformanceCounter("MSExchange ActiveSync", "Proxy Logon Commands Sent Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfProxyLoginReceived = new ExPerformanceCounter("MSExchange ActiveSync", "Proxy Logon Received Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SyncStateKbLeftCompressed = new ExPerformanceCounter("MSExchange ActiveSync", "Sync State KBytes Left Compressed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SyncStateKbTotal = new ExPerformanceCounter("MSExchange ActiveSync", "Sync State KBytes Total", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CurrentlyPendingSync = new ExPerformanceCounter("MSExchange ActiveSync", "Sync Commands Pending", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfDroppedSync = new ExPerformanceCounter("MSExchange ActiveSync", "Sync Commands Dropped/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfDroppedSync = new ExPerformanceCounter("MSExchange ActiveSync", "Sync Dropped Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfDroppedSync
		});

		private static readonly ExPerformanceCounter RateOfConflictingConcurrentSync = new ExPerformanceCounter("MSExchange ActiveSync", "Conflicting Concurrent Sync/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfConflictingConcurrentSync = new ExPerformanceCounter("MSExchange ActiveSync", "Conflicting Concurrent Sync Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfConflictingConcurrentSync
		});

		public static readonly ExPerformanceCounter NumberOfADPolicyQueriesOnReconnect = new ExPerformanceCounter("MSExchange ActiveSync", "Number of AD Policy Queries on Reconnect", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfNotificationManagerInCache = new ExPerformanceCounter("MSExchange ActiveSync", "Number of Notification Manager Objects in Memory", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfAvailabilityRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Availability Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfAvailabilityRequests = new ExPerformanceCounter("MSExchange ActiveSync", "Availability Requests Total", string.Empty, null, new ExPerformanceCounter[]
		{
			AirSyncCounters.RateOfAvailabilityRequests
		});

		public static readonly ExPerformanceCounter RatePerMinuteOfTransientMailboxConnectionFailures = new ExPerformanceCounter("MSExchange ActiveSync", "Transient Mailbox Connection Failures/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfMailboxOfflineErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Mailbox Offline Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfTransientStorageErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Transient Storage Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfPermanentStorageErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Permanent Storage Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfTransientActiveDirectoryErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Transient Active Directory Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfPermanentActiveDirectoryErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Permanent Active Directory Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RatePerMinuteOfTransientErrors = new ExPerformanceCounter("MSExchange ActiveSync", "Transient Errors/minute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRpcLatency = new ExPerformanceCounter("MSExchange ActiveSync", "Average RPC Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageLdapLatency = new ExPerformanceCounter("MSExchange ActiveSync", "Average LDAP Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AutoBlockedDevices = new ExPerformanceCounter("MSExchange ActiveSync", "Number of auto-blocked devices", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			AirSyncCounters.NumberOfRequests,
			AirSyncCounters.CurrentNumberOfRequests,
			AirSyncCounters.NumberOfIncomingProxyRequests,
			AirSyncCounters.NumberOfOutgoingProxyRequests,
			AirSyncCounters.NumberOfWrongCASProxyRequests,
			AirSyncCounters.PID,
			AirSyncCounters.AverageRequestTime,
			AirSyncCounters.AverageHangingTime,
			AirSyncCounters.NumberOfGetHierarchy,
			AirSyncCounters.NumberOfMoveItems,
			AirSyncCounters.NumberOfMeetingResponse,
			AirSyncCounters.NumberOfFolderSyncs,
			AirSyncCounters.NumberOfFolderUpdates,
			AirSyncCounters.NumberOfFolderCreates,
			AirSyncCounters.NumberOfFolderDeletes,
			AirSyncCounters.NumberOfOptions,
			AirSyncCounters.NumberOfSyncRequests,
			AirSyncCounters.NumberOfRecoverySyncRequests,
			AirSyncCounters.NumberOfItemEstimateRequests,
			AirSyncCounters.NumberOfCreateCollections,
			AirSyncCounters.NumberOfMoveCollections,
			AirSyncCounters.NumberOfDeleteCollections,
			AirSyncCounters.NumberOfGetAttachments,
			AirSyncCounters.NumberOfIRMMailsDownloads,
			AirSyncCounters.NumberOfSendIRMMails,
			AirSyncCounters.NumberOfSendMails,
			AirSyncCounters.NumberOfSmartReplys,
			AirSyncCounters.NumberOfSmartForwards,
			AirSyncCounters.NumberOfSearches,
			AirSyncCounters.NumberOfGALSearches,
			AirSyncCounters.NumberOfMailboxSearches,
			AirSyncCounters.NumberOfDocumentLibrarySearches,
			AirSyncCounters.NumberOfItemOperations,
			AirSyncCounters.NumberOfDocumentLibraryFetches,
			AirSyncCounters.NumberOfMailboxItemFetches,
			AirSyncCounters.NumberOfEmptyFolderContents,
			AirSyncCounters.NumberOfMailboxAttachmentFetches,
			AirSyncCounters.NumberOfSettingsRequests,
			AirSyncCounters.NumberOfPing,
			AirSyncCounters.CurrentlyPendingPing,
			AirSyncCounters.NumberOfDroppedPing,
			AirSyncCounters.HeartbeatInterval,
			AirSyncCounters.NumberOfProvisionRequests,
			AirSyncCounters.NumberOfServerItemConversionFailure,
			AirSyncCounters.NumberOfBadItemReportsGenerated,
			AirSyncCounters.NumberOfProxyLoginSent,
			AirSyncCounters.NumberOfProxyLoginReceived,
			AirSyncCounters.SyncStateKbLeftCompressed,
			AirSyncCounters.SyncStateKbTotal,
			AirSyncCounters.CurrentlyPendingSync,
			AirSyncCounters.NumberOfDroppedSync,
			AirSyncCounters.NumberOfConflictingConcurrentSync,
			AirSyncCounters.NumberOfADPolicyQueriesOnReconnect,
			AirSyncCounters.NumberOfNotificationManagerInCache,
			AirSyncCounters.NumberOfAvailabilityRequests,
			AirSyncCounters.RatePerMinuteOfTransientMailboxConnectionFailures,
			AirSyncCounters.RatePerMinuteOfMailboxOfflineErrors,
			AirSyncCounters.RatePerMinuteOfTransientStorageErrors,
			AirSyncCounters.RatePerMinuteOfPermanentStorageErrors,
			AirSyncCounters.RatePerMinuteOfTransientActiveDirectoryErrors,
			AirSyncCounters.RatePerMinuteOfPermanentActiveDirectoryErrors,
			AirSyncCounters.RatePerMinuteOfTransientErrors,
			AirSyncCounters.AverageRpcLatency,
			AirSyncCounters.AverageLdapLatency,
			AirSyncCounters.AutoBlockedDevices
		};
	}
}
