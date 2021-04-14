using System;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public delegate JET_err JET_PFNDURABLECOMMITCALLBACK(JET_INSTANCE instance, JET_COMMIT_ID pCommitIdSeen, DurableCommitCallbackGrbit grbit);
}
