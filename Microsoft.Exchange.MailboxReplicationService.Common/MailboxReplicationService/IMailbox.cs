using System;
using System.Collections.Generic;
using System.Net;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IMailbox : IDisposable
	{
		LatencyInfo GetLatencyInfo();

		SessionStatistics GetSessionStatistics(SessionStatisticsFlags statisticsTypes);

		bool IsConnected();

		void ConfigADConnection(string domainControllerName, string configDomainControllerName, NetworkCredential cred);

		void ConfigPreferredADConnection(string preferredDomainControllerName);

		void Config(IReservation reservation, Guid primaryMailboxGuid, Guid physicalMailboxGuid, TenantPartitionHint partitionHint, Guid mdbGuid, MailboxType mbxType, Guid? mailboxContainerGuid = null);

		void ConfigRestore(MailboxRestoreType restoreFlags);

		void ConfigMDBByName(string mdbName);

		void ConfigMailboxOptions(MailboxOptions options);

		void ConfigPst(string filePath, int? contentCodePage);

		void ConfigEas(NetworkCredential userCredential, SmtpAddress smtpAddress, Guid mailboxGuid, string remoteHostName = null);

		void ConfigOlc(OlcMailboxConfiguration config);

		MailboxInformation GetMailboxInformation();

		void Connect(MailboxConnectFlags connectFlags);

		bool IsCapabilitySupported(MRSProxyCapabilities capability);

		bool IsMailboxCapabilitySupported(MailboxCapabilities capability);

		void Disconnect();

		void SetInTransitStatus(InTransitStatus status, out bool onlineMoveSupported);

		void SeedMBICache();

		MailboxServerInformation GetMailboxServerInformation();

		VersionInformation GetVersion();

		void SetOtherSideVersion(VersionInformation otherSideVersion);

		List<FolderRec> EnumerateFolderHierarchy(EnumerateFolderHierarchyFlags flags, PropTag[] additionalPtagsToLoad);

		List<WellKnownFolder> DiscoverWellKnownFolders(int flags);

		void DeleteMailbox(int flags);

		NamedPropData[] GetNamesFromIDs(PropTag[] pta);

		PropTag[] GetIDsFromNames(bool createIfNotExists, NamedPropData[] npa);

		byte[] GetSessionSpecificEntryId(byte[] entryId);

		MappedPrincipal[] ResolvePrincipals(MappedPrincipal[] principals);

		bool UpdateRemoteHostName(string value);

		ADUser GetADUser();

		void UpdateMovedMailbox(UpdateMovedMailboxOperation op, ADUser remoteRecipientData, string domainController, out ReportEntry[] entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, ArchiveStatusFlags archiveStatus, UpdateMovedMailboxFlags updateMovedMailboxFlags, Guid? newMailboxContainerGuid, CrossTenantObjectId newUnifiedMailboxId);

		RawSecurityDescriptor GetMailboxSecurityDescriptor();

		RawSecurityDescriptor GetUserSecurityDescriptor();

		void AddMoveHistoryEntry(MoveHistoryEntryInternal mhei, int maxMoveHistoryLength);

		ServerHealthStatus CheckServerHealth();

		PropValueData[] GetProps(PropTag[] ptags);

		byte[] GetReceiveFolderEntryId(string msgClass);

		Guid[] ResolvePolicyTag(string policyTagStr);

		string LoadSyncState(byte[] key);

		MessageRec SaveSyncState(byte[] key, string syncState);

		Guid StartIsInteg(List<uint> mailboxCorruptionTypes);

		List<StoreIntegrityCheckJob> QueryIsInteg(Guid isIntegRequestGuid);
	}
}
