using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncManagerConfiguration : ISyncManagerConfiguration, ISubscriptionProcessPermitterConfig
	{
		private SyncManagerConfiguration()
		{
		}

		public static SyncManagerConfiguration Instance
		{
			get
			{
				return SyncManagerConfiguration.instance;
			}
		}

		public TimeSpan WorkTypeBudgetManagerSlidingWindowLength
		{
			get
			{
				return ContentAggregationConfig.WorkTypeBudgetManagerSlidingWindowLength;
			}
		}

		public TimeSpan WorkTypeBudgetManagerSlidingBucketLength
		{
			get
			{
				return ContentAggregationConfig.WorkTypeBudgetManagerSlidingBucketLength;
			}
		}

		public TimeSpan WorkTypeBudgetManagerSampleDispatchedWorkFrequency
		{
			get
			{
				return ContentAggregationConfig.WorkTypeBudgetManagerSampleDispatchedWorkFrequency;
			}
		}

		public TimeSpan DatabaseBackoffTime
		{
			get
			{
				return ContentAggregationConfig.DatabaseBackoffTime;
			}
		}

		public int MaxSyncsPerDB
		{
			get
			{
				return ContentAggregationConfig.MaxSyncsPerDB;
			}
		}

		public TimeSpan DispatchEntryExpirationTime
		{
			get
			{
				return ContentAggregationConfig.DispatchEntryExpirationTime;
			}
		}

		public TimeSpan DispatchEntryExpirationCheckFrequency
		{
			get
			{
				return ContentAggregationConfig.DispatchEntryExpirationCheckFrequency;
			}
		}

		public TimeSpan DispatcherDatabaseRefreshFrequency
		{
			get
			{
				return ContentAggregationConfig.DispatcherDatabaseRefreshFrequency;
			}
		}

		public bool AggregationSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.AggregationSubscriptionsEnabled;
			}
		}

		public bool MigrationSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.MigrationSubscriptionsEnabled;
			}
		}

		public bool PeopleConnectionSubscriptionsEnabled
		{
			get
			{
				return ContentAggregationConfig.PeopleConnectionSubscriptionsEnabled;
			}
		}

		public bool PopAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.PopAggregationEnabled;
			}
		}

		public bool DeltaSyncAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.DeltaSyncAggregationEnabled;
			}
		}

		public bool ImapAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.ImapAggregationEnabled;
			}
		}

		public bool FacebookAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.FacebookAggregationEnabled;
			}
		}

		public bool LinkedInAggregationEnabled
		{
			get
			{
				return ContentAggregationConfig.LinkedInAggregationEnabled;
			}
		}

		private static SyncManagerConfiguration instance = new SyncManagerConfiguration();
	}
}
