using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RemoteServerRoundtripCompleteEventArgs : RoundtripCompleteEventArgs
	{
		internal RemoteServerRoundtripCompleteEventArgs(string serverName, TimeSpan roundtripTime, bool roundtripSuccessful) : base(roundtripTime, roundtripSuccessful)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("serverName", serverName);
			this.serverName = serverName;
		}

		internal string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
