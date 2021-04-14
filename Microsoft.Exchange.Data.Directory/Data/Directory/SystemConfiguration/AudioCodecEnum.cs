using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum AudioCodecEnum
	{
		[LocDescription(DirectoryStrings.IDs.G711)]
		G711,
		[LocDescription(DirectoryStrings.IDs.Wma)]
		Wma,
		[LocDescription(DirectoryStrings.IDs.Gsm)]
		Gsm,
		[LocDescription(DirectoryStrings.IDs.Mp3)]
		Mp3
	}
}
