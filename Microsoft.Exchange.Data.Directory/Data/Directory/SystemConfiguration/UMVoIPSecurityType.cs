using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum UMVoIPSecurityType
	{
		[LocDescription(DirectoryStrings.IDs.SIPSecured)]
		SIPSecured = 1,
		[LocDescription(DirectoryStrings.IDs.Unsecured)]
		Unsecured,
		[LocDescription(DirectoryStrings.IDs.Secured)]
		Secured
	}
}
