using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SSortOrder
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(SSortOrder));

		internal int ulPropTag;

		internal int ulOrder;
	}
}
