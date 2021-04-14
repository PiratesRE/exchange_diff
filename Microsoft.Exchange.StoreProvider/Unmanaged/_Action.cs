using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _Action
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_Action));

		internal uint ActType;

		internal uint ActFlavor;

		internal IntPtr Zero1;

		internal IntPtr Zero2;

		internal uint ulFlags;

		internal uint ulPad;

		internal ActionUnion union;
	}
}
