using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal enum StatusMessage : uint
	{
		ActivateMachine,
		ActivateGroupIdentity,
		AcquireLicense,
		AcquireAdvisory,
		SignIssuanceLicense,
		AcquireClientLicensor,
		AcquireIssuanceLicenseTemplate
	}
}
