using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct SUnionRestriction
	{
		[FieldOffset(0)]
		internal SComparePropsRestriction resCompareProps;

		[FieldOffset(0)]
		internal SAndOrNotRestriction resAnd;

		[FieldOffset(0)]
		internal SContentRestriction resContent;

		[FieldOffset(0)]
		internal SPropertyRestriction resProperty;

		[FieldOffset(0)]
		internal SBitMaskRestriction resBitMask;

		[FieldOffset(0)]
		internal SSizeRestriction resSize;

		[FieldOffset(0)]
		internal SExistRestriction resExist;

		[FieldOffset(0)]
		internal SSubRestriction resSub;

		[FieldOffset(0)]
		internal SCommentRestriction resComment;

		[FieldOffset(0)]
		internal SCountRestriction resCount;

		[FieldOffset(0)]
		internal SNearRestriction resNear;
	}
}
