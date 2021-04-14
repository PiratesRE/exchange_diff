using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Text;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop
{
	public static class Api
	{
		internal static event Api.ErrorHandler HandleError;

		internal static IJetApi Impl { get; set; } = new JetApi();

		public static void JetCreateInstance(out JET_INSTANCE instance, string name)
		{
			Api.Check(Api.Impl.JetCreateInstance(out instance, name));
		}

		public static void JetCreateInstance2(out JET_INSTANCE instance, string name, string displayName, CreateInstanceGrbit grbit)
		{
			Api.Check(Api.Impl.JetCreateInstance2(out instance, name, displayName, grbit));
		}

		public static void JetInit(ref JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetInit(ref instance));
		}

		public static JET_wrn JetInit2(ref JET_INSTANCE instance, InitGrbit grbit)
		{
			return Api.Check(Api.Impl.JetInit2(ref instance, grbit));
		}

		public static void JetGetInstanceInfo(out int numInstances, out JET_INSTANCE_INFO[] instances)
		{
			Api.Check(Api.Impl.JetGetInstanceInfo(out numInstances, out instances));
		}

		public static void JetStopBackupInstance(JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetStopBackupInstance(instance));
		}

		public static void JetStopServiceInstance(JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetStopServiceInstance(instance));
		}

		public static void JetTerm(JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetTerm(instance));
		}

		public static void JetTerm2(JET_INSTANCE instance, TermGrbit grbit)
		{
			Api.Check(Api.Impl.JetTerm2(instance, grbit));
		}

		public static JET_wrn JetSetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, int paramValue, string paramString)
		{
			return Api.Check(Api.Impl.JetSetSystemParameter(instance, sesid, paramid, new IntPtr(paramValue), paramString));
		}

		public static JET_wrn JetSetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, JET_CALLBACK paramValue, string paramString)
		{
			return Api.Check(Api.Impl.JetSetSystemParameter(instance, sesid, paramid, paramValue, paramString));
		}

		public static JET_wrn JetSetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, IntPtr paramValue, string paramString)
		{
			return Api.Check(Api.Impl.JetSetSystemParameter(instance, sesid, paramid, paramValue, paramString));
		}

		public static JET_wrn JetGetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, ref IntPtr paramValue, out string paramString, int maxParam)
		{
			return Api.Check(Api.Impl.JetGetSystemParameter(instance, sesid, paramid, ref paramValue, out paramString, maxParam));
		}

		public static JET_wrn JetGetSystemParameter(JET_INSTANCE instance, JET_SESID sesid, JET_param paramid, ref int paramValue, out string paramString, int maxParam)
		{
			IntPtr intPtr = new IntPtr(paramValue);
			JET_wrn result = Api.Check(Api.Impl.JetGetSystemParameter(instance, sesid, paramid, ref intPtr, out paramString, maxParam));
			paramValue = intPtr.ToInt32();
			return result;
		}

		[CLSCompliant(false)]
		public static void JetGetVersion(JET_SESID sesid, out uint version)
		{
			Api.Check(Api.Impl.JetGetVersion(sesid, out version));
		}

		public static void JetCreateDatabase(JET_SESID sesid, string database, string connect, out JET_DBID dbid, CreateDatabaseGrbit grbit)
		{
			Api.Check(Api.Impl.JetCreateDatabase(sesid, database, connect, out dbid, grbit));
		}

		public static void JetCreateDatabase2(JET_SESID sesid, string database, int maxPages, out JET_DBID dbid, CreateDatabaseGrbit grbit)
		{
			Api.Check(Api.Impl.JetCreateDatabase2(sesid, database, maxPages, out dbid, grbit));
		}

		public static JET_wrn JetAttachDatabase(JET_SESID sesid, string database, AttachDatabaseGrbit grbit)
		{
			return Api.Check(Api.Impl.JetAttachDatabase(sesid, database, grbit));
		}

		public static JET_wrn JetAttachDatabase2(JET_SESID sesid, string database, int maxPages, AttachDatabaseGrbit grbit)
		{
			return Api.Check(Api.Impl.JetAttachDatabase2(sesid, database, maxPages, grbit));
		}

		public static JET_wrn JetOpenDatabase(JET_SESID sesid, string database, string connect, out JET_DBID dbid, OpenDatabaseGrbit grbit)
		{
			return Api.Check(Api.Impl.JetOpenDatabase(sesid, database, connect, out dbid, grbit));
		}

		public static void JetCloseDatabase(JET_SESID sesid, JET_DBID dbid, CloseDatabaseGrbit grbit)
		{
			Api.Check(Api.Impl.JetCloseDatabase(sesid, dbid, grbit));
		}

		public static void JetDetachDatabase(JET_SESID sesid, string database)
		{
			Api.Check(Api.Impl.JetDetachDatabase(sesid, database));
		}

		public static void JetDetachDatabase2(JET_SESID sesid, string database, DetachDatabaseGrbit grbit)
		{
			Api.Check(Api.Impl.JetDetachDatabase2(sesid, database, grbit));
		}

		public static void JetCompact(JET_SESID sesid, string sourceDatabase, string destinationDatabase, JET_PFNSTATUS statusCallback, JET_CONVERT ignored, CompactGrbit grbit)
		{
			Api.Check(Api.Impl.JetCompact(sesid, sourceDatabase, destinationDatabase, statusCallback, ignored, grbit));
		}

		public static void JetGrowDatabase(JET_SESID sesid, JET_DBID dbid, int desiredPages, out int actualPages)
		{
			Api.Check(Api.Impl.JetGrowDatabase(sesid, dbid, desiredPages, out actualPages));
		}

		public static void JetSetDatabaseSize(JET_SESID sesid, string database, int desiredPages, out int actualPages)
		{
			Api.Check(Api.Impl.JetSetDatabaseSize(sesid, database, desiredPages, out actualPages));
		}

		public static void JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out int value, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseInfo(sesid, dbid, out value, infoLevel));
		}

		public static void JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out JET_DBINFOMISC dbinfomisc, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseInfo(sesid, dbid, out dbinfomisc, infoLevel));
		}

		public static void JetGetDatabaseInfo(JET_SESID sesid, JET_DBID dbid, out string value, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseInfo(sesid, dbid, out value, infoLevel));
		}

		public static void JetGetDatabaseFileInfo(string databaseName, out int value, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseFileInfo(databaseName, out value, infoLevel));
		}

		public static void JetGetDatabaseFileInfo(string databaseName, out long value, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseFileInfo(databaseName, out value, infoLevel));
		}

		public static void JetGetDatabaseFileInfo(string databaseName, out JET_DBINFOMISC dbinfomisc, JET_DbInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetDatabaseFileInfo(databaseName, out dbinfomisc, infoLevel));
		}

		public static void JetBackupInstance(JET_INSTANCE instance, string destination, BackupGrbit grbit, JET_PFNSTATUS statusCallback)
		{
			Api.Check(Api.Impl.JetBackupInstance(instance, destination, grbit, statusCallback));
		}

		public static void JetRestoreInstance(JET_INSTANCE instance, string source, string destination, JET_PFNSTATUS statusCallback)
		{
			Api.Check(Api.Impl.JetRestoreInstance(instance, source, destination, statusCallback));
		}

		public static void JetOSSnapshotFreeze(JET_OSSNAPID snapshot, out int numInstances, out JET_INSTANCE_INFO[] instances, SnapshotFreezeGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotFreeze(snapshot, out numInstances, out instances, grbit));
		}

		public static void JetOSSnapshotPrepare(out JET_OSSNAPID snapshot, SnapshotPrepareGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotPrepare(out snapshot, grbit));
		}

		public static void JetOSSnapshotThaw(JET_OSSNAPID snapshot, SnapshotThawGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotThaw(snapshot, grbit));
		}

		public static void JetBeginExternalBackupInstance(JET_INSTANCE instance, BeginExternalBackupGrbit grbit)
		{
			Api.Check(Api.Impl.JetBeginExternalBackupInstance(instance, grbit));
		}

		public static void JetCloseFileInstance(JET_INSTANCE instance, JET_HANDLE handle)
		{
			Api.Check(Api.Impl.JetCloseFileInstance(instance, handle));
		}

		public static void JetEndExternalBackupInstance(JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetEndExternalBackupInstance(instance));
		}

		public static void JetEndExternalBackupInstance2(JET_INSTANCE instance, EndExternalBackupGrbit grbit)
		{
			Api.Check(Api.Impl.JetEndExternalBackupInstance2(instance, grbit));
		}

		public static void JetGetAttachInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars)
		{
			Api.Check(Api.Impl.JetGetAttachInfoInstance(instance, out files, maxChars, out actualChars));
		}

		public static void JetGetLogInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars)
		{
			Api.Check(Api.Impl.JetGetLogInfoInstance(instance, out files, maxChars, out actualChars));
		}

		public static void JetGetTruncateLogInfoInstance(JET_INSTANCE instance, out string files, int maxChars, out int actualChars)
		{
			Api.Check(Api.Impl.JetGetTruncateLogInfoInstance(instance, out files, maxChars, out actualChars));
		}

		public static void JetOpenFileInstance(JET_INSTANCE instance, string file, out JET_HANDLE handle, out long fileSizeLow, out long fileSizeHigh)
		{
			Api.Check(Api.Impl.JetOpenFileInstance(instance, file, out handle, out fileSizeLow, out fileSizeHigh));
		}

		public static void JetReadFileInstance(JET_INSTANCE instance, JET_HANDLE file, byte[] buffer, int bufferSize, out int bytesRead)
		{
			Api.Check(Api.Impl.JetReadFileInstance(instance, file, buffer, bufferSize, out bytesRead));
		}

		public static void JetTruncateLogInstance(JET_INSTANCE instance)
		{
			Api.Check(Api.Impl.JetTruncateLogInstance(instance));
		}

		public static void JetBeginSession(JET_INSTANCE instance, out JET_SESID sesid, string username, string password)
		{
			Api.Check(Api.Impl.JetBeginSession(instance, out sesid, username, password));
		}

		public static void JetSetSessionContext(JET_SESID sesid, IntPtr context)
		{
			Api.Check(Api.Impl.JetSetSessionContext(sesid, context));
		}

		public static void JetResetSessionContext(JET_SESID sesid)
		{
			Api.Check(Api.Impl.JetResetSessionContext(sesid));
		}

		public static void JetEndSession(JET_SESID sesid, EndSessionGrbit grbit)
		{
			Api.Check(Api.Impl.JetEndSession(sesid, grbit));
		}

		public static void JetDupSession(JET_SESID sesid, out JET_SESID newSesid)
		{
			Api.Check(Api.Impl.JetDupSession(sesid, out newSesid));
		}

		public static JET_wrn JetOpenTable(JET_SESID sesid, JET_DBID dbid, string tablename, byte[] parameters, int parametersSize, OpenTableGrbit grbit, out JET_TABLEID tableid)
		{
			return Api.Check(Api.Impl.JetOpenTable(sesid, dbid, tablename, parameters, parametersSize, grbit, out tableid));
		}

		public static void JetCloseTable(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.Check(Api.Impl.JetCloseTable(sesid, tableid));
		}

		public static void JetDupCursor(JET_SESID sesid, JET_TABLEID tableid, out JET_TABLEID newTableid, DupCursorGrbit grbit)
		{
			Api.Check(Api.Impl.JetDupCursor(sesid, tableid, out newTableid, grbit));
		}

		public static void JetComputeStats(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.Check(Api.Impl.JetComputeStats(sesid, tableid));
		}

		public static void JetSetLS(JET_SESID sesid, JET_TABLEID tableid, JET_LS ls, LsGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetLS(sesid, tableid, ls, grbit));
		}

		public static void JetGetLS(JET_SESID sesid, JET_TABLEID tableid, out JET_LS ls, LsGrbit grbit)
		{
			Api.Check(Api.Impl.JetGetLS(sesid, tableid, out ls, grbit));
		}

		public static void JetGetCursorInfo(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.Check(Api.Impl.JetGetCursorInfo(sesid, tableid));
		}

		public static void JetBeginTransaction(JET_SESID sesid)
		{
			Api.Check(Api.Impl.JetBeginTransaction(sesid));
		}

		public static void JetBeginTransaction2(JET_SESID sesid, BeginTransactionGrbit grbit)
		{
			Api.Check(Api.Impl.JetBeginTransaction2(sesid, grbit));
		}

		public static void JetCommitTransaction(JET_SESID sesid, CommitTransactionGrbit grbit)
		{
			Api.Check(Api.Impl.JetCommitTransaction(sesid, grbit));
		}

		public static void JetRollback(JET_SESID sesid, RollbackTransactionGrbit grbit)
		{
			Api.Check(Api.Impl.JetRollback(sesid, grbit));
		}

		public static void JetCreateTable(JET_SESID sesid, JET_DBID dbid, string table, int pages, int density, out JET_TABLEID tableid)
		{
			Api.Check(Api.Impl.JetCreateTable(sesid, dbid, table, pages, density, out tableid));
		}

		public static void JetAddColumn(JET_SESID sesid, JET_TABLEID tableid, string column, JET_COLUMNDEF columndef, byte[] defaultValue, int defaultValueSize, out JET_COLUMNID columnid)
		{
			Api.Check(Api.Impl.JetAddColumn(sesid, tableid, column, columndef, defaultValue, defaultValueSize, out columnid));
		}

		public static void JetDeleteColumn(JET_SESID sesid, JET_TABLEID tableid, string column)
		{
			Api.Check(Api.Impl.JetDeleteColumn(sesid, tableid, column));
		}

		public static void JetDeleteColumn2(JET_SESID sesid, JET_TABLEID tableid, string column, DeleteColumnGrbit grbit)
		{
			Api.Check(Api.Impl.JetDeleteColumn2(sesid, tableid, column, grbit));
		}

		public static void JetDeleteIndex(JET_SESID sesid, JET_TABLEID tableid, string index)
		{
			Api.Check(Api.Impl.JetDeleteIndex(sesid, tableid, index));
		}

		public static void JetDeleteTable(JET_SESID sesid, JET_DBID dbid, string table)
		{
			Api.Check(Api.Impl.JetDeleteTable(sesid, dbid, table));
		}

		public static void JetCreateIndex(JET_SESID sesid, JET_TABLEID tableid, string indexName, CreateIndexGrbit grbit, string keyDescription, int keyDescriptionLength, int density)
		{
			Api.Check(Api.Impl.JetCreateIndex(sesid, tableid, indexName, grbit, keyDescription, keyDescriptionLength, density));
		}

		public static void JetCreateIndex2(JET_SESID sesid, JET_TABLEID tableid, JET_INDEXCREATE[] indexcreates, int numIndexCreates)
		{
			Api.Check(Api.Impl.JetCreateIndex2(sesid, tableid, indexcreates, numIndexCreates));
		}

		public static void JetOpenTempTable(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids)
		{
			Api.Check(Api.Impl.JetOpenTempTable(sesid, columns, numColumns, grbit, out tableid, columnids));
		}

		public static void JetOpenTempTable2(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, int lcid, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids)
		{
			Api.Check(Api.Impl.JetOpenTempTable2(sesid, columns, numColumns, lcid, grbit, out tableid, columnids));
		}

		public static void JetOpenTempTable3(JET_SESID sesid, JET_COLUMNDEF[] columns, int numColumns, JET_UNICODEINDEX unicodeindex, TempTableGrbit grbit, out JET_TABLEID tableid, JET_COLUMNID[] columnids)
		{
			Api.Check(Api.Impl.JetOpenTempTable3(sesid, columns, numColumns, unicodeindex, grbit, out tableid, columnids));
		}

		public static void JetCreateTableColumnIndex3(JET_SESID sesid, JET_DBID dbid, JET_TABLECREATE tablecreate)
		{
			Api.Check(Api.Impl.JetCreateTableColumnIndex3(sesid, dbid, tablecreate));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, out JET_COLUMNDEF columndef)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnName, out columndef));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, out JET_COLUMNDEF columndef)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnid, out columndef));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, out JET_COLUMNBASE columnbase)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnName, out columnbase));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, out JET_COLUMNLIST columnlist)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnName, ColInfoGrbit.None, out columnlist));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, string columnName, ColInfoGrbit grbit, out JET_COLUMNLIST columnlist)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnName, grbit, out columnlist));
		}

		public static void JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string columnName, out JET_COLUMNDEF columndef)
		{
			Api.Check(Api.Impl.JetGetColumnInfo(sesid, dbid, tablename, columnName, out columndef));
		}

		public static void JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string columnName, out JET_COLUMNLIST columnlist)
		{
			Api.Check(Api.Impl.JetGetColumnInfo(sesid, dbid, tablename, columnName, out columnlist));
		}

		public static void JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string columnName, out JET_COLUMNBASE columnbase)
		{
			Api.Check(Api.Impl.JetGetColumnInfo(sesid, dbid, tablename, columnName, out columnbase));
		}

		public static void JetGetObjectInfo(JET_SESID sesid, JET_DBID dbid, out JET_OBJECTLIST objectlist)
		{
			Api.Check(Api.Impl.JetGetObjectInfo(sesid, dbid, out objectlist));
		}

		public static void JetGetObjectInfo(JET_SESID sesid, JET_DBID dbid, JET_objtyp objtyp, string objectName, out JET_OBJECTINFO objectinfo)
		{
			Api.Check(Api.Impl.JetGetObjectInfo(sesid, dbid, objtyp, objectName, out objectinfo));
		}

		public static void JetGetCurrentIndex(JET_SESID sesid, JET_TABLEID tableid, out string indexName, int maxNameLength)
		{
			Api.Check(Api.Impl.JetGetCurrentIndex(sesid, tableid, out indexName, maxNameLength));
		}

		public static void JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out JET_OBJECTINFO result, JET_TblInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableInfo(sesid, tableid, out result, infoLevel));
		}

		public static void JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out string result, JET_TblInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableInfo(sesid, tableid, out result, infoLevel));
		}

		public static void JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out JET_DBID result, JET_TblInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableInfo(sesid, tableid, out result, infoLevel));
		}

		public static void JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, int[] result, JET_TblInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableInfo(sesid, tableid, result, infoLevel));
		}

		public static void JetGetTableInfo(JET_SESID sesid, JET_TABLEID tableid, out int result, JET_TblInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableInfo(sesid, tableid, out result, infoLevel));
		}

		[CLSCompliant(false)]
		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out ushort result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetIndexInfo(sesid, dbid, tablename, indexname, out result, infoLevel));
		}

		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out int result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetIndexInfo(sesid, dbid, tablename, indexname, out result, infoLevel));
		}

		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out JET_INDEXID result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetIndexInfo(sesid, dbid, tablename, indexname, out result, infoLevel));
		}

		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out JET_INDEXLIST result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetIndexInfo(sesid, dbid, tablename, indexname, out result, infoLevel));
		}

		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string indexname, out string result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetIndexInfo(sesid, dbid, tablename, indexname, out result, infoLevel));
		}

		[CLSCompliant(false)]
		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out ushort result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel));
		}

		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out int result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel));
		}

		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXID result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel));
		}

		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXLIST result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel));
		}

		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out string result, JET_IdxInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel));
		}

		public static void JetRenameTable(JET_SESID sesid, JET_DBID dbid, string tableName, string newTableName)
		{
			Api.Check(Api.Impl.JetRenameTable(sesid, dbid, tableName, newTableName));
		}

		public static void JetRenameColumn(JET_SESID sesid, JET_TABLEID tableid, string name, string newName, RenameColumnGrbit grbit)
		{
			Api.Check(Api.Impl.JetRenameColumn(sesid, tableid, name, newName, grbit));
		}

		public static void JetSetColumnDefaultValue(JET_SESID sesid, JET_DBID dbid, string tableName, string columnName, byte[] data, int dataSize, SetColumnDefaultValueGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetColumnDefaultValue(sesid, dbid, tableName, columnName, data, dataSize, grbit));
		}

		public static void JetGotoBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize)
		{
			Api.Check(Api.Impl.JetGotoBookmark(sesid, tableid, bookmark, bookmarkSize));
		}

		public static void JetGotoSecondaryIndexBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] secondaryKey, int secondaryKeySize, byte[] primaryKey, int primaryKeySize, GotoSecondaryIndexBookmarkGrbit grbit)
		{
			Api.Check(Api.Impl.JetGotoSecondaryIndexBookmark(sesid, tableid, secondaryKey, secondaryKeySize, primaryKey, primaryKeySize, grbit));
		}

		public static void JetMove(JET_SESID sesid, JET_TABLEID tableid, int numRows, MoveGrbit grbit)
		{
			Api.Check(Api.Impl.JetMove(sesid, tableid, numRows, grbit));
		}

		public static void JetMove(JET_SESID sesid, JET_TABLEID tableid, JET_Move numRows, MoveGrbit grbit)
		{
			Api.Check(Api.Impl.JetMove(sesid, tableid, (int)numRows, grbit));
		}

		public unsafe static void JetMakeKey(JET_SESID sesid, JET_TABLEID tableid, byte[] data, int dataSize, MakeKeyGrbit grbit)
		{
			if ((data == null && dataSize != 0) || (data != null && dataSize > data.Length))
			{
				throw new ArgumentOutOfRangeException("dataSize", dataSize, "cannot be greater than the length of the data");
			}
			fixed (byte* ptr = data)
			{
				Api.JetMakeKey(sesid, tableid, new IntPtr((void*)ptr), dataSize, grbit);
			}
		}

		public static JET_wrn JetSeek(JET_SESID sesid, JET_TABLEID tableid, SeekGrbit grbit)
		{
			return Api.Check(Api.Impl.JetSeek(sesid, tableid, grbit));
		}

		public static void JetSetIndexRange(JET_SESID sesid, JET_TABLEID tableid, SetIndexRangeGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetIndexRange(sesid, tableid, grbit));
		}

		public static void JetIntersectIndexes(JET_SESID sesid, JET_INDEXRANGE[] ranges, int numRanges, out JET_RECORDLIST recordlist, IntersectIndexesGrbit grbit)
		{
			Api.Check(Api.Impl.JetIntersectIndexes(sesid, ranges, numRanges, out recordlist, grbit));
		}

		public static void JetSetCurrentIndex(JET_SESID sesid, JET_TABLEID tableid, string index)
		{
			Api.Check(Api.Impl.JetSetCurrentIndex(sesid, tableid, index));
		}

		public static void JetSetCurrentIndex2(JET_SESID sesid, JET_TABLEID tableid, string index, SetCurrentIndexGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetCurrentIndex2(sesid, tableid, index, grbit));
		}

		public static void JetSetCurrentIndex3(JET_SESID sesid, JET_TABLEID tableid, string index, SetCurrentIndexGrbit grbit, int itagSequence)
		{
			Api.Check(Api.Impl.JetSetCurrentIndex3(sesid, tableid, index, grbit, itagSequence));
		}

		public static void JetSetCurrentIndex4(JET_SESID sesid, JET_TABLEID tableid, string index, JET_INDEXID indexid, SetCurrentIndexGrbit grbit, int itagSequence)
		{
			Api.Check(Api.Impl.JetSetCurrentIndex4(sesid, tableid, index, indexid, grbit, itagSequence));
		}

		public static void JetIndexRecordCount(JET_SESID sesid, JET_TABLEID tableid, out int numRecords, int maxRecordsToCount)
		{
			if (maxRecordsToCount == 0)
			{
				maxRecordsToCount = int.MaxValue;
			}
			Api.Check(Api.Impl.JetIndexRecordCount(sesid, tableid, out numRecords, maxRecordsToCount));
		}

		public static void JetSetTableSequential(JET_SESID sesid, JET_TABLEID tableid, SetTableSequentialGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetTableSequential(sesid, tableid, grbit));
		}

		public static void JetResetTableSequential(JET_SESID sesid, JET_TABLEID tableid, ResetTableSequentialGrbit grbit)
		{
			Api.Check(Api.Impl.JetResetTableSequential(sesid, tableid, grbit));
		}

		public static void JetGetRecordPosition(JET_SESID sesid, JET_TABLEID tableid, out JET_RECPOS recpos)
		{
			Api.Check(Api.Impl.JetGetRecordPosition(sesid, tableid, out recpos));
		}

		public static void JetGotoPosition(JET_SESID sesid, JET_TABLEID tableid, JET_RECPOS recpos)
		{
			Api.Check(Api.Impl.JetGotoPosition(sesid, tableid, recpos));
		}

		public static void JetGetBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize)
		{
			Api.Check(Api.Impl.JetGetBookmark(sesid, tableid, bookmark, bookmarkSize, out actualBookmarkSize));
		}

		public static void JetGetSecondaryIndexBookmark(JET_SESID sesid, JET_TABLEID tableid, byte[] secondaryKey, int secondaryKeySize, out int actualSecondaryKeySize, byte[] primaryKey, int primaryKeySize, out int actualPrimaryKeySize, GetSecondaryIndexBookmarkGrbit grbit)
		{
			Api.Check(Api.Impl.JetGetSecondaryIndexBookmark(sesid, tableid, secondaryKey, secondaryKeySize, out actualSecondaryKeySize, primaryKey, primaryKeySize, out actualPrimaryKeySize, grbit));
		}

		public static void JetRetrieveKey(JET_SESID sesid, JET_TABLEID tableid, byte[] data, int dataSize, out int actualDataSize, RetrieveKeyGrbit grbit)
		{
			Api.Check(Api.Impl.JetRetrieveKey(sesid, tableid, data, dataSize, out actualDataSize, grbit));
		}

		public static JET_wrn JetRetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data, int dataSize, out int actualDataSize, RetrieveColumnGrbit grbit, JET_RETINFO retinfo)
		{
			return Api.JetRetrieveColumn(sesid, tableid, columnid, data, dataSize, 0, out actualDataSize, grbit, retinfo);
		}

		public unsafe static JET_wrn JetRetrieveColumns(JET_SESID sesid, JET_TABLEID tableid, JET_RETRIEVECOLUMN[] retrievecolumns, int numColumns)
		{
			if (retrievecolumns == null)
			{
				throw new ArgumentNullException("retrievecolumns");
			}
			if (numColumns < 0 || numColumns > retrievecolumns.Length)
			{
				throw new ArgumentOutOfRangeException("numColumns", numColumns, "cannot be negative or greater than retrievecolumns.Length");
			}
			NATIVE_RETRIEVECOLUMN* ptr = stackalloc NATIVE_RETRIEVECOLUMN[checked(unchecked((UIntPtr)numColumns) * (UIntPtr)sizeof(NATIVE_RETRIEVECOLUMN))];
			int err = Api.PinColumnsAndRetrieve(sesid, tableid, ptr, retrievecolumns, numColumns, 0);
			for (int i = 0; i < numColumns; i++)
			{
				retrievecolumns[i].UpdateFromNativeRetrievecolumn(ref ptr[i]);
			}
			return Api.Check(err);
		}

		[CLSCompliant(false)]
		public static JET_wrn JetEnumerateColumns(JET_SESID sesid, JET_TABLEID tableid, int numColumnids, JET_ENUMCOLUMNID[] columnids, out int numColumnValues, out JET_ENUMCOLUMN[] columnValues, JET_PFNREALLOC allocator, IntPtr allocatorContext, int maxDataSize, EnumerateColumnsGrbit grbit)
		{
			return Api.Check(Api.Impl.JetEnumerateColumns(sesid, tableid, numColumnids, columnids, out numColumnValues, out columnValues, allocator, allocatorContext, maxDataSize, grbit));
		}

		public static void JetDelete(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.Check(Api.Impl.JetDelete(sesid, tableid));
		}

		public static void JetPrepareUpdate(JET_SESID sesid, JET_TABLEID tableid, JET_prep prep)
		{
			Api.Check(Api.Impl.JetPrepareUpdate(sesid, tableid, prep));
		}

		public static void JetUpdate(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize)
		{
			Api.Check(Api.Impl.JetUpdate(sesid, tableid, bookmark, bookmarkSize, out actualBookmarkSize));
		}

		public static void JetUpdate(JET_SESID sesid, JET_TABLEID tableid)
		{
			int num;
			Api.Check(Api.Impl.JetUpdate(sesid, tableid, null, 0, out num));
		}

		public static JET_wrn JetSetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data, int dataSize, SetColumnGrbit grbit, JET_SETINFO setinfo)
		{
			return Api.JetSetColumn(sesid, tableid, columnid, data, dataSize, 0, grbit, setinfo);
		}

		[SecurityPermission(SecurityAction.LinkDemand)]
		public unsafe static JET_wrn JetSetColumns(JET_SESID sesid, JET_TABLEID tableid, JET_SETCOLUMN[] setcolumns, int numColumns)
		{
			if (setcolumns == null)
			{
				throw new ArgumentNullException("setcolumns");
			}
			if (numColumns < 0 || numColumns > setcolumns.Length)
			{
				throw new ArgumentOutOfRangeException("numColumns", numColumns, "cannot be negative or greater than setcolumns.Length");
			}
			JET_wrn result;
			using (GCHandleCollection gchandleCollection = default(GCHandleCollection))
			{
				NATIVE_SETCOLUMN* ptr = stackalloc NATIVE_SETCOLUMN[checked(unchecked((UIntPtr)numColumns) * (UIntPtr)sizeof(NATIVE_SETCOLUMN))];
				byte* ptr2 = stackalloc byte[(UIntPtr)128];
				int num = 128;
				for (int i = 0; i < numColumns; i++)
				{
					setcolumns[i].CheckDataSize();
					ptr[i] = setcolumns[i].GetNativeSetcolumn();
					if (setcolumns[i].pvData == null)
					{
						ptr[i].pvData = IntPtr.Zero;
					}
					else if (num >= setcolumns[i].cbData)
					{
						ptr[i].pvData = new IntPtr((void*)ptr2);
						Marshal.Copy(setcolumns[i].pvData, setcolumns[i].ibData, ptr[i].pvData, setcolumns[i].cbData);
						ptr2 += setcolumns[i].cbData;
						num -= setcolumns[i].cbData;
					}
					else
					{
						byte* ptr3 = (byte*)gchandleCollection.Add(setcolumns[i].pvData).ToPointer();
						ptr[i].pvData = new IntPtr((void*)(ptr3 + setcolumns[i].ibData));
					}
				}
				int err = Api.Impl.JetSetColumns(sesid, tableid, ptr, numColumns);
				for (int j = 0; j < numColumns; j++)
				{
					setcolumns[j].err = (JET_wrn)ptr[j].err;
				}
				result = Api.Check(err);
			}
			return result;
		}

		public static void JetGetLock(JET_SESID sesid, JET_TABLEID tableid, GetLockGrbit grbit)
		{
			Api.Check(Api.Impl.JetGetLock(sesid, tableid, grbit));
		}

		public static void JetEscrowUpdate(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] delta, int deltaSize, byte[] previousValue, int previousValueLength, out int actualPreviousValueLength, EscrowUpdateGrbit grbit)
		{
			Api.Check(Api.Impl.JetEscrowUpdate(sesid, tableid, columnid, delta, deltaSize, previousValue, previousValueLength, out actualPreviousValueLength, grbit));
		}

		public static void JetRegisterCallback(JET_SESID sesid, JET_TABLEID tableid, JET_cbtyp cbtyp, JET_CALLBACK callback, IntPtr context, out JET_HANDLE callbackId)
		{
			Api.Check(Api.Impl.JetRegisterCallback(sesid, tableid, cbtyp, callback, context, out callbackId));
		}

		public static void JetUnregisterCallback(JET_SESID sesid, JET_TABLEID tableid, JET_cbtyp cbtyp, JET_HANDLE callbackId)
		{
			Api.Check(Api.Impl.JetUnregisterCallback(sesid, tableid, cbtyp, callbackId));
		}

		public static JET_wrn JetDefragment(JET_SESID sesid, JET_DBID dbid, string tableName, ref int passes, ref int seconds, DefragGrbit grbit)
		{
			return Api.Check(Api.Impl.JetDefragment(sesid, dbid, tableName, ref passes, ref seconds, grbit));
		}

		public static JET_wrn JetDefragment2(JET_SESID sesid, JET_DBID dbid, string tableName, ref int passes, ref int seconds, JET_CALLBACK callback, DefragGrbit grbit)
		{
			return Api.Check(Api.Impl.JetDefragment2(sesid, dbid, tableName, ref passes, ref seconds, callback, grbit));
		}

		public static JET_wrn JetIdle(JET_SESID sesid, IdleGrbit grbit)
		{
			return Api.Check(Api.Impl.JetIdle(sesid, grbit));
		}

		public static void JetFreeBuffer(IntPtr buffer)
		{
			Api.Check(Api.Impl.JetFreeBuffer(buffer));
		}

		internal static JET_wrn Check(int err)
		{
			if (err < 0)
			{
				throw Api.CreateErrorException(err);
			}
			return (JET_wrn)err;
		}

		private static Exception CreateErrorException(int err)
		{
			Api.ErrorHandler handleError = Api.HandleError;
			if (handleError != null)
			{
				handleError((JET_err)err);
			}
			return EsentExceptionHelper.JetErrToException((JET_err)err);
		}

		public static bool TryGetLock(JET_SESID sesid, JET_TABLEID tableid, GetLockGrbit grbit)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetGetLock(sesid, tableid, grbit);
			if (JET_err.WriteConflict == jet_err)
			{
				return false;
			}
			Api.Check((int)jet_err);
			return true;
		}

		public unsafe static JET_wrn JetSetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data, int dataSize, int dataOffset, SetColumnGrbit grbit, JET_SETINFO setinfo)
		{
			if (dataOffset < 0 || (data != null && dataSize != 0 && dataOffset >= data.Length) || (data == null && dataOffset != 0))
			{
				throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "must be inside the data buffer");
			}
			if (data != null && dataSize > checked(data.Length - dataOffset) && SetColumnGrbit.SizeLV != (grbit & SetColumnGrbit.SizeLV))
			{
				throw new ArgumentOutOfRangeException("dataSize", dataSize, "cannot be greater than the length of the data (unless the SizeLV option is used)");
			}
			fixed (byte* ptr = data)
			{
				return Api.JetSetColumn(sesid, tableid, columnid, new IntPtr((void*)((byte*)ptr + dataOffset)), dataSize, grbit, setinfo);
			}
		}

		public unsafe static JET_wrn JetRetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data, int dataSize, int dataOffset, out int actualDataSize, RetrieveColumnGrbit grbit, JET_RETINFO retinfo)
		{
			if (dataOffset < 0 || (data != null && dataSize != 0 && dataOffset >= data.Length) || (data == null && dataOffset != 0))
			{
				throw new ArgumentOutOfRangeException("dataOffset", dataOffset, "must be inside the data buffer");
			}
			if ((data == null && dataSize > 0) || (data != null && dataSize > data.Length))
			{
				throw new ArgumentOutOfRangeException("dataSize", dataSize, "cannot be greater than the length of the data");
			}
			fixed (byte* ptr = data)
			{
				return Api.JetRetrieveColumn(sesid, tableid, columnid, new IntPtr((void*)((byte*)ptr + dataOffset)), dataSize, out actualDataSize, grbit, retinfo);
			}
		}

		internal static JET_wrn JetRetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, IntPtr data, int dataSize, out int actualDataSize, RetrieveColumnGrbit grbit, JET_RETINFO retinfo)
		{
			return Api.Check(Api.Impl.JetRetrieveColumn(sesid, tableid, columnid, data, dataSize, out actualDataSize, grbit, retinfo));
		}

		internal static void JetMakeKey(JET_SESID sesid, JET_TABLEID tableid, IntPtr data, int dataSize, MakeKeyGrbit grbit)
		{
			Api.Check(Api.Impl.JetMakeKey(sesid, tableid, data, dataSize, grbit));
		}

		internal static JET_wrn JetSetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, IntPtr data, int dataSize, SetColumnGrbit grbit, JET_SETINFO setinfo)
		{
			return Api.Check(Api.Impl.JetSetColumn(sesid, tableid, columnid, data, dataSize, grbit, setinfo));
		}

		public static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, byte[] data, MakeKeyGrbit grbit)
		{
			if (data == null)
			{
				Api.JetMakeKey(sesid, tableid, null, 0, grbit);
				return;
			}
			if (data.Length == 0)
			{
				Api.JetMakeKey(sesid, tableid, data, data.Length, grbit | MakeKeyGrbit.KeyDataZeroLength);
				return;
			}
			Api.JetMakeKey(sesid, tableid, data, data.Length, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, string data, Encoding encoding, MakeKeyGrbit grbit)
		{
			Api.CheckEncodingIsValid(encoding);
			if (data == null)
			{
				Api.JetMakeKey(sesid, tableid, null, 0, grbit);
				return;
			}
			if (data.Length == 0)
			{
				Api.JetMakeKey(sesid, tableid, null, 0, grbit | MakeKeyGrbit.KeyDataZeroLength);
				return;
			}
			if (Encoding.Unicode == encoding)
			{
				fixed (char* value = data)
				{
					Api.JetMakeKey(sesid, tableid, new IntPtr((void*)value), checked(data.Length * 2), grbit);
				}
				return;
			}
			byte[] array = null;
			try
			{
				array = Caches.ColumnCache.Allocate();
				int bytes;
				try
				{
					fixed (char* chars = data)
					{
						try
						{
							fixed (byte* ptr = array)
							{
								bytes = encoding.GetBytes(chars, data.Length, ptr, array.Length);
							}
						}
						finally
						{
							byte* ptr = null;
						}
					}
				}
				finally
				{
					string text = null;
				}
				Api.JetMakeKey(sesid, tableid, array, bytes, grbit);
			}
			finally
			{
				if (array != null)
				{
					Caches.ColumnCache.Free(ref array);
				}
			}
		}

		public static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, bool data, MakeKeyGrbit grbit)
		{
			byte data2 = data ? byte.MaxValue : 0;
			Api.MakeKey(sesid, tableid, data2, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, byte data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 1, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, short data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 2, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, int data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 4, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, long data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 8, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, Guid data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 16, grbit);
		}

		public static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, DateTime data, MakeKeyGrbit grbit)
		{
			Api.MakeKey(sesid, tableid, data.ToOADate(), grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, float data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 4, grbit);
		}

		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, double data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 8, grbit);
		}

		[CLSCompliant(false)]
		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, ushort data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 2, grbit);
		}

		[CLSCompliant(false)]
		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, uint data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 4, grbit);
		}

		[CLSCompliant(false)]
		public unsafe static void MakeKey(JET_SESID sesid, JET_TABLEID tableid, ulong data, MakeKeyGrbit grbit)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetMakeKey(sesid, tableid, data2, 8, grbit);
		}

		public static bool TryOpenTable(JET_SESID sesid, JET_DBID dbid, string tablename, OpenTableGrbit grbit, out JET_TABLEID tableid)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetOpenTable(sesid, dbid, tablename, null, 0, grbit, out tableid);
			if (JET_err.ObjectNotFound == jet_err)
			{
				return false;
			}
			Api.Check((int)jet_err);
			return true;
		}

		public static IDictionary<string, JET_COLUMNID> GetColumnDictionary(JET_SESID sesid, JET_TABLEID tableid)
		{
			JET_COLUMNLIST jet_COLUMNLIST;
			Api.JetGetTableColumnInfo(sesid, tableid, string.Empty, out jet_COLUMNLIST);
			Encoding encoding = EsentVersion.SupportsVistaFeatures ? Encoding.Unicode : LibraryHelpers.EncodingASCII;
			IDictionary<string, JET_COLUMNID> result;
			try
			{
				Dictionary<string, JET_COLUMNID> dictionary = new Dictionary<string, JET_COLUMNID>(jet_COLUMNLIST.cRecord, StringComparer.OrdinalIgnoreCase);
				if (jet_COLUMNLIST.cRecord > 0 && Api.TryMoveFirst(sesid, jet_COLUMNLIST.tableid))
				{
					do
					{
						string text = Api.RetrieveColumnAsString(sesid, jet_COLUMNLIST.tableid, jet_COLUMNLIST.columnidcolumnname, encoding, RetrieveColumnGrbit.None);
						text = StringCache.TryToIntern(text);
						uint value = Api.RetrieveColumnAsUInt32(sesid, jet_COLUMNLIST.tableid, jet_COLUMNLIST.columnidcolumnid).Value;
						JET_COLUMNID value2 = new JET_COLUMNID
						{
							Value = value
						};
						dictionary.Add(text, value2);
					}
					while (Api.TryMoveNext(sesid, jet_COLUMNLIST.tableid));
				}
				result = dictionary;
			}
			finally
			{
				Api.JetCloseTable(sesid, jet_COLUMNLIST.tableid);
			}
			return result;
		}

		public static JET_COLUMNID GetTableColumnid(JET_SESID sesid, JET_TABLEID tableid, string columnName)
		{
			JET_COLUMNDEF jet_COLUMNDEF;
			Api.JetGetTableColumnInfo(sesid, tableid, columnName, out jet_COLUMNDEF);
			return jet_COLUMNDEF.columnid;
		}

		public static IEnumerable<ColumnInfo> GetTableColumns(JET_SESID sesid, JET_TABLEID tableid)
		{
			return new GenericEnumerable<ColumnInfo>(() => new TableidColumnInfoEnumerator(sesid, tableid));
		}

		public static IEnumerable<ColumnInfo> GetTableColumns(JET_SESID sesid, JET_DBID dbid, string tablename)
		{
			if (tablename == null)
			{
				throw new ArgumentNullException("tablename");
			}
			return new GenericEnumerable<ColumnInfo>(() => new TableColumnInfoEnumerator(sesid, dbid, tablename));
		}

		public static IEnumerable<IndexInfo> GetTableIndexes(JET_SESID sesid, JET_TABLEID tableid)
		{
			return new GenericEnumerable<IndexInfo>(() => new TableidIndexInfoEnumerator(sesid, tableid));
		}

		public static IEnumerable<IndexInfo> GetTableIndexes(JET_SESID sesid, JET_DBID dbid, string tablename)
		{
			if (tablename == null)
			{
				throw new ArgumentNullException("tablename");
			}
			return new GenericEnumerable<IndexInfo>(() => new TableIndexInfoEnumerator(sesid, dbid, tablename));
		}

		public static IEnumerable<string> GetTableNames(JET_SESID sesid, JET_DBID dbid)
		{
			return new GenericEnumerable<string>(() => new TableNameEnumerator(sesid, dbid));
		}

		public static bool TryJetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXID result, JET_IdxInfo infoLevel)
		{
			int num = Api.Impl.JetGetTableIndexInfo(sesid, tableid, indexname, out result, infoLevel);
			if (num == -1404)
			{
				return false;
			}
			Api.Check(num);
			return true;
		}

		public static void MoveBeforeFirst(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.TryMoveFirst(sesid, tableid);
			Api.TryMovePrevious(sesid, tableid);
		}

		public static void MoveAfterLast(JET_SESID sesid, JET_TABLEID tableid)
		{
			Api.TryMoveLast(sesid, tableid);
			Api.TryMoveNext(sesid, tableid);
		}

		public static bool TryMove(JET_SESID sesid, JET_TABLEID tableid, JET_Move move, MoveGrbit grbit)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetMove(sesid, tableid, (int)move, grbit);
			if (JET_err.NoCurrentRecord == jet_err)
			{
				return false;
			}
			Api.Check((int)jet_err);
			return true;
		}

		public static bool TryMoveFirst(JET_SESID sesid, JET_TABLEID tableid)
		{
			return Api.TryMove(sesid, tableid, JET_Move.First, MoveGrbit.None);
		}

		public static bool TryMoveLast(JET_SESID sesid, JET_TABLEID tableid)
		{
			return Api.TryMove(sesid, tableid, JET_Move.Last, MoveGrbit.None);
		}

		public static bool TryMoveNext(JET_SESID sesid, JET_TABLEID tableid)
		{
			return Api.TryMove(sesid, tableid, JET_Move.Next, MoveGrbit.None);
		}

		public static bool TryMovePrevious(JET_SESID sesid, JET_TABLEID tableid)
		{
			return Api.TryMove(sesid, tableid, JET_Move.Previous, MoveGrbit.None);
		}

		public static bool TrySeek(JET_SESID sesid, JET_TABLEID tableid, SeekGrbit grbit)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetSeek(sesid, tableid, grbit);
			if (JET_err.RecordNotFound == jet_err)
			{
				return false;
			}
			Api.Check((int)jet_err);
			return true;
		}

		public static bool TrySetIndexRange(JET_SESID sesid, JET_TABLEID tableid, SetIndexRangeGrbit grbit)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetSetIndexRange(sesid, tableid, grbit);
			if (JET_err.NoCurrentRecord == jet_err)
			{
				return false;
			}
			Api.Check((int)jet_err);
			return true;
		}

		public static void ResetIndexRange(JET_SESID sesid, JET_TABLEID tableid)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetSetIndexRange(sesid, tableid, SetIndexRangeGrbit.RangeRemove);
			if (JET_err.InvalidOperation == jet_err)
			{
				return;
			}
			Api.Check((int)jet_err);
		}

		public static IEnumerable<byte[]> IntersectIndexes(JET_SESID sesid, params JET_TABLEID[] tableids)
		{
			if (tableids == null)
			{
				throw new ArgumentNullException("tableids");
			}
			JET_INDEXRANGE[] ranges = new JET_INDEXRANGE[tableids.Length];
			for (int i = 0; i < tableids.Length; i++)
			{
				ranges[i] = new JET_INDEXRANGE
				{
					tableid = tableids[i]
				};
			}
			return new GenericEnumerable<byte[]>(() => new IntersectIndexesEnumerator(sesid, ranges));
		}

		[Obsolete("Use the overload that takes a JET_IdxInfo parameter, passing in JET_IdxInfo.List")]
		public static void JetGetIndexInfo(JET_SESID sesid, JET_DBID dbid, string tablename, string ignored, out JET_INDEXLIST indexlist)
		{
			Api.JetGetIndexInfo(sesid, dbid, tablename, ignored, out indexlist, JET_IdxInfo.List);
		}

		[Obsolete("Use the overload that takes a JET_IdxInfo parameter, passing in JET_IdxInfo.List")]
		public static void JetGetTableIndexInfo(JET_SESID sesid, JET_TABLEID tableid, string indexname, out JET_INDEXLIST indexlist)
		{
			Api.JetGetTableIndexInfo(sesid, tableid, indexname, out indexlist, JET_IdxInfo.List);
		}

		public static byte[] GetBookmark(JET_SESID sesid, JET_TABLEID tableid)
		{
			byte[] array = null;
			byte[] result;
			try
			{
				array = Caches.BookmarkCache.Allocate();
				int length;
				Api.JetGetBookmark(sesid, tableid, array, array.Length, out length);
				result = MemoryCache.Duplicate(array, length);
			}
			finally
			{
				if (array != null)
				{
					Caches.BookmarkCache.Free(ref array);
				}
			}
			return result;
		}

		public static byte[] GetSecondaryBookmark(JET_SESID sesid, JET_TABLEID tableid, out byte[] primaryBookmark)
		{
			byte[] array = null;
			byte[] array2 = null;
			primaryBookmark = null;
			byte[] result;
			try
			{
				array = Caches.BookmarkCache.Allocate();
				array2 = Caches.SecondaryBookmarkCache.Allocate();
				int length;
				int length2;
				Api.JetGetSecondaryIndexBookmark(sesid, tableid, array2, array2.Length, out length, array, array.Length, out length2, GetSecondaryIndexBookmarkGrbit.None);
				primaryBookmark = MemoryCache.Duplicate(array, length2);
				result = MemoryCache.Duplicate(array2, length);
			}
			finally
			{
				if (array != null)
				{
					Caches.BookmarkCache.Free(ref array);
				}
				if (array2 != null)
				{
					Caches.BookmarkCache.Free(ref array2);
				}
			}
			return result;
		}

		public static byte[] RetrieveKey(JET_SESID sesid, JET_TABLEID tableid, RetrieveKeyGrbit grbit)
		{
			byte[] array = null;
			byte[] result;
			try
			{
				array = Caches.BookmarkCache.Allocate();
				int length;
				Api.JetRetrieveKey(sesid, tableid, array, array.Length, out length, grbit);
				result = MemoryCache.Duplicate(array, length);
			}
			finally
			{
				if (array != null)
				{
					Caches.BookmarkCache.Free(ref array);
				}
			}
			return result;
		}

		public static int? RetrieveColumnSize(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnSize(sesid, tableid, columnid, 1, RetrieveColumnGrbit.None);
		}

		public static int? RetrieveColumnSize(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, int itagSequence, RetrieveColumnGrbit grbit)
		{
			JET_RETINFO retinfo = new JET_RETINFO
			{
				itagSequence = itagSequence
			};
			int value;
			JET_wrn jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, null, 0, out value, grbit, retinfo);
			if (JET_wrn.ColumnNull == jet_wrn)
			{
				return null;
			}
			return new int?(value);
		}

		public static byte[] RetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit, JET_RETINFO retinfo)
		{
			byte[] array = null;
			byte[] array2;
			try
			{
				array = Caches.ColumnCache.Allocate();
				array2 = array;
				int num;
				JET_wrn jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, array2, array2.Length, out num, grbit, retinfo);
				if (JET_wrn.ColumnNull == jet_wrn)
				{
					array2 = null;
				}
				else if (jet_wrn == JET_wrn.Success)
				{
					array2 = MemoryCache.Duplicate(array2, num);
				}
				else
				{
					array2 = new byte[num];
					jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, array2, array2.Length, out num, grbit, retinfo);
					if (jet_wrn != JET_wrn.Success)
					{
						string message = string.Format(CultureInfo.CurrentCulture, "Column size changed from {0} to {1}. The record was probably updated by another thread.", new object[]
						{
							array2.Length,
							num
						});
						throw new InvalidOperationException(message);
					}
				}
			}
			finally
			{
				if (array != null)
				{
					Caches.ColumnCache.Free(ref array);
				}
			}
			return array2;
		}

		public static byte[] RetrieveColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumn(sesid, tableid, columnid, RetrieveColumnGrbit.None, null);
		}

		public static string RetrieveColumnAsString(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsString(sesid, tableid, columnid, Encoding.Unicode, RetrieveColumnGrbit.None);
		}

		public static string RetrieveColumnAsString(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, Encoding encoding)
		{
			return Api.RetrieveColumnAsString(sesid, tableid, columnid, encoding, RetrieveColumnGrbit.None);
		}

		public static string RetrieveColumnAsString(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, Encoding encoding, RetrieveColumnGrbit grbit)
		{
			if (Encoding.Unicode == encoding)
			{
				return Api.RetrieveUnicodeString(sesid, tableid, columnid, grbit);
			}
			byte[] array = null;
			string @string;
			try
			{
				array = Caches.ColumnCache.Allocate();
				byte[] array2 = array;
				int num;
				JET_wrn jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, array2, array2.Length, out num, grbit, null);
				if (JET_wrn.ColumnNull == jet_wrn)
				{
					return null;
				}
				if (JET_wrn.BufferTruncated == jet_wrn)
				{
					array2 = new byte[num];
					jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, array2, array2.Length, out num, grbit, null);
					if (JET_wrn.BufferTruncated == jet_wrn)
					{
						string message = string.Format(CultureInfo.CurrentCulture, "Column size changed from {0} to {1}. The record was probably updated by another thread.", new object[]
						{
							array2.Length,
							num
						});
						throw new InvalidOperationException(message);
					}
				}
				Encoding encoding2 = (encoding is ASCIIEncoding) ? Api.AsciiDecoder : encoding;
				@string = encoding2.GetString(array2, 0, num);
			}
			finally
			{
				if (array != null)
				{
					Caches.ColumnCache.Free(ref array);
				}
			}
			return @string;
		}

		public static short? RetrieveColumnAsInt16(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsInt16(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static short? RetrieveColumnAsInt16(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			short data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 2, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<short>(data2, 2, wrn, actualDataSize);
		}

		public static int? RetrieveColumnAsInt32(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsInt32(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static int? RetrieveColumnAsInt32(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			int data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 4, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<int>(data2, 4, wrn, actualDataSize);
		}

		public static long? RetrieveColumnAsInt64(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsInt64(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static long? RetrieveColumnAsInt64(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			long data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 8, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<long>(data2, 8, wrn, actualDataSize);
		}

		public static float? RetrieveColumnAsFloat(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsFloat(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static float? RetrieveColumnAsFloat(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			float data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 4, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<float>(data2, 4, wrn, actualDataSize);
		}

		public static double? RetrieveColumnAsDouble(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsDouble(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static double? RetrieveColumnAsDouble(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			double data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 8, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<double>(data2, 8, wrn, actualDataSize);
		}

		public static bool? RetrieveColumnAsBoolean(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsBoolean(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public static bool? RetrieveColumnAsBoolean(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			byte? b = Api.RetrieveColumnAsByte(sesid, tableid, columnid, grbit);
			if (b != null)
			{
				return new bool?(0 != b.Value);
			}
			return null;
		}

		public static byte? RetrieveColumnAsByte(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsByte(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static byte? RetrieveColumnAsByte(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			byte data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 1, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<byte>(data2, 1, wrn, actualDataSize);
		}

		public static Guid? RetrieveColumnAsGuid(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsGuid(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public unsafe static Guid? RetrieveColumnAsGuid(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			Guid data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 16, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<Guid>(data2, 16, wrn, actualDataSize);
		}

		public static DateTime? RetrieveColumnAsDateTime(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsDateTime(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public static DateTime? RetrieveColumnAsDateTime(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			double? num = Api.RetrieveColumnAsDouble(sesid, tableid, columnid, grbit);
			if (num != null)
			{
				return new DateTime?(Conversions.ConvertDoubleToDateTime(num.Value));
			}
			return null;
		}

		[CLSCompliant(false)]
		public static ushort? RetrieveColumnAsUInt16(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsUInt16(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		[CLSCompliant(false)]
		public unsafe static ushort? RetrieveColumnAsUInt16(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			ushort data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 2, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<ushort>(data2, 2, wrn, actualDataSize);
		}

		[CLSCompliant(false)]
		public static uint? RetrieveColumnAsUInt32(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsUInt32(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		[CLSCompliant(false)]
		public unsafe static uint? RetrieveColumnAsUInt32(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			uint data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 4, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<uint>(data2, 4, wrn, actualDataSize);
		}

		[CLSCompliant(false)]
		public static ulong? RetrieveColumnAsUInt64(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.RetrieveColumnAsUInt64(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		[CLSCompliant(false)]
		public unsafe static ulong? RetrieveColumnAsUInt64(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			ulong data2;
			IntPtr data = new IntPtr((void*)(&data2));
			int actualDataSize;
			JET_wrn wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, data, 8, out actualDataSize, grbit, null);
			return Api.CreateReturnValue<ulong>(data2, 8, wrn, actualDataSize);
		}

		public static object DeserializeObjectFromColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			return Api.DeserializeObjectFromColumn(sesid, tableid, columnid, RetrieveColumnGrbit.None);
		}

		public static object DeserializeObjectFromColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			int num;
			if (JET_wrn.ColumnNull == Api.JetRetrieveColumn(sesid, tableid, columnid, null, 0, out num, grbit, null))
			{
				return null;
			}
			object result;
			using (ColumnStream columnStream = new ColumnStream(sesid, tableid, columnid))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				result = binaryFormatter.Deserialize(columnStream);
			}
			return result;
		}

		public static void RetrieveColumns(JET_SESID sesid, JET_TABLEID tableid, params ColumnValue[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentOutOfRangeException("values", values.Length, "must have at least one value");
			}
			ColumnValue.RetrieveColumns(sesid, tableid, values);
		}

		private static T? CreateReturnValue<T>(T data, int dataSize, JET_wrn wrn, int actualDataSize) where T : struct
		{
			if (JET_wrn.ColumnNull == wrn)
			{
				return null;
			}
			Api.CheckDataSize(dataSize, actualDataSize);
			return new T?(data);
		}

		private static void CheckDataSize(int expectedDataSize, int actualDataSize)
		{
			if (actualDataSize < expectedDataSize)
			{
				throw new EsentInvalidColumnException();
			}
		}

		private unsafe static int PinColumnsAndRetrieve(JET_SESID sesid, JET_TABLEID tableid, NATIVE_RETRIEVECOLUMN* nativeretrievecolumns, IList<JET_RETRIEVECOLUMN> retrievecolumns, int numColumns, int i)
		{
			fixed (byte* pvData = retrievecolumns[i].pvData)
			{
				do
				{
					retrievecolumns[i].CheckDataSize();
					retrievecolumns[i].GetNativeRetrievecolumn(ref nativeretrievecolumns[i]);
					nativeretrievecolumns[i].pvData = new IntPtr((void*)((byte*)pvData + retrievecolumns[i].ibData));
					i++;
				}
				while (i < numColumns && retrievecolumns[i].pvData == retrievecolumns[i - 1].pvData);
				return (i == numColumns) ? Api.Impl.JetRetrieveColumns(sesid, tableid, nativeretrievecolumns, numColumns) : Api.PinColumnsAndRetrieve(sesid, tableid, nativeretrievecolumns, retrievecolumns, numColumns, i);
			}
		}

		private unsafe static string RetrieveUnicodeString(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, RetrieveColumnGrbit grbit)
		{
			char* value = stackalloc char[checked(unchecked((UIntPtr)512) * 2)];
			int num;
			JET_wrn jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, new IntPtr((void*)value), 1024, out num, grbit, null);
			if (JET_wrn.ColumnNull == jet_wrn)
			{
				return null;
			}
			if (jet_wrn == JET_wrn.Success)
			{
				return new string(value, 0, num / 2);
			}
			string text = new string('\0', num / 2);
			fixed (char* value2 = text)
			{
				int num2;
				jet_wrn = Api.JetRetrieveColumn(sesid, tableid, columnid, new IntPtr((void*)value2), num, out num2, grbit, null);
				if (JET_wrn.BufferTruncated == jet_wrn)
				{
					string message = string.Format(CultureInfo.CurrentCulture, "Column size changed from {0} to {1}. The record was probably updated by another thread.", new object[]
					{
						num,
						num2
					});
					throw new InvalidOperationException(message);
				}
			}
			return text;
		}

		public static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, string data, Encoding encoding)
		{
			Api.SetColumn(sesid, tableid, columnid, data, encoding, SetColumnGrbit.None);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, string data, Encoding encoding, SetColumnGrbit grbit)
		{
			Api.CheckEncodingIsValid(encoding);
			if (data == null)
			{
				Api.JetSetColumn(sesid, tableid, columnid, null, 0, grbit, null);
				return;
			}
			if (data.Length == 0)
			{
				Api.JetSetColumn(sesid, tableid, columnid, null, 0, grbit | SetColumnGrbit.ZeroLength, null);
				return;
			}
			if (Encoding.Unicode == encoding)
			{
				fixed (char* value = data)
				{
					Api.JetSetColumn(sesid, tableid, columnid, new IntPtr((void*)value), checked(data.Length * 2), grbit, null);
				}
				return;
			}
			if (encoding.GetMaxByteCount(data.Length) <= Caches.ColumnCache.BufferSize)
			{
				byte[] array = null;
				try
				{
					array = Caches.ColumnCache.Allocate();
					try
					{
						fixed (char* chars = data)
						{
							try
							{
								fixed (byte* ptr = array)
								{
									int bytes = encoding.GetBytes(chars, data.Length, ptr, array.Length);
									Api.JetSetColumn(sesid, tableid, columnid, new IntPtr((void*)ptr), bytes, grbit, null);
								}
							}
							finally
							{
								byte* ptr = null;
							}
						}
					}
					finally
					{
						string text = null;
					}
					return;
				}
				finally
				{
					if (array != null)
					{
						Caches.ColumnCache.Free(ref array);
					}
				}
			}
			byte[] bytes2 = encoding.GetBytes(data);
			Api.JetSetColumn(sesid, tableid, columnid, bytes2, bytes2.Length, grbit, null);
		}

		public static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data)
		{
			Api.SetColumn(sesid, tableid, columnid, data, SetColumnGrbit.None);
		}

		public static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte[] data, SetColumnGrbit grbit)
		{
			if (data != null && data.Length == 0)
			{
				grbit |= SetColumnGrbit.ZeroLength;
			}
			int dataSize = (data == null) ? 0 : data.Length;
			Api.JetSetColumn(sesid, tableid, columnid, data, dataSize, grbit, null);
		}

		public static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, bool data)
		{
			byte data2 = data ? byte.MaxValue : 0;
			Api.SetColumn(sesid, tableid, columnid, data2);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, byte data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 1, SetColumnGrbit.None, null);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, short data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 2, SetColumnGrbit.None, null);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, int data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 4, SetColumnGrbit.None, null);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, long data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 8, SetColumnGrbit.None, null);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, Guid data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 16, SetColumnGrbit.None, null);
		}

		public static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, DateTime data)
		{
			Api.SetColumn(sesid, tableid, columnid, data.ToOADate());
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, float data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 4, SetColumnGrbit.None, null);
		}

		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, double data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 8, SetColumnGrbit.None, null);
		}

		public static int EscrowUpdate(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, int delta)
		{
			byte[] array = new byte[4];
			int num;
			Api.JetEscrowUpdate(sesid, tableid, columnid, BitConverter.GetBytes(delta), 4, array, array.Length, out num, EscrowUpdateGrbit.None);
			return BitConverter.ToInt32(array, 0);
		}

		[CLSCompliant(false)]
		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, ushort data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 2, SetColumnGrbit.None, null);
		}

		[CLSCompliant(false)]
		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, uint data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 4, SetColumnGrbit.None, null);
		}

		[CLSCompliant(false)]
		public unsafe static void SetColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, ulong data)
		{
			IntPtr data2 = new IntPtr((void*)(&data));
			Api.JetSetColumn(sesid, tableid, columnid, data2, 8, SetColumnGrbit.None, null);
		}

		public static void SerializeObjectToColumn(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, object value)
		{
			if (value == null)
			{
				Api.SetColumn(sesid, tableid, columnid, null);
				return;
			}
			using (ColumnStream columnStream = new ColumnStream(sesid, tableid, columnid))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter
				{
					Context = new StreamingContext(StreamingContextStates.Persistence)
				};
				binaryFormatter.Serialize(columnStream, value);
			}
		}

		public unsafe static void SetColumns(JET_SESID sesid, JET_TABLEID tableid, params ColumnValue[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentOutOfRangeException("values", values.Length, "must have at least one value");
			}
			NATIVE_SETCOLUMN* nativeColumns = stackalloc NATIVE_SETCOLUMN[checked(unchecked((UIntPtr)values.Length) * (UIntPtr)sizeof(NATIVE_SETCOLUMN))];
			Api.Check(values[0].SetColumns(sesid, tableid, values, nativeColumns, 0));
		}

		private static void CheckEncodingIsValid(Encoding encoding)
		{
			int codePage = encoding.CodePage;
			if (20127 != codePage && 1200 != codePage)
			{
				throw new ArgumentOutOfRangeException("encoding", codePage, "Invalid Encoding type. Only ASCII and Unicode encodings are allowed");
			}
		}

		private static readonly Encoding AsciiDecoder = new UTF8Encoding(false, true);

		internal delegate void ErrorHandler(JET_err error);
	}
}
