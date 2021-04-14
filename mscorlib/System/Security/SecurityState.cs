using System;
using System.Security.Permissions;

namespace System.Security
{
	[SecurityCritical]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	public abstract class SecurityState
	{
		[SecurityCritical]
		public bool IsStateAvailable()
		{
			AppDomainManager currentAppDomainManager = AppDomainManager.CurrentAppDomainManager;
			return currentAppDomainManager != null && currentAppDomainManager.CheckSecuritySettings(this);
		}

		public abstract void EnsureState();
	}
}
