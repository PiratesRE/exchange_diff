using System;

namespace System.Security.AccessControl
{
	[Flags]
	public enum MutexRights
	{
		Modify = 1,
		Delete = 65536,
		ReadPermissions = 131072,
		ChangePermissions = 262144,
		TakeOwnership = 524288,
		Synchronize = 1048576,
		FullControl = 2031617
	}
}
