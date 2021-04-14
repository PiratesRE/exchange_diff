using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum RetrieveColumnGrbit
	{
		None = 0,
		RetrieveCopy = 1,
		RetrieveFromIndex = 2,
		RetrieveFromPrimaryBookmark = 4,
		RetrieveTag = 8,
		RetrieveNull = 16,
		RetrieveIgnoreDefault = 32
	}
}
