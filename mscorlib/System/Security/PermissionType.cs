using System;

namespace System.Security
{
	[Serializable]
	internal enum PermissionType
	{
		SecurityUnmngdCodeAccess,
		SecuritySkipVerification,
		ReflectionTypeInfo,
		SecurityAssert,
		ReflectionMemberAccess,
		SecuritySerialization,
		ReflectionRestrictedMemberAccess,
		FullTrust,
		SecurityBindingRedirects,
		UIPermission,
		EnvironmentPermission,
		FileDialogPermission,
		FileIOPermission,
		ReflectionPermission,
		SecurityPermission,
		SecurityControlEvidence = 16,
		SecurityControlPrincipal
	}
}
