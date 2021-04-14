using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SSortOrderSet
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(SSortOrderSet));

		internal int cSorts;

		internal int cCategories;

		internal int cExpanded;

		internal SSortOrder aSorts;
	}
}
