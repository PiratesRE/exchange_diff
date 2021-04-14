using System;

namespace Microsoft.Isam.Esent.Interop.Server2003
{
	[Flags]
	public enum UpdateGrbit
	{
		None = 0,
		[Obsolete("Only needed for legacy replication applications.")]
		CheckESE97Compatibility = 1
	}
}
