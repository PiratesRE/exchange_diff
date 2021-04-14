using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPerformanceTracker
	{
		void SetMailboxSessionToTrack(IMailboxSession session);

		void Start();

		void Stop();
	}
}
