using System;

namespace System.Security.AccessControl
{
	[Flags]
	public enum RegistryRights
	{
		QueryValues = 1,
		SetValue = 2,
		CreateSubKey = 4,
		EnumerateSubKeys = 8,
		Notify = 16,
		CreateLink = 32,
		ExecuteKey = 131097,
		ReadKey = 131097,
		WriteKey = 131078,
		Delete = 65536,
		ReadPermissions = 131072,
		ChangePermissions = 262144,
		TakeOwnership = 524288,
		FullControl = 983103
	}
}
