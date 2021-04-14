using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _AdrEntry
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_AdrEntry));

		internal int ulReserved1;

		internal int cValues;

		internal unsafe SPropValue* pspva;
	}
}
