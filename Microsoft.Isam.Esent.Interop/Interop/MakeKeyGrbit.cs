using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum MakeKeyGrbit
	{
		None = 0,
		NewKey = 1,
		NormalizedKey = 8,
		KeyDataZeroLength = 16,
		StrLimit = 2,
		SubStrLimit = 4,
		FullColumnStartLimit = 256,
		FullColumnEndLimit = 512,
		PartialColumnStartLimit = 1024,
		PartialColumnEndLimit = 2048
	}
}
