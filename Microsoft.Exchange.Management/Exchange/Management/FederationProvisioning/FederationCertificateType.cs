using System;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	[Flags]
	internal enum FederationCertificateType
	{
		PreviousCertificate = 1,
		CurrentCertificate = 2,
		NextCertificate = 4
	}
}
