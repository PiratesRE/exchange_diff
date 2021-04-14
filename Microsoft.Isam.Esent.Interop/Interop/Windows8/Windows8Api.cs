using System;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public static class Windows8Api
	{
		public static void JetStopServiceInstance2(JET_INSTANCE instance, StopServiceGrbit grbit)
		{
			Api.Check(Api.Impl.JetStopServiceInstance2(instance, grbit));
		}

		public static void JetBeginTransaction3(JET_SESID sesid, long userTransactionId, BeginTransactionGrbit grbit)
		{
			Api.Check(Api.Impl.JetBeginTransaction3(sesid, userTransactionId, grbit));
		}

		public static void JetGetErrorInfo(JET_err error, out JET_ERRINFOBASIC errinfo)
		{
			Api.Check(Api.Impl.JetGetErrorInfo(error, out errinfo));
		}

		public static void JetResizeDatabase(JET_SESID sesid, JET_DBID dbid, int desiredPages, out int actualPages, ResizeDatabaseGrbit grbit)
		{
			Api.Check(Api.Impl.JetResizeDatabase(sesid, dbid, desiredPages, out actualPages, grbit));
		}

		public static void JetCreateIndex4(JET_SESID sesid, JET_TABLEID tableid, JET_INDEXCREATE[] indexcreates, int numIndexCreates)
		{
			Api.Check(Api.Impl.JetCreateIndex4(sesid, tableid, indexcreates, numIndexCreates));
		}

		public static void JetOpenTemporaryTable2(JET_SESID sesid, JET_OPENTEMPORARYTABLE temporarytable)
		{
			Api.Check(Api.Impl.JetOpenTemporaryTable2(sesid, temporarytable));
		}

		public static void JetCreateTableColumnIndex4(JET_SESID sesid, JET_DBID dbid, JET_TABLECREATE tablecreate)
		{
			Api.Check(Api.Impl.JetCreateTableColumnIndex4(sesid, dbid, tablecreate));
		}

		public static void JetSetSessionParameter(JET_SESID sesid, JET_sesparam sesparamid, byte[] data, int dataSize)
		{
			Api.Check(Api.Impl.JetSetSessionParameter(sesid, sesparamid, data, dataSize));
		}

		public static void JetCommitTransaction2(JET_SESID sesid, CommitTransactionGrbit grbit, TimeSpan durableCommit, out JET_COMMIT_ID commitId)
		{
			Api.Check(Api.Impl.JetCommitTransaction2(sesid, grbit, durableCommit, out commitId));
		}

		public static bool JetTryPrereadIndexRanges(JET_SESID sesid, JET_TABLEID tableid, JET_INDEX_RANGE[] indexRanges, int rangeIndex, int rangeCount, out int rangesPreread, JET_COLUMNID[] columnsPreread, PrereadIndexRangesGrbit grbit)
		{
			JET_err jet_err = (JET_err)Api.Impl.JetPrereadIndexRanges(sesid, tableid, indexRanges, rangeIndex, rangeCount, out rangesPreread, columnsPreread, grbit);
			return jet_err >= JET_err.Success;
		}

		public static void JetPrereadIndexRanges(JET_SESID sesid, JET_TABLEID tableid, JET_INDEX_RANGE[] indexRanges, int rangeIndex, int rangeCount, out int rangesPreread, JET_COLUMNID[] columnsPreread, PrereadIndexRangesGrbit grbit)
		{
			Api.Check(Api.Impl.JetPrereadIndexRanges(sesid, tableid, indexRanges, rangeIndex, rangeCount, out rangesPreread, columnsPreread, grbit));
		}

		public static void PrereadKeyRanges(JET_SESID sesid, JET_TABLEID tableid, byte[][] keysStart, int[] keyStartLengths, byte[][] keysEnd, int[] keyEndLengths, int rangeIndex, int rangeCount, out int rangesPreread, JET_COLUMNID[] columnsPreread, PrereadIndexRangesGrbit grbit)
		{
			Api.Check(Api.Impl.JetPrereadKeyRanges(sesid, tableid, keysStart, keyStartLengths, keysEnd, keyEndLengths, rangeIndex, rangeCount, out rangesPreread, columnsPreread, grbit));
		}

		public static void JetSetCursorFilter(JET_SESID sesid, JET_TABLEID tableid, JET_INDEX_COLUMN[] filters, CursorFilterGrbit grbit)
		{
			Api.Check(Api.Impl.JetSetCursorFilter(sesid, tableid, filters, grbit));
		}
	}
}
