using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationIssueCache : ServiceIssueCache
	{
		public MigrationIssueCache(Func<IMigrationJobCache> getJobCache)
		{
			this.GetJobCache = getJobCache;
		}

		protected override string ComponentName
		{
			get
			{
				return "MigrationIssueCache";
			}
		}

		public override bool ScanningIsEnabled
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("IssueCacheIsEnabled");
			}
		}

		protected override int IssueLimit
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("IssueCacheItemLimit");
			}
		}

		protected override TimeSpan FullScanFrequency
		{
			get
			{
				return ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("IssueCacheScanFrequency");
			}
		}

		private Func<IMigrationJobCache> GetJobCache { get; set; }

		protected override ICollection<ServiceIssue> RunFullIssueScan()
		{
			ICollection<ServiceIssue> collection = new List<ServiceIssue>();
			List<MigrationCacheEntry> list = this.GetJobCache().Get();
			foreach (MigrationCacheEntry migrationCacheEntry in list)
			{
				try
				{
					using (IMigrationDataProvider migrationDataProvider = MigrationServiceFactory.Instance.CreateProviderForMigrationMailbox(migrationCacheEntry.TenantPartitionHint, migrationCacheEntry.MigrationMailboxLegacyDN))
					{
						IEnumerable<MigrationJob> byStatus = MigrationJob.GetByStatus(migrationDataProvider, null, MigrationJobStatus.Corrupted);
						foreach (MigrationJob job in byStatus)
						{
							collection.Add(new MigrationJobIssue(job));
						}
						IEnumerable<MigrationJobItem> byStatus2 = MigrationJobItem.GetByStatus(migrationDataProvider, null, MigrationUserStatus.Corrupted, null);
						foreach (MigrationJobItem jobItem in byStatus2)
						{
							collection.Add(new MigrationJobItemIssue(jobItem));
						}
					}
				}
				catch (LocalizedException lastScanError)
				{
					base.LastScanError = lastScanError;
				}
				catch (InvalidDataException lastScanError2)
				{
					base.LastScanError = lastScanError2;
				}
			}
			return collection;
		}

		private const string DiagnosticsComponentName = "MigrationIssueCache";
	}
}
