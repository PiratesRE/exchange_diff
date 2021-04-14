using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3E783D77-6B93-4270-ABE2-0EBAF1905DF4")]
	[ComImport]
	internal interface IExRpcAdmin
	{
		[PreserveSig]
		int HrGetServerVersion(out short pwMajor, out short pwMinor);

		[PreserveSig]
		int HrGetMailboxInfoSize(out int cbInfo);

		[PreserveSig]
		int HrDeletePrivateMailbox([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int ulFlags);

		[PreserveSig]
		int HrGetMailboxBasicInfo([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] rgbInfo, int cbInfo);

		[PreserveSig]
		int HrSetMailboxBasicInfo([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] rgbInfo, int cbInfo);

		[PreserveSig]
		int HrNotifyOnDSChange([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int ulObject);

		[PreserveSig]
		int HrListMdbStatus(int cMdb, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgguidMdb, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] rgulMdbStatus);

		[PreserveSig]
		unsafe int HrReadMapiEvents([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, long llStartEvent, int cEventWanted, int cEventsToCheck, [In] SRestriction* pFilter, int ulFlags, out int pcEventActual, out SafeExLinkedMemoryHandle pEvents, out long pllEndCounter);

		[PreserveSig]
		int HrReadLastMapiEvent([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, out SafeExLinkedMemoryHandle pEvent);

		[PreserveSig]
		int HrSaveWatermarks([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, int cWM, IntPtr pWMs);

		[PreserveSig]
		int HrGetWatermarks([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidConsumer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out int pcWM, out SafeExMemoryHandle pWMs);

		[PreserveSig]
		int HrGetWatermarksForMailbox([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, out int pcWMs, out IntPtr ppWMs);

		[PreserveSig]
		int HrDeleteWatermarksForMailbox([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, out int pcDel);

		[PreserveSig]
		int HrDeleteWatermarksForConsumer([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidConsumer, out int pcDel);

		[PreserveSig]
		int HrGetMailboxTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [In] uint flags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrGetLogonTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrGetMailboxSecurityDescriptor([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out SafeExMemoryHandle ppntsd, out int pcntsd);

		[PreserveSig]
		int HrSetMailboxSecurityDescriptor([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pntsd, int cntsd);

		[PreserveSig]
		int HrMountDatabase([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidStorageGroup, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int ulFlags);

		[PreserveSig]
		int HrUnmountDatabase([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidStorageGroup, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int grfFlags);

		[PreserveSig]
		int HrFlushCache(out int pcMDBs, out SafeExMemoryHandle pCheckpointStatus);

		[PreserveSig]
		int HrGetLastBackupTimes([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime);

		[PreserveSig]
		int HrGetLastBackupInfo([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime, out long ftLastDifferentialBackup, out long ftLastCopyBackup, out int SnapFull, out int SnapIncremental, out int SnapDifferential, out int SnapCopy);

		[PreserveSig]
		int HrPurgeCachedMailboxObject([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[PreserveSig]
		int HrPurgeCachedMdbObject([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[PreserveSig]
		int HrClearAbsentInDsFlagOnMailbox([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[PreserveSig]
		int HrSyncMailboxesWithDS([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[PreserveSig]
		int HrHasLocalReplicas([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.Bool)] out bool fHasReplicas);

		[PreserveSig]
		int HrListAllMdbStatus([MarshalAs(UnmanagedType.Bool)] bool fBasicInformation, out int pcMdbs, out SafeExMemoryHandle pMdbStatus);

		[PreserveSig]
		int HrGetReplicaInformationTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrSyncMailboxWithDS([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[PreserveSig]
		int HrCiCreatePropertyStore([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance);

		[PreserveSig]
		int HrCiDeletePropertyStore([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiAddMappings([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cEntryIds, _SBinaryArray* pEntryIdList, [MarshalAs(UnmanagedType.LPArray)] long[] pReceiveTimes, uint ulBatchID, out uint* rgulDocumentIds);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		int HrCiDeleteMapping([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		int HrCiDeleteFolderMapping([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId);

		[PreserveSig]
		int HrCiDeleteMailboxMapping([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiUpdateStatesByDocumentIdsOnBatchCompletion([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, int cDocumentIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulDocumentIds, _SBinaryArray* pEntryIdList, [MarshalAs(UnmanagedType.LPArray)] int[] rgHresults, uint ulBatchId);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiUpdateStatesByDocumentIdsOnBatchStart([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, int cDocumentIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulDocumentIds, _SBinaryArray* pEntryIdList, [MarshalAs(UnmanagedType.Bool)] bool fRetryStatesExpected, uint ulBatchId);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiGetDataFromDocumentIds([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, int cDocumentIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulDocumentIds, _SBinaryArray* pEntryIdList, out short* rgsStates, out uint* rgulBatchIds);

		[Obsolete("use HrCiEnumerateRetryTable", true)]
		[PreserveSig]
		unsafe int HrCiEnumerateRetryList([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, out int cDocumentIds, out uint* rgulDocumentIds, out _SBinaryArray* pEntryIdList);

		[PreserveSig]
		int HrCiMoveDocument([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryIdSource, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryIdSource, int cbEntryIdTarget, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryIdTarget);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiGetEntryIdFromDocumentId([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, uint ulDocumentId, out int cbEntryId, out byte* pEntryId, out Guid pguidMailbox);

		[PreserveSig]
		int HrCiGetWaterMark([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.Bool)] bool fIsHighWatermark, out ulong ullWatermark, out System.Runtime.InteropServices.ComTypes.FILETIME lastAccessTime);

		[PreserveSig]
		int HrCiSetWaterMark([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.Bool)] bool fIsHighWatermark, ulong ullWatermark);

		[PreserveSig]
		int HrCiGetMailboxState([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out System.Runtime.InteropServices.ComTypes.FILETIME ftStart, out uint ulState, out ulong ullEventCounter);

		[PreserveSig]
		int HrCiSetMailboxStateAndStartDate([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] ref System.Runtime.InteropServices.ComTypes.FILETIME pftStart, uint ulState);

		[PreserveSig]
		int HrCiSetMailboxState([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, uint ulState);

		[PreserveSig]
		int HrCiSetMailboxEventCounter([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, ulong ullEventCounter);

		[PreserveSig]
		int HrCiEnumerateMailboxesByState([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, uint ulState, out int cMailboxes, out SafeExMemoryHandle rgGuidMailboxes);

		[PreserveSig]
		int HrCiPurgeMailboxes([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, uint ulState);

		[PreserveSig]
		int HrCiSetCiEnabled([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.Bool)] bool fIsEnabled);

		[PreserveSig]
		int HrCiSetIndexedPtags([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int cptags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] int[] rgptags);

		[PreserveSig]
		int HrCiGetDocumentIdFromEntryId([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryID, out uint ulDocumentId);

		[PreserveSig]
		int HrDoMaintenanceTask([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int ulTask);

		[PreserveSig]
		int HrExecuteTask([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidTaskClass, int taskId);

		[PreserveSig]
		unsafe int EcReadMdbEvents([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdbVer, [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbRequest, out int cbResponse, _SBinaryArray* pbResponse);

		[PreserveSig]
		unsafe int EcWriteMdbEvents([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdbVer, [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte* pbRequest, out int cbResponse, _SBinaryArray* pbResponse);

		[PreserveSig]
		int HrTriggerPFSync([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] pEntryId, int ulFlags);

		[PreserveSig]
		int HrSetPFReplicas([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] rgszDN, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] rgulAgeLimit, int ulSize);

		[PreserveSig]
		int HrCiGetCatalogState([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, out short catalogState, out ulong propertyBlobSize, out SafeExMemoryHandle propertyBlob);

		[PreserveSig]
		int HrCiSetCatalogState([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, short catalogState, [In] uint cbPropertyBlob, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] byte[] propertyBlob);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		int HrCiGetFailedItems([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] ulong itemNumber, [PointerType("SRowSet *")] out SafeExLinkedMemoryHandle lpSRowset);

		[Obsolete("This feature is gone.", true)]
		[PreserveSig]
		unsafe int HrCiGetDocumentStatesFromEntryIDs([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgGuidMailboxList, _SBinaryArray* pEntryIdList, out short* rgsStates);

		[PreserveSig]
		int HrIntegCheck([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] ref IntegrityTestResult pTestParam);

		[PreserveSig]
		int HrIntegGetProgress([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, long iTest, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult);

		[PreserveSig]
		int HrIntegGetCancel([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult);

		[PreserveSig]
		int HrIntegGetDone([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[PreserveSig]
		int Slot1();

		[PreserveSig]
		int HrCiUpdateRetryTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, int cDocumentIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulDocumentIds, [MarshalAs(UnmanagedType.LPArray)] Guid[] rgMailboxGuids, [MarshalAs(UnmanagedType.LPArray)] int[] rgHresults, [MarshalAs(UnmanagedType.LPArray)] short[] initialStates);

		[PreserveSig]
		int HrCiEnumerateRetryTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, out int cDocumentIds, [PointerType("uint*")] out SafeExMemoryHandle rgulDocumentIds, [PointerType("Guid*")] out SafeExMemoryHandle rgMailboxGuids, [PointerType("short*")] out SafeExMemoryHandle rgStates);

		[PreserveSig]
		int HrCiEntryIdFromDocumentId([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, uint ulDocumentId, out int cbEntryId, [PointerType("byte*")] out SafeExLinkedMemoryHandle pEntryId);

		[PreserveSig]
		int HrGetPublicFolderDN(int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPWStr)] string folderName, out SafeExMemoryHandle lppszDN);

		[PreserveSig]
		int HrCiSeedPropertyStore([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidSourceInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidDestinationInstance);

		[PreserveSig]
		int HrLogReplayRequest([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] uint ulgenLogReplayMax, out uint ulgenLogReplayNext, out uint pCbOut, out SafeExMemoryHandle pDbinfo);

		[PreserveSig]
		int HrStartBlockModeReplicationToPassive([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPWStr)] [In] string passiveName, [In] uint ulFirstGenToSend);

		[PreserveSig]
		int HrGetRestrictionTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrGetViewsTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrGetDatabaseSize([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out ulong dbTotalPages, out ulong dbAvailablePages, out ulong dbPageSize);

		[PreserveSig]
		int HrCiUpdateFailedItem([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint documentId, [In] uint errorCode, [In] uint flags);

		[PreserveSig]
		int HrCiEnumerateFailedItems([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] uint lastMaxDocId, [PointerType("SRowSet *")] out SafeExLinkedMemoryHandle lpSRowset);

		[PreserveSig]
		int HrPrePopulateCache([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStr)] [In] string legacyDN, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] int cbPartitionHint, [MarshalAs(UnmanagedType.LPArray)] byte[] pbPartitionHint, [MarshalAs(UnmanagedType.LPStr)] [In] string dcName);

		[PreserveSig]
		int HrGetMailboxTableEntry([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrGetMailboxTableEntryFlags([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint flags, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrCiEnumerateFailedItemsByMaillbox([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint lastMaxDocId, [PointerType("SRowSet *")] out SafeExLinkedMemoryHandle lpSRowset);

		[PreserveSig]
		int HrCiUpdateFailedItemAndRetryTableByErrorCode([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] uint errorCode, [In] uint lastMaxDocId, out uint curMaxDocId, out uint itemNumber);

		[PreserveSig]
		int HrGetPublicFolderGlobalsTable([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowset);

		[PreserveSig]
		int HrCiGetTableSize([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] short tableId, [In] ulong ulFlags, out ulong pulSize);

		[PreserveSig]
		int HrMultiMailboxSearch([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, ulong cbSearchRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSearchRequest, out ulong cbSearchResponse, out SafeExMemoryHandle pSearchResponse);

		[PreserveSig]
		int HrGetMultiMailboxSearchKeywordStats([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, ulong cbSearchRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pSearchRequest, out ulong cbSearchResponse, out SafeExMemoryHandle pSearchResponse);

		[PreserveSig]
		int HrGetDatabaseProcessInfo([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTags, [PointerType("SRowSet*")] out SafeExProwsHandle lpSRowSet);

		[PreserveSig]
		int HrProcessSnapshotOperation([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pGuidMdb, [In] uint opCode, [In] uint flags);

		[PreserveSig]
		int HrGetPhysicalDatabaseInformation([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pGuidMdb, out uint pCbOut, out SafeExMemoryHandle pDbinfo);

		[PreserveSig]
		unsafe int HrFormatSearchRestriction([In] SRestriction* pRestriction, out long cbFormatted, out SafeExMemoryHandle pbFormatted);
	}
}
