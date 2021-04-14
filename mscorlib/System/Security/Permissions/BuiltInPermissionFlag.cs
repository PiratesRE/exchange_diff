using System;

namespace System.Security.Permissions
{
	[Serializable]
	internal enum BuiltInPermissionFlag
	{
		EnvironmentPermission = 1,
		FileDialogPermission,
		FileIOPermission = 4,
		IsolatedStorageFilePermission = 8,
		ReflectionPermission = 16,
		RegistryPermission = 32,
		SecurityPermission = 64,
		UIPermission = 128,
		PrincipalPermission = 256,
		PublisherIdentityPermission = 512,
		SiteIdentityPermission = 1024,
		StrongNameIdentityPermission = 2048,
		UrlIdentityPermission = 4096,
		ZoneIdentityPermission = 8192,
		KeyContainerPermission = 16384
	}
}
