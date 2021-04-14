using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SNspiRestriction
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(SNspiRestriction));

		internal int rt;

		internal SNspiUnionRestriction union;
	}
}
