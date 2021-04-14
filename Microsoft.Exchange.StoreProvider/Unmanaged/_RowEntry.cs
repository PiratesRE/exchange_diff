using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _RowEntry
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_RowEntry));

		internal int ulRowFlags;

		internal int cValues;

		internal unsafe SPropValue* rgPropVals;
	}
}
