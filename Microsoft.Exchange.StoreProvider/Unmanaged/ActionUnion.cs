using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct ActionUnion
	{
		[FieldOffset(0)]
		internal _MoveCopyAction actMoveCopy;

		[FieldOffset(0)]
		internal _ReplyAction actReply;

		[FieldOffset(0)]
		internal _DeferAction actDeferAction;

		[FieldOffset(0)]
		internal uint scBounceCode;

		[FieldOffset(0)]
		internal unsafe _AdrList* lpadrlist;

		[FieldOffset(0)]
		internal SPropValue propTag;
	}
}
