using System;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	[Flags]
	internal enum TokenAccessLevels
	{
		AssignPrimary = 1,
		Duplicate = 2,
		Impersonate = 4,
		Query = 8,
		QuerySource = 16,
		AdjustPrivileges = 32,
		AdjustGroups = 64,
		AdjustDefault = 128,
		AdjustSessionId = 256,
		Read = 131080,
		Write = 131296,
		AllAccess = 983551,
		MaximumAllowed = 33554432
	}
}
