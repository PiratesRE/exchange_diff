using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SafeExInterfaceHandleExtensions
	{
		internal static void DisposeIfValid(this SafeExInterfaceHandle safeExInterfaceHandle)
		{
			if (safeExInterfaceHandle != null)
			{
				safeExInterfaceHandle.Dispose();
				safeExInterfaceHandle = null;
			}
		}
	}
}
