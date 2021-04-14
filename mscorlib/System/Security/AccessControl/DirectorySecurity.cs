using System;
using System.IO;
using System.Security.Permissions;

namespace System.Security.AccessControl
{
	public sealed class DirectorySecurity : FileSystemSecurity
	{
		[SecuritySafeCritical]
		public DirectorySecurity() : base(true)
		{
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public DirectorySecurity(string name, AccessControlSections includeSections) : base(true, name, includeSections, true)
		{
			string fullPathInternal = Path.GetFullPathInternal(name);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.NoAccess, AccessControlActions.View, fullPathInternal, false, false);
		}
	}
}
