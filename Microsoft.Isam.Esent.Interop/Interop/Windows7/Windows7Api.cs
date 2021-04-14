using System;

namespace Microsoft.Isam.Esent.Interop.Windows7
{
	public static class Windows7Api
	{
		public static void JetConfigureProcessForCrashDump(CrashDumpGrbit grbit)
		{
			Api.Check(Api.Impl.JetConfigureProcessForCrashDump(grbit));
		}

		public static void JetPrereadKeys(JET_SESID sesid, JET_TABLEID tableid, byte[][] keys, int[] keyLengths, int keyIndex, int keyCount, out int keysPreread, PrereadKeysGrbit grbit)
		{
			Api.Check(Api.Impl.JetPrereadKeys(sesid, tableid, keys, keyLengths, keyIndex, keyCount, out keysPreread, grbit));
		}

		public static void JetPrereadKeys(JET_SESID sesid, JET_TABLEID tableid, byte[][] keys, int[] keyLengths, int keyCount, out int keysPreread, PrereadKeysGrbit grbit)
		{
			Windows7Api.JetPrereadKeys(sesid, tableid, keys, keyLengths, 0, keyCount, out keysPreread, grbit);
		}
	}
}
