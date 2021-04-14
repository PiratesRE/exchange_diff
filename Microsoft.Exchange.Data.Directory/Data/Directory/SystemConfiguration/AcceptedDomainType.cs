using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AcceptedDomainType
	{
		[LocDescription(DirectoryStrings.IDs.Authoritative)]
		Authoritative,
		[LocDescription(DirectoryStrings.IDs.ExternalRelay)]
		ExternalRelay,
		[LocDescription(DirectoryStrings.IDs.InternalRelay)]
		InternalRelay
	}
}
