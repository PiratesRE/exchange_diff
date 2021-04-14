using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum CreateTableColumnIndexGrbit
	{
		None = 0,
		FixedDDL = 1,
		TemplateTable = 2,
		NoFixedVarColumnsInDerivedTables = 4
	}
}
