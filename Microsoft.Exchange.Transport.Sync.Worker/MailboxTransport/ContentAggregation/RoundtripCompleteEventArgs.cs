using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RoundtripCompleteEventArgs : EventArgs
	{
		public RoundtripCompleteEventArgs(TimeSpan roundtripTime, bool roundtripSuccessful)
		{
			this.roundtripTime = roundtripTime;
			this.roundtripSuccessful = roundtripSuccessful;
			this.throttlingInfo = null;
		}

		public RoundtripCompleteEventArgs(TimeSpan roundtripTime, ThrottlingInfo throttlingInfo, bool roundtripSuccessful) : this(roundtripTime, roundtripSuccessful)
		{
			this.throttlingInfo = throttlingInfo;
		}

		public TimeSpan RoundtripTime
		{
			get
			{
				return this.roundtripTime;
			}
		}

		internal bool RoundtripSuccessful
		{
			get
			{
				return this.roundtripSuccessful;
			}
		}

		internal ThrottlingInfo ThrottlingInfo
		{
			get
			{
				return this.throttlingInfo;
			}
		}

		private TimeSpan roundtripTime;

		private bool roundtripSuccessful;

		private ThrottlingInfo throttlingInfo;
	}
}
