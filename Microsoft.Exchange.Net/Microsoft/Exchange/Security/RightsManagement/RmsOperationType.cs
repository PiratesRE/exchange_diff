using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal enum RmsOperationType
	{
		AcquireLicense = 1,
		AcquireTemplates,
		AcquireTemplateInfo,
		AcquireServerBoxRac,
		AcquireClc,
		AcquirePrelicense,
		FindServiceLocations,
		AcquireCertificationMexData,
		AcquireServerLicensingMexData,
		AcquireB2BRac,
		AcquireB2BLicense,
		RequestDelegationToken
	}
}
