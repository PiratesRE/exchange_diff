using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Flags]
	public enum RequestFlags
	{
		None = 0,
		CrossOrg = 1,
		IntraOrg = 2,
		Push = 4,
		Pull = 8,
		Offline = 16,
		Protected = 32,
		RemoteLegacy = 64,
		HighPriority = 128,
		Suspend = 256,
		SuspendWhenReadyToComplete = 512,
		MoveOnlyPrimaryMailbox = 1024,
		MoveOnlyArchiveMailbox = 2048,
		TargetIsAggregatedMailbox = 4096,
		Join = 8192,
		Split = 16384
	}
}
