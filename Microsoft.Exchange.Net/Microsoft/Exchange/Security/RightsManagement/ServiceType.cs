using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum ServiceType : uint
	{
		Activation = 1U,
		Certification = 2U,
		Publishing = 4U,
		ClientLicensor = 8U
	}
}
