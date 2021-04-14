using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SRestriction
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(SRestriction));

		internal int rt;

		internal SUnionRestriction union;
	}
}
