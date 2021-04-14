using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DialByNamePrimaryEnum
	{
		[LocDescription(DirectoryStrings.IDs.LastFirst)]
		LastFirst,
		[LocDescription(DirectoryStrings.IDs.FirstLast)]
		FirstLast,
		[LocDescription(DirectoryStrings.IDs.SMTPAddress)]
		SMTPAddress
	}
}
