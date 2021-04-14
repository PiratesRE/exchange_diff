using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public enum JET_RECOVERYACTIONS
	{
		MissingLogMustFail = 1,
		MissingLogContinueToRedo,
		MissingLogContinueTryCurrentLog,
		MissingLogContinueToUndo,
		MissingLogCreateNewLogStream
	}
}
