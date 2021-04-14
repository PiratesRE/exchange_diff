using System;

namespace Microsoft.Isam.Esent.Interop
{
	public enum JET_dbstate
	{
		JustCreated = 1,
		DirtyShutdown,
		CleanShutdown,
		BeingConverted,
		ForceDetach
	}
}
