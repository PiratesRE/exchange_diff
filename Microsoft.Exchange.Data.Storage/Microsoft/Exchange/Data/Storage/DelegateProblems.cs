using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DelegateProblems
	{
		None = 0,
		NoADUser = 2,
		NoADPublicDelegate = 4,
		NoDelegateInfo = 8,
		InvalidReceiveMeetingMessageCopies = 16
	}
}
