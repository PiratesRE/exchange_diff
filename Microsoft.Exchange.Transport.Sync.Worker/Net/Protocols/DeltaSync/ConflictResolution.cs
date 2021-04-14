using System;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	internal enum ConflictResolution : byte
	{
		ClientWins,
		ServerWins
	}
}
