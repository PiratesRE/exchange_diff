using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	public interface IUnrestrictedPermission
	{
		bool IsUnrestricted();
	}
}
