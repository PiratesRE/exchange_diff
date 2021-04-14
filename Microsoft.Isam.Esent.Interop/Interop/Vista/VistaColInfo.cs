using System;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	public static class VistaColInfo
	{
		internal const JET_ColInfo BaseByColid = (JET_ColInfo)8;

		internal const JET_ColInfo GrbitNonDerivedColumnsOnly = (JET_ColInfo)(-2147483648);

		internal const JET_ColInfo GrbitMinimalInfo = (JET_ColInfo)1073741824;

		internal const JET_ColInfo GrbitSortByColumnid = (JET_ColInfo)536870912;
	}
}
