using System;
using System.Collections.Generic;
using System.Net.Security;
using System.ServiceModel;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ServiceContract(SessionMode = SessionMode.Required)]
	internal interface IMailboxReplicationProxyService
	{
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void CloseHandle(long handle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		ProxyServerInformation FindServerByDatabaseOrMailbox(string databaseId, Guid? physicalMailboxGuid, Guid? primaryMailboxGuid, byte[] tenantPartitionHintBytes);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		IEnumerable<ContainerMailboxInformation> GetMailboxContainerMailboxes(Guid mdbGuid, Guid primaryMailboxGuid);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		bool ArePublicFoldersReadyForMigrationCompletion();

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		List<Guid> GetPublicFolderMailboxesExchangeGuids();

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		DataExportBatch DataExport_ExportData2(long dataExportHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void DataExport_CancelExport(long dataExportHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDataImport_ImportBuffer(long dataImportHandle, int opcode, byte[] data);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDataImport_Flush(long dataImportHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		FolderRec IFolder_GetFolderRec2(long folderHandle, int[] additionalPtagsToLoad);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		FolderRec IFolder_GetFolderRec3(long folderHandle, int[] additionalPtagsToLoad, int flags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<MessageRec> IFolder_EnumerateMessagesPaged2(long folderHandle, EnumerateMessagesFlags emFlags, int[] additionalPtagsToLoad, out bool moreData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<MessageRec> IFolder_EnumerateMessagesNextBatch(long folderHandle, out bool moreData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		byte[] IFolder_GetSecurityDescriptor(long folderHandle, int secProp);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IFolder_SetContentsRestriction(long folderHandle, RestrictionData restriction);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		PropValueData[] IFolder_GetProps(long folderHandle, int[] pta);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IFolder_DeleteMessages(long folderHandle, byte[][] entryIds);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		RuleData[] IFolder_GetRules(long folderHandle, int[] extraProps);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropValueData[][] IFolder_GetACL(long folderHandle, int secProp);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropValueData[][] IFolder_GetExtendedAcl(long folderHandle, int aclFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IFolder_GetSearchCriteria(long folderHandle, out RestrictionData restriction, out byte[][] entryIDs, out int searchState);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<MessageRec> IFolder_LookupMessages(long folderHandle, int ptagToLookup, byte[][] keysToLookup, int[] additionalPtagsToLoad);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropProblemData[] IFolder_SetProps(long folderHandle, PropValueData[] pva);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		PropValueData[] ISourceFolder_GetProps(long folderHandle, int[] pta);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void ISourceFolder_GetSearchCriteria(long folderHandle, out RestrictionData restriction, out byte[][] entryIDs, out int searchState);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch ISourceFolder_CopyTo(long folderHandle, int flags, int[] excludeTags, byte[] targetObjectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		DataExportBatch ISourceFolder_Export2(long folderHandle, int[] excludeTags, byte[] targetObjectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		DataExportBatch ISourceFolder_ExportMessages(long folderHandle, int flags, byte[][] entryIds, byte[] targetObjectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		FolderChangesManifest ISourceFolder_EnumerateChanges(long folderHandle, bool catchup);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		FolderChangesManifest ISourceFolder_EnumerateChanges2(long folderHandle, int flags, int maxChanges);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		List<MessageRec> ISourceFolder_EnumerateMessagesPaged(long folderHandle, int maxPageSize);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		int ISourceFolder_GetEstimatedItemCount(long folderHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropProblemData[] IDestinationFolder_SetProps(long folderHandle, PropValueData[] pva);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropProblemData[] IDestinationFolder_SetSecurityDescriptor(long folderHandle, int secProp, byte[] sdData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		bool IDestinationFolder_SetSearchCriteria(long folderHandle, RestrictionData restriction, byte[][] entryIDs, int searchFlags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		long IDestinationFolder_GetFxProxy(long folderHandle, out byte[] objectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		long IDestinationFolder_GetFxProxy2(long folderHandle, int flags, out byte[] objectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationFolder_DeleteMessages(long folderHandle, byte[][] entryIds);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationFolder_SetReadFlagsOnMessages(long folderHandle, int flags, byte[][] entryIds);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationFolder_SetMessageProps(long folderHandle, byte[] entryId, PropValueData[] propValues);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationFolder_SetRules(long folderHandle, RuleData[] rules);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationFolder_SetACL(long folderHandle, int secProp, PropValueData[][] aclData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationFolder_SetExtendedAcl(long folderHandle, int aclFlags, PropValueData[][] aclData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		Guid IDestinationFolder_LinkMailPublicFolder(long folderHandle, LinkMailPublicFolderFlags flags, byte[] objectId);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		Guid IReservationManager_ReserveResources(Guid mailboxGuid, byte[] partitionHintBytes, Guid mdbGuid, int flags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IReservationManager_ReleaseResources(Guid reservationId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		int IMailbox_ReserveResources(Guid reservationId, Guid resourceId, int reservationType);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IMailbox_Config3(Guid primaryMailboxGuid, Guid physicalMailboxGuid, Guid mdbGuid, string mdbName, [MessageParameter(Name = "options")] MailboxType mbxType, int proxyControlFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IMailbox_Config4(Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		long IMailbox_Config5(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IMailbox_Config6(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, string filePath, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IMailbox_Config7(Guid reservationId, Guid primaryMailboxGuid, Guid physicalMailboxGuid, byte[] partitionHint, Guid mdbGuid, string mdbName, MailboxType mbxType, int proxyControlFlags, int localMailboxFlags, Guid? mailboxContainerGuid);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigureProxyService(ProxyConfiguration configuration);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigADConnection(long mailboxHandle, string domainControllerName, string userName, string userDomain, string userPassword);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_ConfigEas(long mailboxHandle, string password, string address);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigEas2(long mailboxHandle, string password, string address, Guid mailboxGuid, string remoteHostName);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigPreferredADConnection(long mailboxHandle, string preferredDomainControllerName);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_ConfigPst(long mailboxHandle, string filePath, int? contentCodePage);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigRestore(long mailboxHandle, int restoreFlags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigOlc(long mailboxHandle, OlcMailboxConfiguration config);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		MailboxInformation IMailbox_GetMailboxInformation(long mailboxHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_Connect(long mailboxHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_Connect2(long mailboxHandle, int connectFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_Disconnect(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_ConfigMailboxOptions(long mailboxHandle, int options);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MailboxServerInformation IMailbox_GetMailboxServerInformation(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_SetOtherSideVersion(long mailboxHandle, VersionInformation otherSideInfo);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_SetInTransitStatus(long mailboxHandle, int status, out bool onlineMoveSupported);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_SeedMBICache(long mailboxHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<FolderRec> IMailbox_EnumerateFolderHierarchyPaged2(long mailboxHandle, EnumerateFolderHierarchyFlags flags, int[] additionalPtagsToLoad, out bool moreData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<FolderRec> IMailbox_EnumerateFolderHierarchyNextBatch(long mailboxHandle, out bool moreData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		List<WellKnownFolder> IMailbox_DiscoverWellKnownFolders(long mailboxHandle, int flags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		bool IMailbox_IsMailboxCapabilitySupported(long mailboxHandle, MailboxCapabilities capability);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		bool IMailbox_IsMailboxCapabilitySupported2(long mailboxHandle, int capability);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_DeleteMailbox(long mailboxHandle, int flags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		NamedPropData[] IMailbox_GetNamesFromIDs(long mailboxHandle, int[] pta);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		int[] IMailbox_GetIDsFromNames(long mailboxHandle, bool createIfNotExists, NamedPropData[] npa);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		byte[] IMailbox_GetSessionSpecificEntryId(long mailboxHandle, byte[] entryId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		bool IMailbox_UpdateRemoteHostName(long mailboxHandle, string value);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		string IMailbox_GetADUser(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_UpdateMovedMailbox(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IMailbox_UpdateMovedMailbox2(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_UpdateMovedMailbox3(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_UpdateMovedMailbox4(long mailboxHandle, UpdateMovedMailboxOperation op, string remoteRecipientData, string domainController, out string entries, Guid newDatabaseGuid, Guid newArchiveDatabaseGuid, string archiveDomain, int archiveStatus, int updateMovedMailboxFlags, Guid? newMailboxContainerGuid, byte[] newUnifiedMailboxIdData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MappedPrincipal[] IMailbox_GetPrincipalsFromMailboxGuids(long mailboxHandle, Guid[] mailboxGuids);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		Guid[] IMailbox_GetMailboxGuidsFromPrincipals(long mailboxHandle, MappedPrincipal[] principals);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		MappedPrincipal[] IMailbox_ResolvePrincipals(long mailboxHandle, MappedPrincipal[] principals);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		byte[] IMailbox_GetMailboxSecurityDescriptor(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		byte[] IMailbox_GetUserSecurityDescriptor(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_AddMoveHistoryEntry(long mailboxHandle, string mheData, int maxMoveHistoryLength);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IMailbox_CheckServerHealth(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		ServerHealthStatus IMailbox_CheckServerHealth2(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropValueData[] IMailbox_GetProps(long mailboxHandle, int[] ptags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		byte[] IMailbox_GetReceiveFolderEntryId(long mailboxHandle, string msgClass);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		SessionStatistics IMailbox_GetSessionStatistics(long mailboxHandle, int statisticsTypes);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		Guid IMailbox_StartIsInteg(long mailboxHandle, List<uint> mailboxCorruptionTypes);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<StoreIntegrityCheckJob> IMailbox_QueryIsInteg(long mailboxHandle, Guid isIntegRequestGuid);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		byte[] ISourceMailbox_GetMailboxBasicInfo(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		byte[] ISourceMailbox_GetMailboxBasicInfo2(long mailboxHandle, int signatureFlags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		long ISourceMailbox_GetFolder(long mailboxHandle, byte[] entryId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		PropValueData[] ISourceMailbox_GetProps(long mailboxHandle, int[] ptags);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		DataExportBatch ISourceMailbox_Export2(long mailboxHandle, int[] excludeProps, byte[] targetObjectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		MailboxChangesManifest ISourceMailbox_EnumerateHierarchyChanges(long mailboxHandle, bool catchup);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MailboxChangesManifest ISourceMailbox_EnumerateHierarchyChanges2(long mailboxHandle, int flags, int maxChanges);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch ISourceMailbox_GetMailboxSyncState(long mailboxHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long ISourceMailbox_SetMailboxSyncState(long mailboxHandle, DataExportBatch firstBatch);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch ISourceMailbox_ExportMessageBatch2(long mailboxHandle, List<MessageRec> messages, byte[] targetObjectData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch ISourceMailbox_ExportMessages(long mailboxHandle, List<MessageRec> messages, int flags, int[] excludeProps, byte[] targetObjectData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch ISourceMailbox_ExportFolders(long mailboxHandle, List<byte[]> folderIds, int exportFoldersDataToCopyFlags, int folderRecFlags, int[] additionalFolderRecProps, int copyPropertiesFlags, int[] excludeProps, int extendedAclFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<ReplayActionResult> ISourceMailbox_ReplayActions(long mailboxHandle, List<ReplayAction> actions);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		List<ItemPropertiesBase> ISourceMailbox_GetMailboxSettings(long mailboxHandle, int flags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		bool IDestinationMailbox_MailboxExists(long mailboxHandle);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		CreateMailboxResult IDestinationMailbox_CreateMailbox(long mailboxHandle, byte[] mailboxData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		CreateMailboxResult IDestinationMailbox_CreateMailbox2(long mailboxHandle, byte[] mailboxData, int sourceSignatureFlags);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationMailbox_ProcessMailboxSignature(long mailboxHandle, byte[] mailboxData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		DataExportBatch IDestinationMailbox_LoadSyncState2(long mailboxHandle, byte[] key);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IDestinationMailbox_SaveSyncState2(long mailboxHandle, byte[] key, DataExportBatch firstBatch);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IDestinationMailbox_GetFolder(long mailboxHandle, byte[] entryId);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		long IDestinationMailbox_GetFxProxy(long mailboxHandle, out byte[] objectData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		PropProblemData[] IDestinationMailbox_SetProps(long mailboxHandle, PropValueData[] pva);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		long IDestinationMailbox_GetFxProxyPool(long mailboxHandle, byte[][] folderIds, out byte[] objectData);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_CreateFolder(long mailboxHandle, FolderRec sourceFolder, bool failIfExists);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_CreateFolder2(long mailboxHandle, FolderRec folderRec, bool failIfExists, out byte[] newFolderId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_CreateFolder3(long mailboxHandle, FolderRec folderRec, int createFolderFlags, out byte[] newFolderId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_MoveFolder(long mailboxHandle, byte[] folderId, byte[] oldParentId, byte[] newParentId);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_DeleteFolder(long mailboxHandle, FolderRec folderRec);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		void IDestinationMailbox_SetMailboxSecurityDescriptor(long mailboxHandle, byte[] sdData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationMailbox_SetUserSecurityDescriptor(long mailboxHandle, byte[] sdData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationMailbox_PreFinalSyncDataProcessing(long mailboxHandle, int? sourceMailboxVersion);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		int IDestinationMailbox_CheckDataGuarantee(long mailboxHandle, DateTime commitTimestamp, out byte[] failureReasonData);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationMailbox_ForceLogRoll(long mailboxHandle);

		[OperationContract]
		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		List<ReplayAction> IDestinationMailbox_GetActions(long mailboxHandle, string replaySyncState, int maxNumberOfActions);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		void IDestinationMailbox_SetMailboxSettings(long mailboxHandle, ItemPropertiesBase item);

		[FaultContract(typeof(MailboxReplicationServiceFault), ProtectionLevel = ProtectionLevel.EncryptAndSign)]
		[OperationContract]
		MigrationAccount[] SelectAccountsToMigrate(long maximumAccounts, long? maximumTotalSize, int? constraintId);
	}
}
