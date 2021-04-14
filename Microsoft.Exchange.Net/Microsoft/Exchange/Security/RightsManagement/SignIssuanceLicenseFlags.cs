using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum SignIssuanceLicenseFlags : uint
	{
		Online = 1U,
		Offline = 2U,
		Cancel = 4U,
		ServerIssuanceLicense = 8U,
		AutoGenerateKey = 16U,
		OwnerLicenseNoPersist = 32U
	}
}
