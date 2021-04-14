using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	[FriendAccessAllowed]
	internal class CLRConfig
	{
		[FriendAccessAllowed]
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CheckLegacyManagedDeflateStream();

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CheckThrowUnobservedTaskExceptions();
	}
}
