using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Vista;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Isam.Esent.Interop.Implementation
{
	[BestFitMapping(false, ThrowOnUnmappableChar = true)]
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetTracing(int traceop, int tracetag, IntPtr ul);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, IntPtr ulParam);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetResourceParam(JET_INSTANCE instance, JET_resoper resoper, JET_resid resid, out IntPtr ulParam);

		[DllImport("eseback2.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public unsafe static extern int HrESEBackupRestoreRegister3(string wszDisplayName, uint fFlags, string wszEndpointAnnotation, NATIVE_ESEBACK_CALLBACKS_IMPL* pCallbacks, Guid* pguidCrimsonPublisher);

		[DllImport("eseback2.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int HrESEBackupRestoreUnregister();

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfoW(string szFilename, out NATIVE_DBINFOMISC7 dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetLogFileInfoW(string szLog, out NATIVE_LOGINFOMISC2 pvResult, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public unsafe static extern int JetGetPageInfo2(IntPtr pvPages, uint cbData, NATIVE_PAGEINFO* rgPageInfo, uint cbPageInfo, uint grbit, uint ulInfoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetConsumeLogData(JET_INSTANCE instance, NATIVE_EMITDATACTX* pEmitLogDataCtrx, byte* pvLogData, uint cbLogData, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetInstanceMiscInfo(IntPtr instance, ref NATIVE_CHECKPOINTINFO pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetBeginDatabaseIncrementalReseedW(JET_INSTANCE instance, string szDatabase, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetEndDatabaseIncrementalReseedW(JET_INSTANCE instance, string wszDatabase, uint genMinRequired, uint genFirstDivergedLog, uint genMaxRequired, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public unsafe static extern int JetPatchDatabasePagesW(JET_INSTANCE instance, string szDatabase, uint pgnoStart, uint cpg, byte* pv, uint cb, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetRemoveLogfileW(string szDatabase, string szLogfile, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetDatabasePages(IntPtr sesid, uint dbid, uint pgnoStart, uint cpg, IntPtr pv, uint cb, out uint cbActual, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetDBUtilitiesW(ref NATIVE_DBUTIL_LEGACY pdbutil);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetDBUtilitiesW(ref NATIVE_DBUTIL_CHECKSUMLOG pdbutil);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref uint pv);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref NATIVE_TESTHOOKTESTINJECTION pv);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref NATIVE_TESTHOOKEVICTCACHE pv);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref IntPtr pv);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref NATIVE_TESTHOOKTRACETESTMARKER pv);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTestHook(int opcode, ref NATIVE_TESTHOOKCORRUPT_DATABASEFILE pv);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetPrereadTablesW(IntPtr sesid, uint dbid, string[] rgsz, int csz, int grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetDatabaseScan(IntPtr sesid, uint dbid, ref uint pcSeconds, uint cmsecSleep, IntPtr callback, uint grbit);

		public static Encoding Encoding { get; private set; } = LibraryHelpers.EncodingASCII;

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateInstance(out IntPtr instance, string szInstanceName);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateInstanceW(out IntPtr instance, string szInstanceName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateInstance2(out IntPtr instance, string szInstanceName, string szDisplayName, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateInstance2W(out IntPtr instance, string szInstanceName, string szDisplayName, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetInit(ref IntPtr instance);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetInit2(ref IntPtr instance, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetInit3W(ref IntPtr instance, ref NATIVE_RSTINFO prstinfo, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetInit3W(ref IntPtr instance, IntPtr prstinfo, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetGetInstanceInfo(out uint pcInstanceInfo, out NATIVE_INSTANCE_INFO* prgInstanceInfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetGetInstanceInfoW(out uint pcInstanceInfo, out NATIVE_INSTANCE_INFO* prgInstanceInfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetInstanceMiscInfo(IntPtr instance, ref NATIVE_SIGNATURE pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetStopBackupInstance(IntPtr instance);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetStopServiceInstance(IntPtr instance);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetStopServiceInstance2(IntPtr instance, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTerm(IntPtr instance);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTerm2(IntPtr instance, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public unsafe static extern int JetSetSystemParameter(IntPtr* pinstance, IntPtr sesid, uint paramid, IntPtr lParam, string szParam);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public unsafe static extern int JetSetSystemParameterW(IntPtr* pinstance, IntPtr sesid, uint paramid, IntPtr lParam, string szParam);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetSystemParameter(IntPtr instance, IntPtr sesid, uint paramid, ref IntPtr plParam, [Out] StringBuilder szParam, uint cbMax);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetSystemParameterW(IntPtr instance, IntPtr sesid, uint paramid, ref IntPtr plParam, [Out] StringBuilder szParam, uint cbMax);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetVersion(IntPtr sesid, out uint dwVersion);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateDatabase(IntPtr sesid, string szFilename, string szConnect, out uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateDatabaseW(IntPtr sesid, string szFilename, string szConnect, out uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateDatabase2(IntPtr sesid, string szFilename, uint cpgDatabaseSizeMax, out uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateDatabase2W(IntPtr sesid, string szFilename, uint cpgDatabaseSizeMax, out uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetAttachDatabase(IntPtr sesid, string szFilename, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetAttachDatabaseW(IntPtr sesid, string szFilename, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetAttachDatabase2(IntPtr sesid, string szFilename, uint cpgDatabaseSizeMax, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetAttachDatabase2W(IntPtr sesid, string szFilename, uint cpgDatabaseSizeMax, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDetachDatabase(IntPtr sesid, string szFilename);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDetachDatabase2(IntPtr sesid, string szFilename, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetDetachDatabase2W(IntPtr sesid, string szFilename, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetDetachDatabaseW(IntPtr sesid, string szFilename);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetOpenDatabase(IntPtr sesid, string database, string szConnect, out uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetOpenDatabaseW(IntPtr sesid, string database, string szConnect, out uint dbid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetCloseDatabase(IntPtr sesid, uint dbid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCompact(IntPtr sesid, string szDatabaseSrc, string szDatabaseDest, IntPtr pfnStatus, IntPtr pconvert, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCompactW(IntPtr sesid, string szDatabaseSrc, string szDatabaseDest, IntPtr pfnStatus, IntPtr pconvert, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGrowDatabase(IntPtr sesid, uint dbid, uint cpg, out uint pcpgReal);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetDatabaseSize(IntPtr sesid, string szDatabaseName, uint cpg, out uint pcpgReal);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetSetDatabaseSizeW(IntPtr sesid, string szDatabaseName, uint cpg, out uint pcpgReal);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfo(IntPtr sesid, uint dbid, out int intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfo(IntPtr sesid, uint dbid, out NATIVE_DBINFOMISC dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfo(IntPtr sesid, uint dbid, out NATIVE_DBINFOMISC4 dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfo(IntPtr sesid, uint dbid, [Out] StringBuilder stringValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfoW(IntPtr sesid, uint dbid, out int intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfoW(IntPtr sesid, uint dbid, out NATIVE_DBINFOMISC dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfoW(IntPtr sesid, uint dbid, out NATIVE_DBINFOMISC4 dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseInfoW(IntPtr sesid, uint dbid, [Out] StringBuilder stringValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfoW(string szFilename, out int intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfo(string szFilename, out int intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfoW(string szFilename, out long intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfo(string szFilename, out long intValue, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfoW(string szFilename, out NATIVE_DBINFOMISC4 dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfo(string szFilename, out NATIVE_DBINFOMISC dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetDatabaseFileInfoW(string szFilename, out NATIVE_DBINFOMISC dbinfomisc, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetBackupInstance(IntPtr instance, string szBackupPath, uint grbit, IntPtr pfnStatus);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetBackupInstanceW(IntPtr instance, string szBackupPath, uint grbit, IntPtr pfnStatus);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetRestoreInstance(IntPtr instance, string sz, string szDest, IntPtr pfn);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetRestoreInstanceW(IntPtr instance, string sz, string szDest, IntPtr pfn);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotPrepare(out IntPtr snapId, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotPrepareInstance(IntPtr snapId, IntPtr instance, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetOSSnapshotFreeze(IntPtr snapId, out uint pcInstanceInfo, out NATIVE_INSTANCE_INFO* prgInstanceInfo, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetOSSnapshotFreezeW(IntPtr snapId, out uint pcInstanceInfo, out NATIVE_INSTANCE_INFO* prgInstanceInfo, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetOSSnapshotGetFreezeInfoW(IntPtr snapId, out uint pcInstanceInfo, out NATIVE_INSTANCE_INFO* prgInstanceInfo, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotThaw(IntPtr snapId, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotTruncateLog(IntPtr snapId, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotTruncateLogInstance(IntPtr snapId, IntPtr instance, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotEnd(IntPtr snapId, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOSSnapshotAbort(IntPtr snapId, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetBeginExternalBackupInstance(IntPtr instance, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetCloseFileInstance(IntPtr instance, IntPtr handle);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetEndExternalBackupInstance(IntPtr instance);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetEndExternalBackupInstance2(IntPtr instance, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetAttachInfoInstance(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetAttachInfoInstanceW(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetLogInfoInstance(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetLogInfoInstanceW(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTruncateLogInfoInstance(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTruncateLogInfoInstanceW(IntPtr instance, [Out] byte[] szz, uint cbMax, out uint pcbActual);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetOpenFileInstance(IntPtr instance, string szFileName, out IntPtr phfFile, out uint pulFileSizeLow, out uint pulFileSizeHigh);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetOpenFileInstanceW(IntPtr instance, string szFileName, out IntPtr phfFile, out uint pulFileSizeLow, out uint pulFileSizeHigh);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetReadFileInstance(IntPtr instance, IntPtr handle, IntPtr pv, uint cb, out uint pcbActual);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetTruncateLogInstance(IntPtr instance);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetBeginSession(IntPtr instance, out IntPtr session, string username, string password);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetSessionContext(IntPtr session, IntPtr context);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetResetSessionContext(IntPtr session);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetEndSession(IntPtr sesid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetDupSession(IntPtr sesid, out IntPtr newSesid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetGetThreadStats(JET_THREADSTATS* pvResult, uint cbMax);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetOpenTable(IntPtr sesid, uint dbid, string tablename, byte[] pvParameters, uint cbParameters, uint grbit, out IntPtr tableid);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetOpenTableW(IntPtr sesid, uint dbid, string tablename, byte[] pvParameters, uint cbParameters, uint grbit, out IntPtr tableid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetCloseTable(IntPtr sesid, IntPtr tableid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetDupCursor(IntPtr sesid, IntPtr tableid, out IntPtr tableidNew, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetComputeStats(IntPtr sesid, IntPtr tableid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetLS(IntPtr sesid, IntPtr tableid, IntPtr ls, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetLS(IntPtr sesid, IntPtr tableid, out IntPtr pls, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetCursorInfo(IntPtr sesid, IntPtr tableid, IntPtr pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetBeginTransaction(IntPtr sesid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetBeginTransaction2(IntPtr sesid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetBeginTransaction3(IntPtr sesid, long trxid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetCommitTransaction(IntPtr sesid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetRollback(IntPtr sesid, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateTable(IntPtr sesid, uint dbid, string szTableName, int pages, int density, out IntPtr tableid);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetAddColumn(IntPtr sesid, IntPtr tableid, string szColumnName, [In] ref NATIVE_COLUMNDEF columndef, [In] byte[] pvDefault, uint cbDefault, out uint columnid);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDeleteColumn(IntPtr sesid, IntPtr tableid, string szColumnName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDeleteColumn2(IntPtr sesid, IntPtr tableid, string szColumnName, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDeleteIndex(IntPtr sesid, IntPtr tableid, string szIndexName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDeleteTable(IntPtr sesid, uint dbid, string szTableName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateIndex(IntPtr sesid, IntPtr tableid, string szIndexName, uint grbit, string szKey, uint cbKey, uint lDensity);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateIndex2(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEXCREATE[] pindexcreate, uint cIndexCreate);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateIndex2W(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEXCREATE1[] pindexcreate, uint cIndexCreate);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateIndex3W(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEXCREATE2[] pindexcreate, uint cIndexCreate);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTempTable(IntPtr sesid, [In] NATIVE_COLUMNDEF[] rgcolumndef, uint ccolumn, uint grbit, out IntPtr ptableid, [Out] uint[] rgcolumnid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTempTable2(IntPtr sesid, [In] NATIVE_COLUMNDEF[] rgcolumndef, uint ccolumn, uint lcid, uint grbit, out IntPtr ptableid, [Out] uint[] rgcolumnid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTempTable3(IntPtr sesid, [In] NATIVE_COLUMNDEF[] rgcolumndef, uint ccolumn, [In] ref NATIVE_UNICODEINDEX pidxunicode, uint grbit, out IntPtr ptableid, [Out] uint[] rgcolumnid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTemporaryTable(IntPtr sesid, [In] [Out] ref NATIVE_OPENTEMPORARYTABLE popentemporarytable);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTempTable3(IntPtr sesid, [In] NATIVE_COLUMNDEF[] rgcolumndef, uint ccolumn, IntPtr pidxunicode, uint grbit, out IntPtr ptableid, [Out] uint[] rgcolumnid);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetCreateTableColumnIndex2(IntPtr sesid, uint dbid, ref NATIVE_TABLECREATE2 tablecreate3);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateTableColumnIndex2W(IntPtr sesid, uint dbid, ref NATIVE_TABLECREATE2 tablecreate3);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateTableColumnIndex3W(IntPtr sesid, uint dbid, ref NATIVE_TABLECREATE3 tablecreate3);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, ref uint pcolumnid, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNBASE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, string szIgnored, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfoW(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetTableColumnInfoW(IntPtr sesid, IntPtr tableid, ref uint pcolumnid, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfoW(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNBASE_WIDE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetTableColumnInfoW(IntPtr sesid, IntPtr tableid, ref uint pcolumnid, ref NATIVE_COLUMNBASE_WIDE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableColumnInfoW(IntPtr sesid, IntPtr tableid, string szIgnored, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNBASE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, ref uint pcolumnid, ref NATIVE_COLUMNBASE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetColumnInfoW(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetColumnInfoW(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetColumnInfoW(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNBASE_WIDE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetColumnInfoW(IntPtr sesid, uint dbid, string szTableName, ref uint pcolumnid, ref NATIVE_COLUMNBASE_WIDE columnbase, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetObjectInfo(IntPtr sesid, uint dbid, uint objtyp, string szContainerName, string szObjectName, [In] [Out] ref NATIVE_OBJECTLIST objectlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetObjectInfoW(IntPtr sesid, uint dbid, uint objtyp, string szContainerName, string szObjectName, [In] [Out] ref NATIVE_OBJECTLIST objectlist, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetObjectInfo(IntPtr sesid, uint dbid, uint objtyp, string szContainerName, string szObjectName, [In] [Out] ref NATIVE_OBJECTINFO objectinfo, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetObjectInfoW(IntPtr sesid, uint dbid, uint objtyp, string szContainerName, string szObjectName, [In] [Out] ref NATIVE_OBJECTINFO objectinfo, uint cbMax, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetCurrentIndex(IntPtr sesid, IntPtr tableid, [Out] StringBuilder szIndexName, uint cchIndexName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableInfo(IntPtr sesid, IntPtr tableid, out NATIVE_OBJECTINFO pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableInfo(IntPtr sesid, IntPtr tableid, out uint pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableInfo(IntPtr sesid, IntPtr tableid, [Out] int[] pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableInfo(IntPtr sesid, IntPtr tableid, [Out] StringBuilder pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableInfoW(IntPtr sesid, IntPtr tableid, out NATIVE_OBJECTINFO pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableInfoW(IntPtr sesid, IntPtr tableid, out uint pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableInfoW(IntPtr sesid, IntPtr tableid, [Out] int[] pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableInfoW(IntPtr sesid, IntPtr tableid, [Out] StringBuilder pvResult, uint cbMax, uint infoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetIndexInfo(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out ushort result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetIndexInfo(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out uint result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetIndexInfo(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out JET_INDEXID result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetIndexInfo(IntPtr sesid, uint dbid, string szTableName, string szIndexName, [In] [Out] ref NATIVE_INDEXLIST result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetIndexInfoW(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out ushort result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetIndexInfoW(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out uint result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetIndexInfoW(IntPtr sesid, uint dbid, string szTableName, string szIndexName, out JET_INDEXID result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetIndexInfoW(IntPtr sesid, uint dbid, string szTableName, string szIndexName, [In] [Out] ref NATIVE_INDEXLIST result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetIndexInfoW(IntPtr sesid, uint dbid, string szTableName, string szIndexName, [Out] StringBuilder result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfo(IntPtr sesid, IntPtr tableid, string szIndexName, out ushort result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfo(IntPtr sesid, IntPtr tableid, string szIndexName, out uint result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfo(IntPtr sesid, IntPtr tableid, string szIndexName, out JET_INDEXID result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfo(IntPtr sesid, IntPtr tableid, string szIndexName, [In] [Out] ref NATIVE_INDEXLIST result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfoW(IntPtr sesid, IntPtr tableid, string szIndexName, out ushort result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfoW(IntPtr sesid, IntPtr tableid, string szIndexName, out uint result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfoW(IntPtr sesid, IntPtr tableid, string szIndexName, out JET_INDEXID result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfoW(IntPtr sesid, IntPtr tableid, string szIndexName, [In] [Out] ref NATIVE_INDEXLIST result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetGetTableIndexInfoW(IntPtr sesid, IntPtr tableid, string szIndexName, [Out] StringBuilder result, uint cbResult, uint InfoLevel);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetRenameTable(IntPtr sesid, uint dbid, string szName, string szNameNew);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetRenameColumn(IntPtr sesid, IntPtr tableid, string szName, string szNameNew, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetColumnDefaultValue(IntPtr sesid, uint tableid, [MarshalAs(UnmanagedType.LPStr)] string szTableName, [MarshalAs(UnmanagedType.LPStr)] string szColumnName, byte[] pvData, uint cbData, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGotoBookmark(IntPtr sesid, IntPtr tableid, [In] byte[] pvBookmark, uint cbBookmark);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGotoSecondaryIndexBookmark(IntPtr sesid, IntPtr tableid, [In] byte[] pvSecondaryKey, uint cbSecondaryKey, [In] byte[] pvPrimaryBookmark, uint cbPrimaryBookmark, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetMove(IntPtr sesid, IntPtr tableid, int cRow, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetMakeKey(IntPtr sesid, IntPtr tableid, IntPtr pvData, uint cbData, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSeek(IntPtr sesid, IntPtr tableid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetIndexRange(IntPtr sesid, IntPtr tableid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetIntersectIndexes(IntPtr sesid, [In] NATIVE_INDEXRANGE[] rgindexrange, uint cindexrange, [In] [Out] ref NATIVE_RECORDLIST recordlist, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetCurrentIndex(IntPtr sesid, IntPtr tableid, string szIndexName);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetCurrentIndex2(IntPtr sesid, IntPtr tableid, string szIndexName, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetCurrentIndex3(IntPtr sesid, IntPtr tableid, string szIndexName, uint grbit, uint itagSequence);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetSetCurrentIndex4(IntPtr sesid, IntPtr tableid, string szIndexName, [In] ref JET_INDEXID indexid, uint grbit, uint itagSequence);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetIndexRecordCount(IntPtr sesid, IntPtr tableid, out uint crec, uint crecMax);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetTableSequential(IntPtr sesid, IntPtr tableid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetResetTableSequential(IntPtr sesid, IntPtr tableid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetRecordPosition(IntPtr sesid, IntPtr tableid, out NATIVE_RECPOS precpos, uint cbRecpos);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGotoPosition(IntPtr sesid, IntPtr tableid, [In] ref NATIVE_RECPOS precpos);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetPrereadKeys(IntPtr sesid, IntPtr tableid, void** rgpvKeys, uint* rgcbKeys, int ckeys, out int pckeysPreread, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetBookmark(IntPtr sesid, IntPtr tableid, [Out] byte[] pvBookmark, uint cbMax, out uint cbActual);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetSecondaryIndexBookmark(IntPtr sesid, IntPtr tableid, [Out] byte[] secondaryKey, uint secondaryKeySize, out uint actualSecondaryKeySize, [Out] byte[] primaryKey, uint primaryKeySize, out uint actualPrimaryKeySize, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetRetrieveColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, out uint cbActual, uint grbit, IntPtr pretinfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetRetrieveColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, out uint cbActual, uint grbit, [In] [Out] ref NATIVE_RETINFO pretinfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetRetrieveColumns(IntPtr sesid, IntPtr tableid, [In] [Out] NATIVE_RETRIEVECOLUMN* psetcolumn, uint csetcolumn);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetRetrieveKey(IntPtr sesid, IntPtr tableid, [Out] byte[] pvData, uint cbMax, out uint cbActual, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetEnumerateColumns(IntPtr sesid, IntPtr tableid, uint cEnumColumnId, NATIVE_ENUMCOLUMNID* rgEnumColumnId, out uint pcEnumColumn, out NATIVE_ENUMCOLUMN* prgEnumColumn, JET_PFNREALLOC pfnRealloc, IntPtr pvReallocContext, uint cbDataMost, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetRecordSize(IntPtr sesid, IntPtr tableid, ref NATIVE_RECSIZE precsize, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetRecordSize2(IntPtr sesid, IntPtr tableid, ref NATIVE_RECSIZE2 precsize, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetDelete(IntPtr sesid, IntPtr tableid);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetPrepareUpdate(IntPtr sesid, IntPtr tableid, uint prep);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetUpdate(IntPtr sesid, IntPtr tableid, [Out] byte[] pvBookmark, uint cbBookmark, out uint cbActual);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetUpdate2(IntPtr sesid, IntPtr tableid, [Out] byte[] pvBookmark, uint cbBookmark, out uint cbActual, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, uint grbit, IntPtr psetinfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, uint grbit, [In] ref NATIVE_SETINFO psetinfo);

		[DllImport("ese.dll", ExactSpelling = true)]
		public unsafe static extern int JetSetColumns(IntPtr sesid, IntPtr tableid, [In] [Out] NATIVE_SETCOLUMN* psetcolumn, uint csetcolumn);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetLock(IntPtr sesid, IntPtr tableid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetEscrowUpdate(IntPtr sesid, IntPtr tableid, uint columnid, [In] byte[] pv, uint cbMax, [Out] byte[] pvOld, uint cbOldMax, out uint cbOldActual, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetRegisterCallback(IntPtr sesid, IntPtr tableid, uint cbtyp, NATIVE_CALLBACK callback, IntPtr pvContext, out IntPtr pCallbackId);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetUnregisterCallback(IntPtr sesid, IntPtr tableid, uint cbtyp, IntPtr hCallbackId);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDefragment(IntPtr sesid, uint dbid, string szTableName, ref uint pcPasses, ref uint pcSeconds, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		public static extern int JetDefragment2(IntPtr sesid, uint dbid, string szTableName, ref uint pcPasses, ref uint pcSeconds, IntPtr callback, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetDefragment2W(IntPtr sesid, uint dbid, string szTableName, ref uint pcPasses, ref uint pcSeconds, IntPtr callback, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetIdle(IntPtr sesid, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetConfigureProcessForCrashDump(uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetFreeBuffer(IntPtr pbBuf);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetGetErrorInfoW(ref int error, [In] [Out] ref NATIVE_ERRINFOBASIC pvResult, uint cbMax, uint InfoLevel, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetResizeDatabase(IntPtr sesid, uint dbid, uint cpg, out uint pcpgActual, uint grbit);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateIndex4W(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEXCREATE3[] pindexcreate, uint cIndexCreate);

		[DllImport("ese.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern int JetCreateTableColumnIndex4W(IntPtr sesid, uint dbid, ref NATIVE_TABLECREATE4 tablecreate3);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetOpenTemporaryTable2(IntPtr sesid, [In] [Out] ref NATIVE_OPENTEMPORARYTABLE2 popentemporarytable);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetSessionParameter(IntPtr sesid, uint sesparamid, IntPtr data, int dataSize);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetCommitTransaction2(IntPtr sesid, uint grbit, uint cmsecDurableCommit, ref NATIVE_COMMIT_ID pCommitId);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetPrereadIndexRanges(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEX_RANGE[] pIndexRanges, uint cIndexRanges, out int pcRangesPreread, uint[] rgcolumnidPreread, uint ccolumnidPreread, uint grbit);

		[DllImport("ese.dll", ExactSpelling = true)]
		public static extern int JetSetCursorFilter(IntPtr sesid, IntPtr tableid, [In] NATIVE_INDEX_COLUMN[] pFilters, uint cFilters, uint grbit);

		private const string EsentDll = "ese.dll";

		private const string Eseback2Dll = "eseback2.dll";

		private const CharSet EsentCharSet = CharSet.Ansi;
	}
}
