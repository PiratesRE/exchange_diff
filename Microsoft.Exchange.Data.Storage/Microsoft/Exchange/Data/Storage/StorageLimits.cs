using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StorageLimits : IStorageLimits
	{
		private StorageLimits()
		{
			this.storageLimitDefaults = new StorageLimits.StorageLimitDefaults();
		}

		public static StorageLimits Instance
		{
			get
			{
				if (StorageLimits.instance == null)
				{
					StorageLimits.instance = new StorageLimits();
				}
				return StorageLimits.instance;
			}
		}

		public int NamedPropertyNameMaximumLength
		{
			get
			{
				return this.CurrentLimits.NamedPropertyNameMaximumLength;
			}
		}

		public int UserConfigurationMaxSearched
		{
			get
			{
				return this.CurrentLimits.UserConfigurationMaxSearched;
			}
		}

		public int FindNamesViewResultsLimit
		{
			get
			{
				return this.CurrentLimits.FindNamesViewResultsLimit;
			}
		}

		public int AmbiguousNamesViewResultsLimit
		{
			get
			{
				return this.CurrentLimits.AmbiguousNamesViewResultsLimit;
			}
		}

		public int CalendarSingleInstanceLimit
		{
			get
			{
				return this.CurrentLimits.CalendarSingleInstanceLimit;
			}
		}

		public int CalendarExpansionInstanceLimit
		{
			get
			{
				return this.CurrentLimits.CalendarExpansionInstanceLimit;
			}
		}

		public int CalendarExpansionMaxMasters
		{
			get
			{
				return this.CurrentLimits.CalendarExpansionMaxMasters;
			}
		}

		public int CalendarMaxNumberVEventsForICalImport
		{
			get
			{
				return this.CurrentLimits.CalendarMaxNumberVEventsForICalImport;
			}
		}

		public int CalendarMaxNumberBytesForICalImport
		{
			get
			{
				return this.CurrentLimits.CalendarMaxNumberBytesForICalImport;
			}
		}

		public int RecurrenceMaximumInterval
		{
			get
			{
				return this.CurrentLimits.RecurrenceMaximumInterval;
			}
		}

		public int RecurrenceMaximumNumberedOccurrences
		{
			get
			{
				return this.CurrentLimits.RecurrenceMaximumNumberedOccurrences;
			}
		}

		public int DistributionListMaxMembersPropertySize
		{
			get
			{
				return this.CurrentLimits.DistributionListMaxMembersPropertySize;
			}
		}

		public int DistributionListMaxNumberOfEntries
		{
			get
			{
				return this.CurrentLimits.DistributionListMaxNumberOfEntries;
			}
		}

		public int DefaultFolderMinimumSuffix
		{
			get
			{
				return this.CurrentLimits.DefaultFolderMinimumSuffix;
			}
		}

		public int DefaultFolderMaximumSuffix
		{
			get
			{
				return this.CurrentLimits.DefaultFolderMaximumSuffix;
			}
		}

		public int DefaultFolderDataCacheMaxRowCount
		{
			get
			{
				return this.CurrentLimits.DefaultFolderDataCacheMaxRowCount;
			}
		}

		public int NotificationsMaxSubscriptions
		{
			get
			{
				return this.CurrentLimits.NotificationsMaxSubscriptions;
			}
		}

		public int MaxDelegates
		{
			get
			{
				return this.CurrentLimits.MaxDelegates;
			}
		}

		public BufferPoolCollection.BufferSize PropertyStreamPageSize
		{
			get
			{
				return this.CurrentLimits.PropertyStreamPageSize;
			}
		}

		public long ConversionsFolderMaxTotalMessageSize
		{
			get
			{
				return this.CurrentLimits.ConversionsFolderMaxTotalMessageSize;
			}
		}

		private IStorageLimits CurrentLimits
		{
			get
			{
				return this.testLimits ?? this.storageLimitDefaults;
			}
		}

		internal void TestSetLimits(IStorageLimits testLimits)
		{
			this.testLimits = testLimits;
		}

		private readonly StorageLimits.StorageLimitDefaults storageLimitDefaults;

		private static StorageLimits instance;

		private IStorageLimits testLimits;

		private class StorageLimitDefaults : IStorageLimits
		{
			public int NamedPropertyNameMaximumLength
			{
				get
				{
					return 127;
				}
			}

			public int UserConfigurationMaxSearched
			{
				get
				{
					return 10000;
				}
			}

			public int FindNamesViewResultsLimit
			{
				get
				{
					return 10000;
				}
			}

			public int AmbiguousNamesViewResultsLimit
			{
				get
				{
					return 200;
				}
			}

			public int CalendarSingleInstanceLimit
			{
				get
				{
					return 10000;
				}
			}

			public int CalendarExpansionInstanceLimit
			{
				get
				{
					return 5000;
				}
			}

			public int CalendarExpansionMaxMasters
			{
				get
				{
					return 1000;
				}
			}

			public int CalendarMaxNumberVEventsForICalImport
			{
				get
				{
					return 10000;
				}
			}

			public int CalendarMaxNumberBytesForICalImport
			{
				get
				{
					return 10485760;
				}
			}

			public int RecurrenceMaximumInterval
			{
				get
				{
					return 1000;
				}
			}

			public int RecurrenceMaximumNumberedOccurrences
			{
				get
				{
					return 999;
				}
			}

			public int DistributionListMaxMembersPropertySize
			{
				get
				{
					return 15000;
				}
			}

			public int DistributionListMaxNumberOfEntries
			{
				get
				{
					return 10000;
				}
			}

			public int DefaultFolderMinimumSuffix
			{
				get
				{
					return 1;
				}
			}

			public int DefaultFolderMaximumSuffix
			{
				get
				{
					return 9;
				}
			}

			public int DefaultFolderDataCacheMaxRowCount
			{
				get
				{
					return 1024;
				}
			}

			public int NotificationsMaxSubscriptions
			{
				get
				{
					return 1024;
				}
			}

			public int MaxDelegates
			{
				get
				{
					return 100000;
				}
			}

			public BufferPoolCollection.BufferSize PropertyStreamPageSize
			{
				get
				{
					return BufferPoolCollection.BufferSize.Size256K;
				}
			}

			public long ConversionsFolderMaxTotalMessageSize
			{
				get
				{
					return 134217728L;
				}
			}
		}
	}
}
