using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum DialByNameSecondaryEnum
	{
		[LocDescription(DirectoryStrings.IDs.LastFirst)]
		LastFirst,
		[LocDescription(DirectoryStrings.IDs.FirstLast)]
		FirstLast,
		[LocDescription(DirectoryStrings.IDs.SMTPAddress)]
		SMTPAddress,
		[LocDescription(DirectoryStrings.IDs.None)]
		None
	}
}
