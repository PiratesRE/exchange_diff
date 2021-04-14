using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum CompactGrbit
	{
		None = 0,
		Stats = 32,
		[Obsolete("Use esentutl repair functionality instead.")]
		Repair = 64
	}
}
