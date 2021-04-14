using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum ActivationFlags : uint
	{
		Machine = 1U,
		GroupIdentity = 2U,
		Temporary = 4U,
		Cancel = 8U,
		Silent = 16U,
		SharedGroupIdentity = 32U,
		Delayed = 64U
	}
}
