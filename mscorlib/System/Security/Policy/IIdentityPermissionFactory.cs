using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public interface IIdentityPermissionFactory
	{
		IPermission CreateIdentityPermission(Evidence evidence);
	}
}
