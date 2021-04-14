using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum GzipLevel
	{
		[LocDescription(DirectoryStrings.IDs.Off)]
		Off,
		[LocDescription(DirectoryStrings.IDs.Low)]
		Low,
		[LocDescription(DirectoryStrings.IDs.High)]
		High,
		[LocDescription(DirectoryStrings.IDs.Error)]
		Error
	}
}
