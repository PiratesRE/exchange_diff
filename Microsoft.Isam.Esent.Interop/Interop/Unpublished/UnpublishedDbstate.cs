using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public static class UnpublishedDbstate
	{
		public const JET_dbstate IncrementalReseedInProgress = (JET_dbstate)6;

		public const JET_dbstate DirtyAndPatchedShutdown = (JET_dbstate)7;
	}
}
