﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace System.StubHelpers
{
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	internal static class ObjectMarshaler
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ConvertToNative(object objSrc, IntPtr pDstVariant);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object ConvertToManaged(IntPtr pSrcVariant);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearNative(IntPtr pVariant);
	}
}
