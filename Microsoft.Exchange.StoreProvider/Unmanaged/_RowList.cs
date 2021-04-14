using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _RowList
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_RowList));

		internal int cEntries;

		internal _RowEntry aEntries;
	}
}
