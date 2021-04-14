using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum ObjectInfoFlags
	{
		None = 0,
		System = -2147483648,
		TableFixedDDL = 1073741824,
		TableTemplate = 536870912,
		TableDerived = 268435456,
		TableNoFixedVarColumnsInDerivedTables = 67108864
	}
}
