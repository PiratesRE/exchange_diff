using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CrossServerBehavior
	{
		public string ClientId { get; private set; }

		public bool PreExchange15 { get; private set; }

		public bool ShouldTrace { get; private set; }

		public bool ShouldLogInfoWatson { get; private set; }

		public bool ShouldBlock { get; private set; }

		public CrossServerBehavior(string clientId, bool preExchange15, bool shouldTrace, bool shouldLogInfoWatson, bool shouldBlock)
		{
			this.ClientId = clientId;
			this.PreExchange15 = preExchange15;
			this.ShouldTrace = shouldTrace;
			this.ShouldLogInfoWatson = shouldLogInfoWatson;
			this.ShouldBlock = shouldBlock;
		}
	}
}
