using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum ColInfoGrbit
	{
		None = 0,
		NonDerivedColumnsOnly = -2147483648,
		MinimalInfo = 1073741824,
		SortByColumnid = 536870912
	}
}
