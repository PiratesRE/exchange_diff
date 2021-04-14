using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum ElcContentSettingFlags
	{
		None = 0,
		RetentionEnabled = 1,
		MoveDateBasedRetention = 2,
		JournalingEnabled = 4,
		JournalAsMSG = 8,
		Tag = 16
	}
}
