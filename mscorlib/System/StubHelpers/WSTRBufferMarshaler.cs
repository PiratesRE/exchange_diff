using System;
using System.Runtime.ConstrainedExecution;

namespace System.StubHelpers
{
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	internal static class WSTRBufferMarshaler
	{
		internal static IntPtr ConvertToNative(string strManaged)
		{
			return IntPtr.Zero;
		}

		internal static string ConvertToManaged(IntPtr bstr)
		{
			return null;
		}

		internal static void ClearNative(IntPtr pNative)
		{
		}
	}
}
