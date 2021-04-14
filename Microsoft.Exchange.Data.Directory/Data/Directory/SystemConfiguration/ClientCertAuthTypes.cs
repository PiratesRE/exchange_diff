using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ClientCertAuthTypes
	{
		[LocDescription(DirectoryStrings.IDs.ClientCertAuthIgnore)]
		Ignore,
		[LocDescription(DirectoryStrings.IDs.ClientCertAuthAccepted)]
		Accepted,
		[LocDescription(DirectoryStrings.IDs.ClientCertAuthRequired)]
		Required
	}
}
