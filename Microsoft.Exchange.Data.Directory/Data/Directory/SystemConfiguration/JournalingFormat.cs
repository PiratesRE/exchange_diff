using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum JournalingFormat
	{
		[LocDescription(DirectoryStrings.IDs.UseMsg)]
		UseMsg,
		[LocDescription(DirectoryStrings.IDs.UseTnef)]
		UseTnef
	}
}
