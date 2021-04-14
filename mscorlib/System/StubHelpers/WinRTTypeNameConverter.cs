using System;
using System.Runtime.CompilerServices;

namespace System.StubHelpers
{
	internal static class WinRTTypeNameConverter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string ConvertToWinRTTypeName(Type managedType, out bool isPrimitive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type GetTypeFromWinRTTypeName(string typeName, out bool isPrimitive);
	}
}
