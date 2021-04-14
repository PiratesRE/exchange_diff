using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum EnumerateColumnsGrbit
	{
		None = 0,
		EnumerateCompressOutput = 524288,
		EnumerateCopy = 1,
		EnumerateIgnoreDefault = 32,
		EnumeratePresenceOnly = 131072,
		EnumerateTaggedOnly = 262144
	}
}
