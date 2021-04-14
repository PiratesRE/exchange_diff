using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _ReadState
	{
		public static readonly int SizeOf = Marshal.SizeOf(typeof(_ReadState));

		internal int cbSourceKey;

		internal unsafe byte* pbSourceKey;

		internal int ulFlags;
	}
}
