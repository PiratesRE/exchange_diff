using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct SNspiUnionRestriction
	{
		[FieldOffset(0)]
		internal SNspiAndOrNotRestriction resAnd;

		[FieldOffset(0)]
		internal SContentRestriction resContent;

		[FieldOffset(0)]
		internal SPropertyRestriction resProperty;

		[FieldOffset(0)]
		internal SExistRestriction resExist;
	}
}
