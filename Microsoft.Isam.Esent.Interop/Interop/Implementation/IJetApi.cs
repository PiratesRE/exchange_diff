using System;
using Microsoft.Isam.Esent.Interop.Server2003;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows7;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Isam.Esent.Interop.Implementation
{
	internal interface IJetApi
	{
		int JetTracing(JET_traceop operation, JET_tracetag tag, bool value);

		int JetTracing(JET_traceop operation, JET_tracetag tag, JET_DBID value);

		int JetTracing(JET_traceop operation, JET_tracetag tag, int value);

		int JetTracing(JET_traceop operation, JET_tracetag tag, JET_PFNTRACEREGISTER callback);

		int JetTracing(JET_traceop operation, JET_tracetag tag, JET_PFNTRACEEMIT callback);

		int JetSetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, long longParameter);

		int JetGetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, out long paramValue);

		int JetConsumeLogData(JET_INSTANCE instance, JET_EMITDATACTX emitLogDataCtx, byte[] logDataBuf, int logDataStartOffset, int logDataLength, ShadowLogConsumeGrbit grbit);

		int JetGetLogFileInfo(string logFileName, out JET_LOGINFOMISC info, JET_LogInfo infoLevel);

		int JetGetPageInfo2(byte[] bytesPages, int bytesPagesLength, JET_PAGEINFO[] pageInfos, PageInfoGrbit grbit, JET_PageInfo infoLevel);

		int JetGetInstanceMiscInfo(JET_INSTANCE instance, out JET_CHECKPOINTINFO checkpointInfo, JET_InstanceMiscInfo infoLevel);

		int JetBeginDatabaseIncrementalReseed(JET_INSTANCE instance, string wszDatabase, BeginDatabaseIncrementalReseedGrbit grbit);

		int JetEndDatabaseIncrementalReseed(JET_INSTANCE instance, string wszDatabase, int genMinRequired, int genFirstDivergedLog, int genMaxRequired, EndDatabaseIncrementalReseedGrbit grbit);

		int JetPatchDatabasePages(JET_INSTANCE instance, string databaseFileName, int pgnoStart, int pageCount, byte[] inputData, int dataLength, PatchDatabasePagesGrbit grbit);

		int JetRemoveLogfile(string databaseFileName, string logFileName, RemoveLogfileGrbit grbit);

		int JetGetDatabasePages(JET_SESID sesid, JET_DBID dbid, int pgnoStart, int countPages, byte[] pageBytes, int pageBytesLength, out int pageBytesRead, GetDatabasePagesGrbit grbit);

		int JetDBUtilities(JET_DBUTIL dbutil);

		int JetTestHook(int opcode, ref uint pv);

		int JetTestHook(int opcode, ref NATIVE_TESTHOOKTESTINJECTION pv);

		int JetTestHook(int opcode, JET_TESTHOOKEVICTCACHE nativeTestHookEvictCachedPage);

		int JetTestHook(int opcode, ref IntPtr pv);

		int JetTestHook(int opcode, ref NATIVE_TESTHOOKTRACETESTMARKER pv);

		int JetTestHook(int opcode, ref NATIVE_TESTHOOKCORRUPT_DATABASEFILE pv);

		int JetPrereadTables(JET_SESID sesid, JET_DBID dbid, string[] rgsz, PrereadTablesGrbit grbit);

		int JetDatabaseScan(JET_SESID sesid, JET_DBID dbid, ref int seconds, int sleepInMsec, JET_CALLBACK callback, DatabaseScanGrbit grbit);

		int JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, int value);

		int JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, ref NATIVE_OPERATIONCONTEXT operationContext);

		JetCapabilities Capabilities { get; }

		int JetCreateInstance(out JET_INSTANCE instance, string name);

		int JetCreateInstance2(out JET_INSTANCE instance, string name, string displayName, CreateInstanceGrbit grbit);

		int JetInit(ref JET_INSTANCE instance);

		int JetInit2(ref JET_INSTANCE instance, InitGrbit grbit);

		int JetInit3(ref JET_INSTANCE instance, JET_RSTINFO recoveryOptions, InitGrbit grbit);

		int JetGetInstanceInfo(out int numInstances, out JET_INSTANCE_INFO[] instances);

		int JetGetInstanceMiscInfo(JET_INSTANCE instance, out JET_SIGNATURE signature, JET_InstanceMiscInfo infoLevel);

		int JetStopBackupInstance(JET_INSTANCE instance);

		int JetStopServiceInstance(JET_INSTANCE instance);

		int JetStopServiceInstance2(JET_INSTANCE instance, StopServiceGrbit grbit);

		int JetTerm(JET_INSTANCE instance);

		int JetTerm2(JET_INSTANCE instance, TermGrbit grbit);

		int JetSetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, IntPtr paramValue, string paramString);

		int JetSetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, JET_CALLBACK paramValue, string paramString);

		int JetGetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, ref IntPtr paramValue, out string paramString, int maxParam);

		int JetGetVersion(JET_SESID sesid, out uint version);

		int JetCreateDatabase(JET_SESID sesid, string database, string connect, out JET_DBID dbid, CreateDatabaseGrbit grbit);

		int JetCreateDatabase2(JET_SESID sesid, string database, int maxPages, out JET_DBID dbid, CreateDatabaseGrbit grbit);

		int JetAttachDatabase(JET_SESID sesid, string database, AttachDatabaseGrbit grbit);

		int JetAttachDatabase2(JET_SESID sesid, string database, int maxPages, AttachDatabaseGrbit grbit);

		int JetOpenDatabase(JET_SESID sesid, string database, string connect, out JET_DBID dbid, OpenDatabaseGrbit grbit);

		int JetCloseDatabase(JET_SESID sesid, JET_DBID dbid, CloseDatabaseGrbit grbit);

		int JetDetachDatabase(JET_SESID sesid, string database);

		int JetDetachDatabase2(JET_SESID sesid, string database, DetachDatabaseGrbit grbit);

		int JetCompact(JET_SESID sesid, string sourceDatabase, string destinationDatabase, JET_PFNSTATUS statusCallback, object ignored, CompactGrbit grbit);

		int JetGrowDatabase(JET_SESID sesid, JET_DBID dbid, int desiredPages, out int actualPages);

		int JetSetDatabaseSize(JET_SESID sesid, string database, int desiredPages, out int actualPages);

		int JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out int value, JET_DbInfo infoLevel);

		int JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out string value, JET_DbInfo infoLevel);

		int JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out JET_DBINFOMISC dbinfomisc, JET_DbInfo infoLevel);

		int JetGetDatabaseFileInfo(string databaseName, out int value, JET_DbInfo infoLevel);

		int JetGetDatabaseFileInfo(string databaseName, out long value, JET_DbInfo infoLevel);

		int JetGetDatabaseFileInfo(string databaseName, out JET_DBINFOMISC dbinfomisc, JET_DbInfo infoLevel);

		int JetBackupInstance(JET_INSTANCE instance, string destination, BackupGrbit grbit, JET_PFNSTATUS statusCallback);

		int JetRestoreInstance(JET_INSTANCE instance, string source, string destination, JET_PFNSTATUS statusCallback);

		int JetOSSnapshotPrepare(out JET_OSSNAPID snapid, SnapshotPrepareGrbit grbit);

		int JetOSSnapshotPrepareInstance(JET_OSSNAPID snapshot, JET_INSTANCE instance, SnapshotPrepareInstanceGrbit grbit);

		int JetOSSnapshotFreeze(JET_OSSNAPID snapshot, out int numInstances, out JET_INSTANCE_INFO[] instances, SnapshotFreezeGrbit grbit);

		int JetOSSnapshotGetFreezeInfo(JET_OSSNAPID snapshot, out int numInstances, out JET_INSTANCE_INFO[] instances, SnapshotGetFreezeInfoGrbit grbit);

		int JetOSSnapshotThaw(JET_OSSNAPID snapid, SnapshotThawGrbit grbit);

		int JetOSSnapshotTruncateLog(JET_OSSNAPID snapshot, SnapshotTruncateLogGrbit grbit);

		int JetOSSnapshotTruncateLogInstance(JET_OSSNAPID snapshot, JET_INSTANCE instance, SnapshotTruncateLogGrbit grbit);

		int JetOSSnapshotEnd(JET_OSSNAPID snapid, SnapshotEndGrbit grbit);

		int JetOSSnapshotAbort(JET_OSSNAPID snapid, SnapshotAbortGrbit grbit);

		int JetBeginExternalBackupInstance(JET_INSTANCE instance, BeginExternalBackupGrbit grbit);

		int JetCloseFileInstance(JET_INSTANCE instance, JET_HANDLE handle);

		int JetEndExternalBackupInstance(JET_INSTANCE instance);

		int JetEndExternalBackupInstance2(JET_INSTANCE instance, EndExternalBackupGrbit grbit);

		int JetGetAttachInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars);

		int JetGetLogInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars);

		int JetGetTruncateLogInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars);

		int JetOpenFileInstance(JET_INSTANCE instance, string file, out JET_HANDLE handle, out long fileSizeLow, out long fileSizeHigh);

		int JetReadFileInstance(JET_INSTANCE instance, JET_HANDLE file, byte[] buffer, int bufferSize, out int bytesRead);

		int JetTruncateLogInstance(JET_INSTANCE instance);

		int JetBeginSession(JET_INSTANCE instance, out JET_SESID sesid, string username, string password);

		int JetSetSessionContext(JET_SESID sesid, IntPtr context);

		int JetResetSessionContext(JET_SESID sesid);

		int JetEndSession(JET_SESID sesid, EndSessionGrbit grbit);

		int JetDupSession(JET_SESID sesid, out JET_SESID newSesid);

		int JetGetThreadStats(out JET_THREADSTATS threadstats);

		int JetOpenTable(JET_SESID sesid, JET_DBID dbid, string tablename, byte[] parameters, int parametersLength, OpenTableGrbit grbit, out JET_TABLEID tableid);

		int JetCloseTable(JET_SESID sesid, JET_TABLEID tableid);

		int JetDupCursor(JET_SESID sesid, JET_TABLEID tableid, out JET_TABLEID newTableid, DupCursorGrbit grbit);

		int JetComputeStats(JET_SESID sesid, JET_TABLEID tableid);

		int JetSetLS(JET_SESID sesid, JET_TABLEID tableid, JET_LS ls, LsGrbit grbit);

		int JetGetLS(JET_SESID sesid, JET_TABLEID tableid, out JET_LS ls, LsGrbit grbit);

		int JetGetCursorInfo(JET_SESID sesid, JET_TABLEID tableid);

		int JetBeginTransaction(JET_SESID sesid);

		int JetBeginTransaction2(JET_SESID sesid, BeginTransactionGrbit grbit);

		int JetBeginTransaction3(JET_SESID sesid, long userTransactionId, BeginTransactionGrbit grbit);

		int JetCommitTransaction(JET_SESID sesid, CommitTransactionGrbit grbit);

		int JetRollback(JET_SESID sesid, RollbackTransactionGrbit grbit);

		int JetCreateTable(JET_SESID sesid, JET_DBID dbid, string table, int pages, int density, out JET_TABLEID tableid);

		int JetAddColumn(JET_SESID sesid, JET_TABLEID tableid, string column, JET_COLUMNDEF columndef, byte[] defaultValue, int defaultValueSize, out JET_COLUMNID columnid);

		int JetDeleteColumn(JET_SESID sesid, JET_TABLEID tableid, string column);

		int JetDeleteColumn2(JET_SESID sesid, JET_TABLEID tableid, string column, DeleteColumnGrbit grbit);

		int JetDeleteIndex(JET_SESID sesid, JET_TABLEID tableid, string index);

		int JetDeleteTable(JET_SESID sesid, JET_DBID dbid, string table);

		int JetCreateIndex(JET_SESID sesid, JET_TABLEID tableid, string indexName, CreateIndexGrbit grbit, string keyDescription, int keyDescriptionLength, int density);

		int JetCreateIndex2(JET_SESID sesid, JET_TABLEID tableid, JET_INDEXCREATE[] indexcreates, int numIndexCreates);

		int JetOpenTempTable(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids);

		int JetOpenTempTable2(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, int lcid, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids);

		int JetOpenTempTable3(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, JET_UNICODEINDEX unicodeindex, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids);

		int JetOpenTemporaryTable(JET_SESID sesid, JET_OPENTEMPORARYTABLE temporarytable);

		int JetCreateTableColumnIndex3(JET_SESID sesid, JET_DBID dbid, JET_TABLECREATE tablecreate);

		int JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, out JET_COLUMNDEF columndef);

		int JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, out JET_COLUMNDEF columndef);

		int JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, out JET_COLUMNBASE columnbase);

		int JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, out JET_COLUMNBASE columnbase);

		int JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string ignored, ColInfoGrbit grbit, out JET_COLUMNLIST columnlist);

		int JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string columnName, out JET_COLUMNDEF columndef);

		int JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string ignored, out JET_COLUMNLIST columnlist);

		int JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string columnName, out JET_COLUMNBASE columnbase);

		int JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string columnName, JET_COLUMNID columnid, out JET_COLUMNBASE columnbase);

		int JetGetObjectInfo(JET_SESID sesid, JET_DBID dbid, out JET_OBJECTLIST objectlist);

		int JetGetObjectInfo(JET_SESID sesid, JET_DBID dbid, JET_objtyp objtyp, string objectName, out JET_OBJECTINFO objectinfo);

		int JetGetCurrentIndex(JET_SESID sesid, JET_TABLEID tableid, out string indexName, int maxNameLength);

		int JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out JET_OBJECTINFO result, JET_TblInfo infoLevel);

		int JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out string result, JET_TblInfo infoLevel);

		int JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out JET_DBID result, JET_TblInfo infoLevel);

		int JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, int[] result, JET_TblInfo infoLevel);

		int JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out int result, JET_TblInfo infoLevel);

		int JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out ushort result, JET_IdxInfo infoLevel);

		int JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out int result, JET_IdxInfo infoLevel);

		int JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out JET_INDEXID result, JET_IdxInfo infoLevel);

		int JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out JET_INDEXLIST result, JET_IdxInfo infoLevel);

		int JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out string result, JET_IdxInfo infoLevel);

		int JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out ushort result, JET_IdxInfo infoLevel);

		int JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out int result, JET_IdxInfo infoLevel);

		int JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXID result, JET_IdxInfo infoLevel);

		int JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXLIST result, JET_IdxInfo infoLevel);

		int JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out string result, JET_IdxInfo infoLevel);

		int JetRenameTable(JET_SESID sesid, JET_DBID dbid, string tableName, string newTableName);

		int JetRenameColumn(JET_SESID sesid, JET_TABLEID tableid, string name, string newName, RenameColumnGrbit grbit);

		int JetSetColumnDefaultValue(JET_SESID sesid, JET_DBID dbid, string tableName, string columnName, byte[] data, int dataSize, SetColumnDefaultValueGrbit grbit);

		int JetGotoBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize);

		int JetGotoSecondaryIndexBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] secondaryKey, int secondaryKeySize, byte[] primaryKey, int primaryKeySize, GotoSecondaryIndexBookmarkGrbit grbit);

		int JetMove(JET_SESID sesid, JET_TABLEID tableid, int numRows, MoveGrbit grbit);

		int JetMakeKey(JET_SESID sesid, JET_TABLEID tableid, IntPtr data, int dataSize, MakeKeyGrbit grbit);

		int JetSeek(JET_SESID sesid, JET_TABLEID tableid, SeekGrbit grbit);

		int JetSetIndexRange(JET_SESID sesid, JET_TABLEID tableid, SetIndexRangeGrbit grbit);

		int JetIntersectIndexes(JET_SESID sesid, JET_INDEXRANGE[] ranges, int numRanges, out JET_RECORDLIST recordlist, IntersectIndexesGrbit grbit);

		int JetSetCurrentIndex(JET_SESID sesid, JET_TABLEID tableid, string index);

		int JetSetCurrentIndex2(JET_SESID sesid, JET_TABLEID tableid, string index, SetCurrentIndexGrbit grbit);

		int JetSetCurrentIndex3(JET_SESID sesid, JET_TABLEID tableid, string index, SetCurrentIndexGrbit grbit, int itagSequence);

		int JetSetCurrentIndex4(JET_SESID sesid, JET_TABLEID tableid, string index, JET_INDEXID indexid, SetCurrentIndexGrbit grbit, int itagSequence);

		int JetIndexRecordCount(JET_SESID sesid, JET_TABLEID tableid, out int numRecords, int maxRecordsToCount);

		int JetSetTableSequential(JET_SESID sesid, JET_TABLEID tableid, SetTableSequentialGrbit grbit);

		int JetResetTableSequential(JET_SESID sesid, JET_TABLEID tableid, ResetTableSequentialGrbit grbit);

		int JetGetRecordPosition(JET_SESID sesid, JET_TABLEID tableid, out JET_RECPOS recpos);

		int JetGotoPosition(JET_SESID sesid, JET_TABLEID tableid, JET_RECPOS recpos);

		int JetPrereadKeys(JET_SESID sesid, JET_TABLEID tableid, byte[][] keys, int[] keyLengths, int keyIndex, int keyCount, out int keysPreread, PrereadKeysGrbit grbit);

		int JetGetBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize);

		int JetGetSecondaryIndexBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] secondaryKey, int secondaryKeySize, out int actualSecondaryKeySize, byte[] primaryKey, int primaryKeySize, out int actualPrimaryKeySize, GetSecondaryIndexBookmarkGrbit grbit);

		int JetRetrieveKey(JET_SESID sesid, JET_TABLEID tableid, byte[] data, int dataSize, out int actualDataSize, RetrieveKeyGrbit grbit);

		int JetRetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, IntPtr data, int dataSize, out int actualDataSize, RetrieveColumnGrbit grbit, JET_RETINFO retinfo);

		unsafe int JetRetrieveColumns(JET_SESID sesid, JET_TABLEID tableid, NATIVE_RETRIEVECOLUMN* retrievecolumns, int numColumns);

		int JetEnumerateColumns(JET_SESID sesid, JET_TABLEID tableid, int numColumnids, JET_ENUMCOLUMNID[] columnids, out int numColumnValues, out JET_ENUMCOLUMN[] columnValues, JET_PFNREALLOC allocator, IntPtr allocatorContext, int maxDataSize, EnumerateColumnsGrbit grbit);

		int JetGetRecordSize(JET_SESID sesid, JET_TABLEID tableid, ref JET_RECSIZE recsize, GetRecordSizeGrbit grbit);

		int JetDelete(JET_SESID sesid, JET_TABLEID tableid);

		int JetPrepareUpdate(JET_SESID sesid, JET_TABLEID tableid, JET_prep prep);

		int JetUpdate(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize);

		int JetUpdate2(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize, UpdateGrbit grbit);

		int JetSetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, IntPtr data, int dataSize, SetColumnGrbit grbit, JET_SETINFO setinfo);

		unsafe int JetSetColumns(JET_SESID sesid, JET_TABLEID tableid, NATIVE_SETCOLUMN* setcolumns, int numColumns);

		int JetGetLock(JET_SESID sesid, JET_TABLEID tableid, GetLockGrbit grbit);

		int JetEscrowUpdate(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] delta, int deltaSize, byte[] previousValue, int previousValueLength, out int actualPreviousValueLength, EscrowUpdateGrbit grbit);

		int JetRegisterCallback(JET_SESID sesid, JET_TABLEID tableid, JET_cbtyp cbtyp, JET_CALLBACK callback, IntPtr context, out JET_HANDLE callbackId);

		int JetUnregisterCallback(JET_SESID sesid, JET_TABLEID tableid, JET_cbtyp cbtyp, JET_HANDLE callbackId);

		int JetDefragment(JET_SESID sesid, JET_DBID dbid, string tableName, ref int passes, ref int seconds, DefragGrbit grbit);

		int JetDefragment2(JET_SESID sesid, JET_DBID dbid, string tableName, ref int passes, ref int seconds, JET_CALLBACK callback, DefragGrbit grbit);

		int JetIdle(JET_SESID sesid, IdleGrbit grbit);

		int JetConfigureProcessForCrashDump(CrashDumpGrbit grbit);

		int JetFreeBuffer(IntPtr buffer);

		int JetGetErrorInfo(JET_err error, out JET_ERRINFOBASIC errinfo);

		int JetResizeDatabase(JET_SESID sesid, JET_DBID dbid, int desiredPages, out int actualPages, ResizeDatabaseGrbit grbit);

		int JetCreateIndex4(JET_SESID sesid, JET_TABLEID tableid, JET_INDEXCREATE[] indexcreates, int numIndexCreates);

		int JetOpenTemporaryTable2(JET_SESID sesid, JET_OPENTEMPORARYTABLE temporarytable);

		int JetCreateTableColumnIndex4(JET_SESID sesid, JET_DBID dbid, JET_TABLECREATE tablecreate);

		int JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, byte[] data, int dataSize);

		int JetCommitTransaction2(JET_SESID sesid, CommitTransactionGrbit grbit, TimeSpan durableCommit, out JET_COMMIT_ID commitId);

		int JetPrereadIndexRanges(JET_SESID sesid, JET_TABLEID tableid, JET_INDEX_RANGE[] indexRanges, int rangeIndex, int rangeCount, out int rangesPreread, JET_COLUMNID[] columnsPreread, PrereadIndexRangesGrbit grbit);

		int JetPrereadKeyRanges(JET_SESID sesid, JET_TABLEID tableid, byte[][] keysStart, int[] keyStartLengths, byte[][] keysEnd, int[] keyEndLengths, int rangeIndex, int rangeCount, out int rangesPreread, JET_COLUMNID[] columnsPreread, PrereadIndexRangesGrbit grbit);

		int JetSetCursorFilter(JET_SESID sesid, JET_TABLEID tableid, JET_INDEX_COLUMN[] filters, CursorFilterGrbit grbit);
	}
}
