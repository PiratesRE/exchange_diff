using System;

namespace System.Security.AccessControl
{
	[Flags]
	public enum EventWaitHandleRights
	{
		Modify = 2,
		Delete = 65536,
		ReadPermissions = 131072,
		ChangePermissions = 262144,
		TakeOwnership = 524288,
		Synchronize = 1048576,
		FullControl = 2031619
	}
}
