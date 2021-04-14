using System;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	public static class VistaApi
	{
		public static void JetGetColumnInfo(JET_SESID sesid, JET_DBID dbid, string tablename, JET_COLUMNID columnid, out JET_COLUMNBASE columnbase)
		{
			Api.Check(Api.Impl.JetGetColumnInfo(sesid, dbid, tablename, columnid, out columnbase));
		}

		public static void JetGetTableColumnInfo(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid, out JET_COLUMNBASE columnbase)
		{
			Api.Check(Api.Impl.JetGetTableColumnInfo(sesid, tableid, columnid, out columnbase));
		}

		public static void JetOpenTemporaryTable(JET_SESID sesid, JET_OPENTEMPORARYTABLE temporarytable)
		{
			Api.Check(Api.Impl.JetOpenTemporaryTable(sesid, temporarytable));
		}

		public static void JetGetThreadStats(out JET_THREADSTATS threadstats)
		{
			Api.Check(Api.Impl.JetGetThreadStats(out threadstats));
		}

		public static void JetOSSnapshotPrepareInstance(JET_OSSNAPID snapshot, JET_INSTANCE instance, SnapshotPrepareInstanceGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotPrepareInstance(snapshot, instance, grbit));
		}

		public static void JetOSSnapshotTruncateLog(JET_OSSNAPID snapshot, SnapshotTruncateLogGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotTruncateLog(snapshot, grbit));
		}

		public static void JetOSSnapshotTruncateLogInstance(JET_OSSNAPID snapshot, JET_INSTANCE instance, SnapshotTruncateLogGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotTruncateLogInstance(snapshot, instance, grbit));
		}

		public static void JetOSSnapshotGetFreezeInfo(JET_OSSNAPID snapshot, out int numInstances, out JET_INSTANCE_INFO[] instances, SnapshotGetFreezeInfoGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotGetFreezeInfo(snapshot, out numInstances, out instances, grbit));
		}

		public static void JetOSSnapshotEnd(JET_OSSNAPID snapshot, SnapshotEndGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotEnd(snapshot, grbit));
		}

		public static void JetGetInstanceMiscInfo(JET_INSTANCE instance, out JET_SIGNATURE signature, JET_InstanceMiscInfo infoLevel)
		{
			Api.Check(Api.Impl.JetGetInstanceMiscInfo(instance, out signature, infoLevel));
		}

		public static JET_wrn JetInit3(ref JET_INSTANCE instance, JET_RSTINFO recoveryOptions, InitGrbit grbit)
		{
			return Api.Check(Api.Impl.JetInit3(ref instance, recoveryOptions, grbit));
		}

		public static void JetGetRecordSize(JET_SESID sesid, JET_TABLEID tableid, ref JET_RECSIZE recsize, GetRecordSizeGrbit grbit)
		{
			Api.Check(Api.Impl.JetGetRecordSize(sesid, tableid, ref recsize, grbit));
		}
	}
}
