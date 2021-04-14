using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct THREAD_DIAG_CONTEXT
	{
		internal long pNextCtx;

		internal uint dwTid;

		internal uint dwRequestId;

		internal long dtFirstEvent;

		internal byte fDataOverflow;

		internal byte fCircularBuffering;

		internal uint dwDataSize;

		internal uint dwSegm2Beg;

		internal uint dwSegm2Len;

		[FixedBuffer(typeof(byte), 512)]
		internal THREAD_DIAG_CONTEXT.<byteData>e__FixedBuffer0 byteData;

		[UnsafeValueType]
		[CompilerGenerated]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <byteData>e__FixedBuffer0
		{
			public byte FixedElementField;
		}
	}
}
