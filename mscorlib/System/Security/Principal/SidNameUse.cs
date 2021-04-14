using System;

namespace System.Security.Principal
{
	internal enum SidNameUse
	{
		User = 1,
		Group,
		Domain,
		Alias,
		WellKnownGroup,
		DeletedAccount,
		Invalid,
		Unknown,
		Computer
	}
}
