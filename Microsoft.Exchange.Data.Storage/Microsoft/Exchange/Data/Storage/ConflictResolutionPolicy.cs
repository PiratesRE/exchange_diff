using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ConflictResolutionPolicy
	{
		ClientWins = 1,
		ServerWins,
		ConflictMessage,
		DelegateResolution
	}
}
