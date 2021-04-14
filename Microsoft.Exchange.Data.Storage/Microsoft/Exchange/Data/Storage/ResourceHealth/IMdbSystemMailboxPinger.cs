using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMdbSystemMailboxPinger
	{
		DateTime LastSuccessfulPingUtc { get; }

		DateTime LastPingAttemptUtc { get; }

		bool Pinging { get; }

		bool Ping();
	}
}
