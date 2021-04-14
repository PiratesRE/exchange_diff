using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal
{
	[ComVisible(false)]
	public static class InternalApplicationIdentityHelper
	{
		[SecurityCritical]
		public static object GetInternalAppId(ApplicationIdentity id)
		{
			return id.Identity;
		}
	}
}
