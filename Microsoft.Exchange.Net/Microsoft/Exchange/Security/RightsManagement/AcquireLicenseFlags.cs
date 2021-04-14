using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum AcquireLicenseFlags : uint
	{
		NonSilent = 1U,
		NoPersist = 2U,
		Cancel = 4U,
		FetchAdvisory = 8U,
		NoUI = 16U
	}
}
