using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum CreateIndexGrbit
	{
		None = 0,
		IndexUnique = 1,
		IndexPrimary = 2,
		IndexDisallowNull = 4,
		IndexIgnoreNull = 8,
		IndexIgnoreAnyNull = 32,
		IndexIgnoreFirstNull = 64,
		IndexLazyFlush = 128,
		IndexEmpty = 256,
		IndexUnversioned = 512,
		IndexSortNullsHigh = 1024
	}
}
