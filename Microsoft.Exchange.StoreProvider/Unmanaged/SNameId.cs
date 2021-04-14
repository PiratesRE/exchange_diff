using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SNameId
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(SNameId));

		internal IntPtr lpGuid;

		internal int ulKind;

		internal SUnionNameId union;
	}
}
