using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal class RoleEntryComparer : IComparer<RoleEntry>
	{
		public int Compare(RoleEntry a, RoleEntry b)
		{
			return RoleEntry.CompareRoleEntriesByName(a, b);
		}

		public static readonly RoleEntryComparer Instance = new RoleEntryComparer();
	}
}
