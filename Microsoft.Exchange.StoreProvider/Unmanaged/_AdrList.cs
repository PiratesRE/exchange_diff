using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _AdrList
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_AdrList));

		internal int cEntries;

		internal _AdrEntry adrEntry1;
	}
}
