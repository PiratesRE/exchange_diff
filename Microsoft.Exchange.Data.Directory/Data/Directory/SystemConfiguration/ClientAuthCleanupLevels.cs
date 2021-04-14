using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ClientAuthCleanupLevels
	{
		[LocDescription(DirectoryStrings.IDs.High)]
		High,
		[LocDescription(DirectoryStrings.IDs.Low)]
		Low
	}
}
