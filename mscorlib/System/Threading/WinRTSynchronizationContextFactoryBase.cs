using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Threading
{
	[FriendAccessAllowed]
	[SecurityCritical]
	internal class WinRTSynchronizationContextFactoryBase
	{
		[SecurityCritical]
		public virtual SynchronizationContext Create(object coreDispatcher)
		{
			return null;
		}
	}
}
