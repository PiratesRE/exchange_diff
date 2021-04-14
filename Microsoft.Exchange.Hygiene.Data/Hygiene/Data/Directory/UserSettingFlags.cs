using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Flags]
	internal enum UserSettingFlags
	{
		None = 0,
		IsOutbound = 1,
		SpamEnabled = 2,
		PolicyEnabled = 4,
		VirusEnabled = 8,
		Archive = 16
	}
}
