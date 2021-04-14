using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	internal delegate JET_err NATIVE_JET_PFNDURABLECOMMITCALLBACK(IntPtr instance, ref NATIVE_COMMIT_ID pCommitIdSeen, uint grbit);
}
