using System;

namespace System.Security.AccessControl
{
	[Flags]
	public enum AceFlags : byte
	{
		None = 0,
		ObjectInherit = 1,
		ContainerInherit = 2,
		NoPropagateInherit = 4,
		InheritOnly = 8,
		Inherited = 16,
		SuccessfulAccess = 64,
		FailedAccess = 128,
		InheritanceFlags = 15,
		AuditFlags = 192
	}
}
