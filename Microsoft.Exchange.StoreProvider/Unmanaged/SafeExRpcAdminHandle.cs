using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExRpcAdminHandle : SafeExInterfaceHandle
	{
		protected SafeExRpcAdminHandle()
		{
		}

		internal SafeExRpcAdminHandle(IntPtr handle) : base(handle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExRpcAdminHandle>(this);
		}

		internal int HrGetServerVersion(out short pwMajor, out short pwMinor)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetServerVersion(this.handle, out pwMajor, out pwMinor);
		}

		internal int HrGetMailboxInfoSize(out int cbInfo)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxInfoSize(this.handle, out cbInfo);
		}

		internal int HrDeletePrivateMailbox(Guid pguidMdb, Guid pguidMailbox, int ulFlags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrDeletePrivateMailbox(this.handle, pguidMdb, pguidMailbox, ulFlags);
		}

		internal int HrGetMailboxBasicInfo(Guid pguidMdb, Guid pguidMailbox, byte[] rgbInfo, int cbInfo)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxBasicInfo(this.handle, pguidMdb, pguidMailbox, rgbInfo, cbInfo);
		}

		internal int HrSetMailboxBasicInfo(Guid pguidMdb, Guid pguidMailbox, byte[] rgbInfo, int cbInfo)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSetMailboxBasicInfo(this.handle, pguidMdb, pguidMailbox, rgbInfo, cbInfo);
		}

		internal int HrNotifyOnDSChange(Guid pguidMdb, Guid pguidMailbox, int ulObject)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrNotifyOnDSChange(this.handle, pguidMdb, pguidMailbox, ulObject);
		}

		internal int HrListMdbStatus(int cMdb, Guid[] rgguidMdb, uint[] rgulMdbStatus)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrListMdbStatus(this.handle, cMdb, rgguidMdb, rgulMdbStatus);
		}

		internal unsafe int HrReadMapiEvents(Guid pguidMdb, ref Guid pguidMdbVer, long llStartEvent, int cEventWanted, int cEventsToCheck, SRestriction* pFilter, int ulFlags, out int pcEventActual, out SafeExLinkedMemoryHandle pEvents, out long pllEndCounter)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrReadMapiEvents(this.handle, pguidMdb, ref pguidMdbVer, llStartEvent, cEventWanted, cEventsToCheck, pFilter, ulFlags, out pcEventActual, out pEvents, out pllEndCounter);
		}

		internal int HrReadLastMapiEvent(Guid pguidMdb, ref Guid pguidMdbVer, out SafeExLinkedMemoryHandle pEvent)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrReadLastMapiEvent(this.handle, pguidMdb, ref pguidMdbVer, out pEvent);
		}

		internal int HrSaveWatermarks(Guid pguidMdb, ref Guid pguidMdbVer, int cWM, IntPtr pWMs)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSaveWatermarks(this.handle, pguidMdb, ref pguidMdbVer, cWM, pWMs);
		}

		internal int HrGetWatermarks(Guid pguidMdb, ref Guid pguidMdbVer, Guid pguidConsumer, Guid pguidMailbox, out int pcWM, out SafeExMemoryHandle pWMs)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetWatermarks(this.handle, pguidMdb, ref pguidMdbVer, pguidConsumer, pguidMailbox, out pcWM, out pWMs);
		}

		internal int HrGetWatermarksForMailbox(Guid pguidMdb, ref Guid pguidMdbVer, Guid pguidMailboxDS, out int pcWMs, out SafeExMemoryHandle pWMs)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetWatermarksForMailbox(this.handle, pguidMdb, ref pguidMdbVer, pguidMailboxDS, out pcWMs, out pWMs);
		}

		internal int HrDeleteWatermarksForMailbox(Guid pguidMdb, ref Guid pguidMdbVer, Guid pguidMailboxDS, out int pcDel)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrDeleteWatermarksForMailbox(this.handle, pguidMdb, ref pguidMdbVer, pguidMailboxDS, out pcDel);
		}

		internal int HrDeleteWatermarksForConsumer(Guid pguidMdb, ref Guid pguidMdbVer, Guid pguidMailboxDS, Guid pguidConsumer, out int pcDel)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrDeleteWatermarksForConsumer(this.handle, pguidMdb, ref pguidMdbVer, pguidMailboxDS, pguidConsumer, out pcDel);
		}

		internal int HrGetMailboxTable(Guid pguidMdb, uint flags, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxTable(this.handle, pguidMdb, flags, lpPropTagArray, out lpSRowset);
		}

		internal int HrGetLogonTable(Guid pguidMdb, PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetLogonTable(this.handle, pguidMdb, lpPropTagArray, ulFlags, out lpSRowset);
		}

		internal int HrGetMailboxSecurityDescriptor(Guid pguidMdb, Guid pguidMailbox, out SafeExMemoryHandle ppntsd, out int pcntsd)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxSecurityDescriptor(this.handle, pguidMdb, pguidMailbox, out ppntsd, out pcntsd);
		}

		internal int HrSetMailboxSecurityDescriptor(Guid pguidMdb, Guid pguidMailbox, byte[] pntsd, int cntsd)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSetMailboxSecurityDescriptor(this.handle, pguidMdb, pguidMailbox, pntsd, cntsd);
		}

		internal int HrMountDatabase(Guid pguidStorageGroup, Guid pguidMdb, int ulFlags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrMountDatabase(this.handle, pguidStorageGroup, pguidMdb, ulFlags);
		}

		internal int HrUnmountDatabase(Guid pguidStorageGroup, Guid pguidMdb, int grfFlags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrUnmountDatabase(this.handle, pguidStorageGroup, pguidMdb, grfFlags);
		}

		internal int HrFlushCache(out int pcMDBs, out SafeExMemoryHandle pCheckpointStatus)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrFlushCache(this.handle, out pcMDBs, out pCheckpointStatus);
		}

		internal int HrGetLastBackupTimes(Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetLastBackupTimes(this.handle, pguidMdb, out ftLastCompleteBackupTime, out ftLastIncrementalBackupTime);
		}

		internal int HrGetLastBackupInfo(Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime, out long ftLastDifferentialBackup, out long ftLastCopyBackup, out int SnapFull, out int SnapIncremental, out int SnapDifferential, out int SnapCopy)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetLastBackupInfo(this.handle, pguidMdb, out ftLastCompleteBackupTime, out ftLastIncrementalBackupTime, out ftLastDifferentialBackup, out ftLastCopyBackup, out SnapFull, out SnapIncremental, out SnapDifferential, out SnapCopy);
		}

		internal int HrPurgeCachedMailboxObject(Guid pguidMailbox)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrPurgeCachedMailboxObject(this.handle, pguidMailbox);
		}

		internal int HrPurgeCachedMdbObject(Guid pguidMdb)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrPurgeCachedMdbObject(this.handle, pguidMdb);
		}

		internal int HrClearAbsentInDsFlagOnMailbox(Guid pguidMdb, Guid pguidMailbox)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrClearAbsentInDsFlagOnMailbox(this.handle, pguidMdb, pguidMailbox);
		}

		internal int HrSyncMailboxesWithDS(Guid pguidMdb)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSyncMailboxesWithDS(this.handle, pguidMdb);
		}

		internal int HrHasLocalReplicas(Guid pguidMdb, out bool fHasReplicas)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrHasLocalReplicas(this.handle, pguidMdb, out fHasReplicas);
		}

		internal int HrListAllMdbStatus(bool fBasicInformation, out int pcMdbs, out SafeExMemoryHandle pMdbStatus)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrListAllMdbStatus(this.handle, fBasicInformation, out pcMdbs, out pMdbStatus);
		}

		internal int HrGetReplicaInformationTable(Guid pguidMdb, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetReplicaInformationTable(this.handle, pguidMdb, lpPropTagArray, out lpSRowset);
		}

		internal int HrSyncMailboxWithDS(Guid pguidMdb, Guid pguidMailbox)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSyncMailboxWithDS(this.handle, pguidMdb, pguidMailbox);
		}

		internal int HrCiCreatePropertyStore(Guid pguidMdb, Guid pguidInstance)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiCreatePropertyStore(this.handle, pguidMdb, pguidInstance);
		}

		internal int HrCiDeletePropertyStore(Guid pguidMdb, Guid pguidInstance)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiDeletePropertyStore(this.handle, pguidMdb, pguidInstance);
		}

		internal int HrCiDeleteMailboxMapping(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiDeleteMailboxMapping(this.handle, pguidMdb, pguidInstance, pguidMailbox);
		}

		internal int HrCiMoveDocument(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, int cbEntryIdSource, byte[] pEntryIdSource, int cbEntryIdTarget, byte[] pEntryIdTarget)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiMoveDocument(this.handle, pguidMdb, pguidInstance, pguidMailbox, cbEntryIdSource, pEntryIdSource, cbEntryIdTarget, pEntryIdTarget);
		}

		internal int HrCiGetWaterMark(Guid pguidMdb, Guid pguidInstance, bool fIsHighWatermark, out ulong ullWatermark, out System.Runtime.InteropServices.ComTypes.FILETIME lastAccessTime)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiGetWaterMark(this.handle, pguidMdb, pguidInstance, fIsHighWatermark, out ullWatermark, out lastAccessTime);
		}

		internal int HrCiSetWaterMark(Guid pguidMdb, Guid pguidInstance, bool fIsHighWatermark, ulong ullWatermark)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetWaterMark(this.handle, pguidMdb, pguidInstance, fIsHighWatermark, ullWatermark);
		}

		internal int HrCiGetMailboxState(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, out System.Runtime.InteropServices.ComTypes.FILETIME ftStart, out uint ulState, out ulong ullEventCounter)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiGetMailboxState(this.handle, pguidMdb, pguidInstance, pguidMailbox, out ftStart, out ulState, out ullEventCounter);
		}

		internal int HrCiSetMailboxStateAndStartDate(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, ref System.Runtime.InteropServices.ComTypes.FILETIME pftStart, uint ulState)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetMailboxStateAndStartDate(this.handle, pguidMdb, pguidInstance, pguidMailbox, ref pftStart, ulState);
		}

		internal int HrCiSetMailboxState(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, uint ulState)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetMailboxState(this.handle, pguidMdb, pguidInstance, pguidMailbox, ulState);
		}

		internal int HrCiSetMailboxEventCounter(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, ulong ullEventCounter)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetMailboxEventCounter(this.handle, pguidMdb, pguidInstance, pguidMailbox, ullEventCounter);
		}

		internal int HrCiEnumerateMailboxesByState(Guid pguidMdb, Guid pguidInstance, uint ulState, out int cMailboxes, out SafeExMemoryHandle rgGuidMailboxes)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiEnumerateMailboxesByState(this.handle, pguidMdb, pguidInstance, ulState, out cMailboxes, out rgGuidMailboxes);
		}

		internal int HrCiPurgeMailboxes(Guid pguidMdb, Guid pguidInstance, uint ulState)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiPurgeMailboxes(this.handle, pguidMdb, pguidInstance, ulState);
		}

		internal int HrCiSetCiEnabled(Guid pguidMdb, bool fIsEnabled)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetCiEnabled(this.handle, pguidMdb, fIsEnabled);
		}

		internal int HrCiSetIndexedPtags(Guid pguidMdb, int cptags, int[] rgptags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetIndexedPtags(this.handle, pguidMdb, cptags, rgptags);
		}

		internal int HrCiGetDocumentIdFromEntryId(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, int cbEntryId, byte[] pEntryID, out uint ulDocumentId)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiGetDocumentIdFromEntryId(this.handle, pguidMdb, pguidInstance, pguidMailbox, cbEntryId, pEntryID, out ulDocumentId);
		}

		internal int HrDoMaintenanceTask(Guid pguidMdb, int ulTask)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrDoMaintenanceTask(this.handle, pguidMdb, ulTask);
		}

		internal int HrExecuteTask(Guid pguidMdb, Guid pguidTaskClass, int taskId)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrExecuteTask(this.handle, pguidMdb, pguidTaskClass, taskId);
		}

		internal unsafe int EcReadMdbEvents(Guid pguidMdb, Guid pguidMdbVer, int cbRequest, byte[] pbRequest, out int cbResponse, _SBinaryArray* pbResponse)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_EcReadMdbEvents(this.handle, pguidMdb, pguidMdbVer, cbRequest, pbRequest, out cbResponse, pbResponse);
		}

		internal unsafe int EcWriteMdbEvents(Guid pguidMdb, Guid pguidMdbVer, int cbRequest, byte[] pbRequest, out int cbResponse, _SBinaryArray* pbResponse)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_EcWriteMdbEvents(this.handle, pguidMdb, pguidMdbVer, cbRequest, pbRequest, out cbResponse, pbResponse);
		}

		internal int HrTriggerPFSync(Guid pguidMdb, int cbEntryId, byte[] pEntryId, int ulFlags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrTriggerPFSync(this.handle, pguidMdb, cbEntryId, pEntryId, ulFlags);
		}

		internal int HrSetPFReplicas(Guid pguidMdb, string[] rgszDN, int[] rgulAgeLimit, int ulSize)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrSetPFReplicas(this.handle, pguidMdb, rgszDN, rgulAgeLimit, ulSize);
		}

		internal int HrCiGetCatalogState(Guid pguidMdb, Guid pguidInstance, out short catalogState, out ulong propertyBlobSize, out SafeExMemoryHandle propertyBlob)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiGetCatalogState(this.handle, pguidMdb, pguidInstance, out catalogState, out propertyBlobSize, out propertyBlob);
		}

		internal int HrCiSetCatalogState(Guid pguidMdb, Guid pguidInstance, short catalogState, uint cbPropertyBlob, byte[] propertyBlob)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSetCatalogState(this.handle, pguidMdb, pguidInstance, catalogState, cbPropertyBlob, propertyBlob);
		}

		internal int HrIntegCheck(Guid pguidMdb, ref IntegrityTestResult pTestParam)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrIntegCheck(this.handle, pguidMdb, ref pTestParam);
		}

		internal int HrIntegGetProgress(Guid pguidMdb, long iTest, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrIntegGetProgress(this.handle, pguidMdb, iTest, out pcTotalTest, out piCurrentTest, out pcCurrentPercent, out pTestResult);
		}

		internal int HrIntegGetCancel(Guid pguidMdb, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrIntegGetCancel(this.handle, pguidMdb, out pcTotalTest, out piCurrentTest, out pcCurrentPercent, out pTestResult);
		}

		internal int HrIntegGetDone(Guid pguidMdb)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrIntegGetDone(this.handle, pguidMdb);
		}

		internal int HrCiUpdateRetryTable(Guid pguidMdb, Guid pguidInstance, int cDocumentIds, uint[] rgulDocumentIds, Guid[] rgMailboxGuids, int[] rgHresults, short[] initialStates)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiUpdateRetryTable(this.handle, pguidMdb, pguidInstance, cDocumentIds, rgulDocumentIds, rgMailboxGuids, rgHresults, initialStates);
		}

		internal int HrCiEnumerateRetryTable(Guid pguidMdb, Guid pguidInstance, out int cDocumentIds, out SafeExMemoryHandle rgulDocumentIds, out SafeExMemoryHandle rgMailboxGuids, out SafeExMemoryHandle rgStates)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiEnumerateRetryTable(this.handle, pguidMdb, pguidInstance, out cDocumentIds, out rgulDocumentIds, out rgMailboxGuids, out rgStates);
		}

		internal int HrCiEntryIdFromDocumentId(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, uint ulDocumentId, out int cbEntryId, out SafeExLinkedMemoryHandle pEntryId)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiEntryIdFromDocumentId(this.handle, pguidMdb, pguidInstance, pguidMailbox, ulDocumentId, out cbEntryId, out pEntryId);
		}

		internal int HrGetPublicFolderDN(int cbEntryId, byte[] pEntryId, string folderName, out SafeExMemoryHandle lppszDN)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetPublicFolderDN(this.handle, cbEntryId, pEntryId, folderName, out lppszDN);
		}

		internal int HrCiSeedPropertyStore(Guid pguidMdb, Guid pguidSourceInstance, Guid pguidDestinationInstance)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiSeedPropertyStore(this.handle, pguidMdb, pguidSourceInstance, pguidDestinationInstance);
		}

		internal int HrCiGetTableSize(Guid pguidMdb, Guid pguidInstance, short tableId, ulong flags, out ulong size)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiGetTableSize(this.handle, pguidMdb, pguidInstance, tableId, flags, out size);
		}

		internal int HrLogReplayRequest(Guid pguidMdb, uint ulgenLogReplayMax, uint ulLogReplayFlags, out uint ulgenLogReplayNext, out uint pCbOut, out SafeExMemoryHandle pDbinfo, out uint patchPageNumber, out uint cbPatchToken, out SafeExMemoryHandle ppvPatchToken, out uint cbPatchData, out SafeExMemoryHandle ppvPatchData, out uint cpgnoCorrupt, out SafeExMemoryHandle ppgnoCorrupt)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrLogReplayRequest(this.handle, pguidMdb, ulgenLogReplayMax, ulLogReplayFlags, out ulgenLogReplayNext, out pCbOut, out pDbinfo, out patchPageNumber, out cbPatchToken, out ppvPatchToken, out cbPatchData, out ppvPatchData, out cpgnoCorrupt, out ppgnoCorrupt);
		}

		internal int HrStartBlockModeReplicationToPassive(Guid pguidMdb, string passiveName, uint ulFirstGenToSend)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrStartBlockModeReplicationToPassive(this.handle, pguidMdb, passiveName, ulFirstGenToSend);
		}

		internal int HrStartSendingGranularLogData(Guid pguidMdb, string pipeName, uint flags, uint maxIoDepth, uint maxIoLatency)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrStartSendingGranularLogData(this.handle, pguidMdb, pipeName, flags, maxIoDepth, maxIoLatency);
		}

		internal int HrStopSendingGranularLogData(Guid pguidMdb, uint flags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrStopSendingGranularLogData(this.handle, pguidMdb, flags);
		}

		internal int HrGetRestrictionTable(Guid pguidMdb, Guid pguidMailbox, int cbEntryId, byte[] pEntryId, PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetRestrictionTable(this.handle, pguidMdb, pguidMailbox, cbEntryId, pEntryId, lpPropTagArray, ulFlags, out lpSRowset);
		}

		internal int HrGetViewsTable(Guid pguidMdb, Guid pguidMailbox, int cbEntryId, byte[] pEntryId, PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetViewsTable(this.handle, pguidMdb, pguidMailbox, cbEntryId, pEntryId, lpPropTagArray, ulFlags, out lpSRowset);
		}

		internal int HrGetDatabaseSize(Guid pguidMdb, out ulong dbTotalPages, out ulong dbAvailablePages, out ulong dbPageSize)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetDatabaseSize(this.handle, pguidMdb, out dbTotalPages, out dbAvailablePages, out dbPageSize);
		}

		internal int HrCiUpdateFailedItem(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, uint documentId, uint errorCode, uint flags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiUpdateFailedItem(this.handle, pguidMdb, pguidInstance, pguidMailbox, documentId, errorCode, flags);
		}

		internal int HrCiEnumerateFailedItems(Guid pguidMdb, Guid pguidInstance, uint lastMaxDocId, out SafeExLinkedMemoryHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiEnumerateFailedItems(this.handle, pguidMdb, pguidInstance, lastMaxDocId, out lpSRowset);
		}

		internal int HrPrePopulateCache(Guid pguidMdb, string legacyDN, Guid pguidMailbox, byte[] partitionHint, string dcName)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrPrePopulateCache(this.handle, pguidMdb, legacyDN, pguidMailbox, (partitionHint != null) ? partitionHint.Length : 0, partitionHint, dcName);
		}

		internal int HrGetMailboxTableEntry(Guid pguidMdb, Guid pguidMailbox, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxTableEntry(this.handle, pguidMdb, pguidMailbox, lpPropTagArray, out lpSRowset);
		}

		internal int HrGetMailboxTableEntryFlags(Guid pguidMdb, Guid pguidMailbox, uint flags, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxTableEntryFlags(this.handle, pguidMdb, pguidMailbox, flags, lpPropTagArray, out lpSRowset);
		}

		internal int HrCiEnumerateFailedItemsByMailbox(Guid pguidMdb, Guid pguidInstance, Guid pguidMailbox, uint lastMaxDocId, out SafeExLinkedMemoryHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiEnumerateFailedItemsByMailbox(this.handle, pguidMdb, pguidInstance, pguidMailbox, lastMaxDocId, out lpSRowset);
		}

		internal int HrCiUpdateFailedItemAndRetryTableByErrorCode(Guid pguidMdb, Guid pguidInstance, uint errorCode, uint lastMaxDocId, out uint curMaxDocId, out uint itemNumber)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrCiUpdateFailedItemAndRetryTableByErrorCode(this.handle, pguidMdb, pguidInstance, errorCode, lastMaxDocId, out curMaxDocId, out itemNumber);
		}

		internal int HrGetResourceMonitorDigest(Guid pguidMdb, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetResourceMonitorDigest(this.handle, pguidMdb, lpPropTagArray, out lpSRowset);
		}

		internal int HrGetMailboxSignatureBasicInfo(Guid pguidMdb, Guid pguidMailbox, uint ulFlags, out int cbMailboxBasicInfo, out SafeExMemoryHandle ppbMailboxBasicInfo)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMailboxSignatureBasicInfo(this.handle, pguidMdb, pguidMailbox, ulFlags, out cbMailboxBasicInfo, out ppbMailboxBasicInfo);
		}

		internal int HrGetFeatureVersion(uint versionedFeature, out uint featureVersion)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetFeatureVersion(this.handle, versionedFeature, out featureVersion);
		}

		internal int HrISIntegCheck(Guid pguidMdb, Guid pguidMailbox, uint ulFlags, int dbtasks, uint[] dbtaskids, out SafeExMemoryHandle taskId)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrISIntegCheck(this.handle, pguidMdb, pguidMailbox, ulFlags, dbtasks, dbtaskids, out taskId);
		}

		internal int HrForceNewLog(Guid pguidMdb)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrForceNewLog(this.handle, pguidMdb);
		}

		internal int HrGetPublicFolderGlobalsTable(Guid pguidMdb, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetPublicFolderGlobalsTable(this.handle, pguidMdb, lpPropTagArray, out lpSRowset);
		}

		internal int HrMultiMailboxSearch(Guid pguidMdb, ulong cbRequest, byte[] pbRequest, out ulong cbResponse, out SafeExMemoryHandle pSearchResponse)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrMultiMailboxSearch(this.handle, pguidMdb, cbRequest, pbRequest, out cbResponse, out pSearchResponse);
		}

		internal int HrGetMultiMailboxSearchKeywordStats(Guid pguidMdb, ulong cbRequest, byte[] pbRequest, out ulong cbResponse, out SafeExMemoryHandle pSearchResponse)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetMultiMailboxSearchKeywordStats(this.handle, pguidMdb, cbRequest, pbRequest, out cbResponse, out pSearchResponse);
		}

		internal unsafe int HrFormatSearchRestriction(SRestriction* restriction, out ulong size, out SafeExMemoryHandle lpRestriction)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrFormatSearchRestriction(this.handle, restriction, out size, out lpRestriction);
		}

		internal uint GetStorePerRPCStats(out PerRpcStats pPerRpcPerformanceStatistics)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_GetStorePerRPCStats(this.handle, out pPerRpcPerformanceStatistics);
		}

		internal int HrGetDatabaseProcessInfo(Guid pMdbGuid, PropTag[] lpPropTags, out SafeExProwsHandle lpSRowSet)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetDatabaseProcessInfo(this.handle, pMdbGuid, lpPropTags, out lpSRowSet);
		}

		internal int HrProcessSnapshotOperation(Guid pGuidMdb, uint opCode, uint flags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrProcessSnapshotOperation(this.handle, pGuidMdb, opCode, flags);
		}

		internal int HrGetPhysicalDatabaseInformation(Guid pGuidMdb, out uint pCbOut, out SafeExMemoryHandle pDbinfo)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrGetPhysicalDatabaseInformation(this.handle, pGuidMdb, out pCbOut, out pDbinfo);
		}

		internal int HrStoreIntegrityCheckEx(Guid pguidMdb, Guid pguidMailbox, Guid pguidRequest, uint ulOperation, uint ulFlags, uint[] dbtaskIds, PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrStoreIntegrityCheckEx(this.handle, pguidMdb, pguidMailbox, pguidRequest, ulOperation, ulFlags, (uint)((dbtaskIds == null) ? 0 : dbtaskIds.Length), dbtaskIds, lpPropTagArray, out lpSRowset);
		}

		internal unsafe int HrCreateUserInfo(Guid pguidMdb, Guid pguidUserInfo, uint ulFlags, PropValue[] properties)
		{
			int num = 0;
			foreach (PropValue propValue in properties)
			{
				num += propValue.GetBytesToMarshal();
			}
			fixed (byte* ptr = new byte[num])
			{
				PropValue.MarshalToNative(properties, ptr);
				return SafeExRpcAdminHandle.IExRpcAdmin_HrCreateUserInfo(this.handle, pguidMdb, pguidUserInfo, ulFlags, properties.Length, (SPropValue*)ptr);
			}
		}

		internal int HrReadUserInfo(Guid pguidMdb, Guid pguidUserInfo, uint ulFlags, PropTag[] lpPropTags, out SafeExProwsHandle lpSRowset)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrReadUserInfo(this.handle, pguidMdb, pguidUserInfo, ulFlags, lpPropTags, out lpSRowset);
		}

		internal unsafe int HrUpdateUserInfo(Guid pguidMdb, Guid pguidUserInfo, uint ulFlags, PropValue[] properties, PropTag[] lpDeletePropTags)
		{
			int num = 0;
			foreach (PropValue propValue in properties)
			{
				num += propValue.GetBytesToMarshal();
			}
			fixed (byte* ptr = new byte[num])
			{
				PropValue.MarshalToNative(properties, ptr);
				return SafeExRpcAdminHandle.IExRpcAdmin_HrUpdateUserInfo(this.handle, pguidMdb, pguidUserInfo, ulFlags, properties.Length, (SPropValue*)ptr, lpDeletePropTags);
			}
		}

		internal int HrDeleteUserInfo(Guid pguidMdb, Guid pguidUserInfo, uint ulFlags)
		{
			return SafeExRpcAdminHandle.IExRpcAdmin_HrDeleteUserInfo(this.handle, pguidMdb, pguidUserInfo, ulFlags);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetServerVersion(IntPtr iExRpcAdmin, out short pwMajor, out short pwMinor);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxInfoSize(IntPtr iExRpcAdmin, out int cbInfo);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrDeletePrivateMailbox(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxBasicInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] rgbInfo, int cbInfo);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSetMailboxBasicInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] rgbInfo, int cbInfo);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrNotifyOnDSChange(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int ulObject);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrListMdbStatus(IntPtr iExRpcAdmin, int cMdb, [MarshalAs(UnmanagedType.LPArray)] [In] Guid[] rgguidMdb, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] rgulMdbStatus);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_HrReadMapiEvents(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, long llStartEvent, int cEventWanted, int cEventsToCheck, [In] SRestriction* pFilter, int ulFlags, out int pcEventActual, out SafeExLinkedMemoryHandle pEvents, out long pllEndCounter);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrReadLastMapiEvent(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, out SafeExLinkedMemoryHandle pEvent);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSaveWatermarks(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, int cWM, IntPtr pWMs);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetWatermarks(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidConsumer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out int pcWM, out SafeExMemoryHandle pWMs);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetWatermarksForMailbox(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, out int pcWMs, out SafeExMemoryHandle pWMs);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrDeleteWatermarksForMailbox(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, out int pcDel);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrDeleteWatermarksForConsumer(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] [Out] ref Guid pguidMdbVer, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailboxDS, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidConsumer, out int pcDel);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] uint flags, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetLogonTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxSecurityDescriptor(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out SafeExMemoryHandle ppntsd, out int pcntsd);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSetMailboxSecurityDescriptor(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pntsd, int cntsd);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrMountDatabase(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidStorageGroup, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrUnmountDatabase(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidStorageGroup, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int grfFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrFlushCache(IntPtr iExRpcAdmin, out int pcMDBs, out SafeExMemoryHandle pCheckpointStatus);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetLastBackupTimes(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetLastBackupInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long ftLastCompleteBackupTime, out long ftLastIncrementalBackupTime, out long ftLastDifferentialBackup, out long ftLastCopyBackup, out int SnapFull, out int SnapIncremental, out int SnapDifferential, out int SnapCopy);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrPurgeCachedMailboxObject(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrPurgeCachedMdbObject(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrClearAbsentInDsFlagOnMailbox(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSyncMailboxesWithDS(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrHasLocalReplicas(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.Bool)] out bool fHasReplicas);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrListAllMdbStatus(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.Bool)] bool fBasicInformation, out int pcMdbs, out SafeExMemoryHandle pMdbStatus);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetReplicaInformationTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSyncMailboxWithDS(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiCreatePropertyStore(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiDeletePropertyStore(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiDeleteMailboxMapping(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiMoveDocument(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryIdSource, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryIdSource, int cbEntryIdTarget, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryIdTarget);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiGetWaterMark(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.Bool)] bool fIsHighWatermark, out ulong ullWatermark, out System.Runtime.InteropServices.ComTypes.FILETIME lastAccessTime);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetWaterMark(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.Bool)] bool fIsHighWatermark, ulong ullWatermark);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiGetMailboxState(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, out System.Runtime.InteropServices.ComTypes.FILETIME ftStart, out uint ulState, out ulong ullEventCounter);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetMailboxStateAndStartDate(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] ref System.Runtime.InteropServices.ComTypes.FILETIME pftStart, uint ulState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetMailboxState(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, uint ulState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetMailboxEventCounter(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, ulong ullEventCounter);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiEnumerateMailboxesByState(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, uint ulState, out int cMailboxes, out SafeExMemoryHandle rgGuidMailboxes);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiPurgeMailboxes(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, uint ulState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetCiEnabled(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.Bool)] bool fIsEnabled);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetIndexedPtags(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int cptags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] int[] rgptags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiGetDocumentIdFromEntryId(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryID, out uint ulDocumentId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrDoMaintenanceTask(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, int ulTask);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrExecuteTask(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidTaskClass, int ulTaskId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_EcReadMdbEvents(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdbVer, [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbRequest, out int cbResponse, _SBinaryArray* pbResponse);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_EcWriteMdbEvents(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdbVer, [In] int cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbRequest, out int cbResponse, _SBinaryArray* pbResponse);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrTriggerPFSync(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] int cbEntryId, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] pEntryId, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrSetPFReplicas(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] rgszDN, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] int[] rgulAgeLimit, int ulSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiGetCatalogState(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, out short catalogState, out ulong propertyBlobSize, out SafeExMemoryHandle propertyBlob);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSetCatalogState(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, short catalogState, [In] uint cbPropertyBlob, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] byte[] propertyBlob);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrIntegCheck(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] ref IntegrityTestResult pTestParam);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrIntegGetProgress(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, long iTest, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrIntegGetCancel(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out long pcTotalTest, out long piCurrentTest, out long pcCurrentPercent, out IntegrityTestResult pTestResult);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrIntegGetDone(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiUpdateRetryTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, int cDocumentIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulDocumentIds, [MarshalAs(UnmanagedType.LPArray)] Guid[] rgMailboxGuids, [MarshalAs(UnmanagedType.LPArray)] int[] rgHresults, [MarshalAs(UnmanagedType.LPArray)] short[] initialStates);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiEnumerateRetryTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, out int cDocumentIds, out SafeExMemoryHandle rgulDocumentIds, out SafeExMemoryHandle rgMailboxGuids, out SafeExMemoryHandle rgStates);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiEntryIdFromDocumentId(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, uint ulDocumentId, out int cbEntryId, out SafeExLinkedMemoryHandle pEntryId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetPublicFolderDN(IntPtr iExRpcAdmin, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPWStr)] string folderName, out SafeExMemoryHandle lppszDN);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiSeedPropertyStore(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidSourceInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidDestinationInstance);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrLogReplayRequest(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] uint ulgenLogReplayMax, [In] uint ulLogReplayFlags, out uint ulgenLogReplayNext, out uint pCbOut, out SafeExMemoryHandle pDbinfo, out uint ppatchPageNumber, out uint pcbPatchToken, out SafeExMemoryHandle ppvPatchToken, out uint pcbPatchData, out SafeExMemoryHandle ppvPatchData, out uint pcpgnoCorrupt, out SafeExMemoryHandle ppgnoCorrupt);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrStartBlockModeReplicationToPassive(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPWStr)] [In] string passiveName, [In] uint ulFirstGenToSend);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrStartSendingGranularLogData(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPWStr)] [In] string pipeName, [In] uint flags, [In] uint maxIoDepth, [In] uint maxIoLatency);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrStopSendingGranularLogData(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] uint flags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetRestrictionTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetViewsTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, int cbEntryId, [MarshalAs(UnmanagedType.LPArray)] byte[] pEntryId, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, int ulFlags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetDatabaseSize(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, out ulong dbTotalPages, out ulong dbAvailablePages, out ulong dbPageSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiUpdateFailedItem(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint documentId, [In] uint errorCode, [In] uint flags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiEnumerateFailedItems(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] uint lastMaxDocId, out SafeExLinkedMemoryHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrPrePopulateCache(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStr)] [In] string legacyDN, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] int cbPartitionHint, [MarshalAs(UnmanagedType.LPArray)] byte[] pbPartitionHint, [MarshalAs(UnmanagedType.LPStr)] [In] string dcName);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxTableEntry(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxTableEntryFlags(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint flags, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiEnumerateFailedItemsByMailbox(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint lastMaxDocId, out SafeExLinkedMemoryHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiUpdateFailedItemAndRetryTableByErrorCode(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] uint errorCode, [In] uint lastMaxDocId, out uint curMaxDocId, out uint itemNumber);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetResourceMonitorDigest(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMailboxSignatureBasicInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint ulFlags, out int cbMailboxBasicInfo, out SafeExMemoryHandle ppbMailboxBasicInfo);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetFeatureVersion(IntPtr iExRpcAdmin, [In] uint versionedFeature, out uint featureVersion);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrISIntegCheck(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMailbox, [In] uint Flags, [In] int cTaskIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulTaskIds, out SafeExMemoryHandle szTaskId);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrForceNewLog(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetPublicFolderGlobalsTable(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrCiGetTableSize(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidInstance, [In] short tableId, [In] ulong ulFlags, out ulong ulSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrMultiMailboxSearch(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] ulong cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbRequest, out ulong cbResponse, out SafeExMemoryHandle ppvResponse);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetMultiMailboxSearchKeywordStats(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pguidMdb, [In] ulong cbRequest, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbRequest, out ulong cbResponse, out SafeExMemoryHandle ppvResponse);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_HrFormatSearchRestriction(IntPtr iExRpcAdmin, [In] SRestriction* restricition, out ulong size, out SafeExMemoryHandle lpFormattedRestriction);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern uint IExRpcAdmin_GetStorePerRPCStats(IntPtr iExRpcAdmin, out PerRpcStats pPerRpcPerformanceStatistics);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetDatabaseProcessInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrProcessSnapshotOperation(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, uint opCode, uint flags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrGetPhysicalDatabaseInformation(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, out uint pCbOut, out SafeExMemoryHandle pDbinfo);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrStoreIntegrityCheckEx(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMailboxGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pRequestGuid, [In] uint ulOperation, [In] uint ulFlags, [In] uint cTaskIds, [MarshalAs(UnmanagedType.LPArray)] uint[] rgulTaskIds, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTagArray, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_HrCreateUserInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pUserInfoGuid, [In] uint ulFlags, int cValues, SPropValue* lpPropArray);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrReadUserInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pUserInfoGuid, [In] uint ulFlags, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpPropTags, out SafeExProwsHandle lpSRowset);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExRpcAdmin_HrUpdateUserInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pUserInfoGuid, [In] uint ulFlags, int cValues, SPropValue* lpPropArray, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpDeletePropTags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExRpcAdmin_HrDeleteUserInfo(IntPtr iExRpcAdmin, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pMdbGuid, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid pUserInfoGuid, [In] uint ulFlags);
	}
}
