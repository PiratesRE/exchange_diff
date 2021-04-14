using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Worker.Health;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SyncStorageProviderConnectionStatistics
	{
		internal SyncStorageProviderConnectionStatistics()
		{
			this.totalSuccessfulRoundtrips = 0;
			this.totalTimeForAllSuccesfulRoundtrips = TimeSpan.Zero;
			this.totalUnsuccessfulRoundtrips = 0;
			this.totalTimeForAllUnsuccesfulRoundtrips = TimeSpan.Zero;
			this.throttlingStatistics = new ThrottlingStatistics();
		}

		public int TotalSuccessfulRoundtrips
		{
			get
			{
				return this.totalSuccessfulRoundtrips;
			}
		}

		internal TimeSpan AverageSuccessfulRoundtripTime
		{
			get
			{
				double value = (this.totalSuccessfulRoundtrips != 0) ? (this.totalTimeForAllSuccesfulRoundtrips.TotalMilliseconds / (double)this.totalSuccessfulRoundtrips) : 0.0;
				return TimeSpan.FromMilliseconds(value);
			}
		}

		public int TotalUnsuccessfulRoundtrips
		{
			get
			{
				return this.totalUnsuccessfulRoundtrips;
			}
		}

		internal TimeSpan AverageUnsuccessfulRoundtripTime
		{
			get
			{
				double value = (this.totalUnsuccessfulRoundtrips != 0) ? (this.totalTimeForAllUnsuccesfulRoundtrips.TotalMilliseconds / (double)this.totalUnsuccessfulRoundtrips) : 0.0;
				return TimeSpan.FromMilliseconds(value);
			}
		}

		internal TimeSpan AverageBackoffTime
		{
			get
			{
				int num = this.totalSuccessfulRoundtrips + this.totalUnsuccessfulRoundtrips;
				double value = (num != 0) ? (this.throttlingStatistics.TotalBackoffTime.TotalMilliseconds / (double)num) : 0.0;
				return TimeSpan.FromMilliseconds(value);
			}
		}

		internal ThrottlingStatistics ThrottlingStatistics
		{
			get
			{
				return this.throttlingStatistics;
			}
		}

		internal void OnRoundtripComplete(object sender, RoundtripCompleteEventArgs e)
		{
			SyncUtilities.ThrowIfArgumentNull("RoundtripCompleteEventArgs", e != null);
			if (e.RoundtripSuccessful)
			{
				this.totalSuccessfulRoundtrips++;
				this.totalTimeForAllSuccesfulRoundtrips += e.RoundtripTime;
			}
			else
			{
				this.totalUnsuccessfulRoundtrips++;
				this.totalTimeForAllUnsuccesfulRoundtrips += e.RoundtripTime;
			}
			this.throttlingStatistics.Add(e.ThrottlingInfo);
		}

		private int totalSuccessfulRoundtrips;

		private TimeSpan totalTimeForAllSuccesfulRoundtrips;

		private int totalUnsuccessfulRoundtrips;

		private TimeSpan totalTimeForAllUnsuccesfulRoundtrips;

		private ThrottlingStatistics throttlingStatistics;
	}
}
