using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RoundtripCompleteEventArgs : EventArgs
	{
		public RoundtripCompleteEventArgs(TimeSpan roundtripTime, bool roundtripSuccessful)
		{
			this.RoundtripTime = roundtripTime;
			this.RoundtripSuccessful = roundtripSuccessful;
			this.ServerName = string.Empty;
		}

		public RoundtripCompleteEventArgs(TimeSpan roundtripTime, bool roundtripSuccessful, string serverName) : this(roundtripTime, roundtripSuccessful)
		{
			this.ServerName = serverName;
		}

		public TimeSpan RoundtripTime { get; private set; }

		internal bool RoundtripSuccessful { get; private set; }

		internal string ServerName { get; private set; }
	}
}
