using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum RetentionAndArchiveFlags
	{
		None = 0,
		ExplicitTag = 1,
		UserOverride = 2,
		Autotag = 4,
		PersonalTag = 8,
		AllRetentionFlags = 15,
		ExplictArchiveTag = 16,
		KeepInPlace = 32,
		AllArchiveFlags = 48,
		SystemData = 64,
		NeedsRescan = 128,
		PendingRescan = 256,
		EHAMigration = 512,
		[Obsolete("This feature has been retired.  Keeping this value to prevent re-use as objects may already have this bit enabled.")]
		RescanForTags = 1024
	}
}
