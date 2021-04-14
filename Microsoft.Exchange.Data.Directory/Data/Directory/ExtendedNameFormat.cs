using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum ExtendedNameFormat
	{
		NameUnknown,
		NameFullyQualifiedDN,
		NameSamCompatible,
		NameDisplay,
		NameUniqueId = 6,
		NameCanonical,
		NameUserPrincipal,
		NameCanonicalEx,
		NameServicePrincipal,
		NameDnsDomain = 12
	}
}
