using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum EnumerateLicenseFlags : uint
	{
		Machine = 1U,
		GroupIdentity = 2U,
		GroupIdentityName = 4U,
		GroupIdentityLid = 8U,
		SpecifiedGroupIdentity = 16U,
		Eul = 32U,
		EulLid = 64U,
		ClientLicensor = 128U,
		ClientLicensorLid = 256U,
		SpecifiedClientLicensor = 512U,
		RevocationList = 1024U,
		RevocationListLid = 2048U,
		Expired = 4096U,
		IssuanceLicenseTemplate = 16384U,
		IssuanceLicenseTemplateLid = 32768U
	}
}
