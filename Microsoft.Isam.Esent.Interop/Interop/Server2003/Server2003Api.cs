using System;

namespace Microsoft.Isam.Esent.Interop.Server2003
{
	public static class Server2003Api
	{
		public static void JetOSSnapshotAbort(JET_OSSNAPID snapid, SnapshotAbortGrbit grbit)
		{
			Api.Check(Api.Impl.JetOSSnapshotAbort(snapid, grbit));
		}

		public static void JetUpdate2(JET_SESID sesid, JET_TABLEID tableid, byte[] bookmark, int bookmarkSize, out int actualBookmarkSize, UpdateGrbit grbit)
		{
			Api.Check(Api.Impl.JetUpdate2(sesid, tableid, bookmark, bookmarkSize, out actualBookmarkSize, grbit));
		}
	}
}
