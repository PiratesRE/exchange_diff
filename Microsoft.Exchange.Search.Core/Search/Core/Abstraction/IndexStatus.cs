using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal sealed class IndexStatus
	{
		public IndexStatus(string seedingSource, VersionInfo version) : this(ContentIndexStatusType.Seeding, IndexStatusErrorCode.SeedingCatalog, version, seedingSource)
		{
		}

		public IndexStatus(int mailboxToCrawl, VersionInfo version) : this(version.IsUpgrading ? ContentIndexStatusType.HealthyAndUpgrading : ContentIndexStatusType.Crawling, IndexStatusErrorCode.CrawlingDatabase, version, null, mailboxToCrawl)
		{
		}

		public IndexStatus(ContentIndexStatusType indexingState, IndexStatusErrorCode errorCode, VersionInfo version) : this(indexingState, errorCode, version, null)
		{
		}

		public IndexStatus(ContentIndexStatusType indexingState, IndexStatusErrorCode errorCode, VersionInfo version, string seedingSource) : this(indexingState, errorCode, version, seedingSource, 0)
		{
		}

		public IndexStatus(ContentIndexStatusType indexingState, IndexStatusErrorCode errorCode, VersionInfo version, string seedingSource, int mailboxToCrawl) : this(indexingState, errorCode, version, seedingSource, mailboxToCrawl, ExDateTime.UtcNow)
		{
			if ((mailboxToCrawl != 0 && indexingState != ContentIndexStatusType.Crawling && indexingState != ContentIndexStatusType.HealthyAndUpgrading) || (mailboxToCrawl <= 0 && (indexingState == ContentIndexStatusType.Crawling || indexingState == ContentIndexStatusType.HealthyAndUpgrading)))
			{
				throw new ArgumentOutOfRangeException(string.Format("Unexpected mailboxToCrawl value {0} when indexingState is {1}", mailboxToCrawl, indexingState));
			}
		}

		public IndexStatus(ContentIndexStatusType indexingState, IndexStatusErrorCode errorCode, VersionInfo version, string seedingSource, int mailboxToCrawl, ExDateTime timeStamp)
		{
			if (indexingState == ContentIndexStatusType.Failed && errorCode == IndexStatusErrorCode.Unknown)
			{
				throw new ArgumentException("Must have a known error code for failed state");
			}
			this.TimeStamp = timeStamp;
			this.IndexingState = indexingState;
			this.ErrorCode = errorCode;
			this.Version = version;
			this.MailboxesToCrawl = mailboxToCrawl;
			this.AgeOfLastNotificationProcessed = IndexStatus.DefaultCounterValue;
			this.RetriableItemsCount = IndexStatus.DefaultCounterValue;
			this.SeedingSource = seedingSource;
		}

		public ExDateTime TimeStamp { get; set; }

		public ContentIndexStatusType IndexingState { get; set; }

		public IndexStatusErrorCode ErrorCode { get; set; }

		public VersionInfo Version { get; set; }

		public int MailboxesToCrawl { get; set; }

		public string SeedingSource { get; set; }

		public long AgeOfLastNotificationProcessed { get; set; }

		public long RetriableItemsCount { get; set; }

		public static LocalizedString GetExcludeReasonFromErrorCode(IndexStatusErrorCode errorCode)
		{
			switch (errorCode)
			{
			case IndexStatusErrorCode.Unknown:
			case IndexStatusErrorCode.Success:
				return LocalizedString.Empty;
			case IndexStatusErrorCode.InternalError:
				return Strings.InternalError;
			case IndexStatusErrorCode.CrawlingDatabase:
				return Strings.CrawlingDatabase;
			case IndexStatusErrorCode.DatabaseOffline:
				return Strings.DatabaseOffline;
			case IndexStatusErrorCode.MapiNetworkError:
				return Strings.MapiNetworkError;
			case IndexStatusErrorCode.CatalogCorruption:
			case IndexStatusErrorCode.CatalogCorruptionWhenFeedingStarts:
			case IndexStatusErrorCode.CatalogCorruptionWhenFeedingCompletes:
				return Strings.CatalogCorruption;
			case IndexStatusErrorCode.SeedingCatalog:
				return Strings.SeedingCatalog;
			case IndexStatusErrorCode.CatalogSuspended:
				return Strings.CatalogSuspended;
			case IndexStatusErrorCode.CatalogReseed:
			case IndexStatusErrorCode.EventsMissingWithNotificationsWatermark:
			case IndexStatusErrorCode.CrawlOnNonPreferredActiveWithNotificationsWatermark:
			case IndexStatusErrorCode.CrawlOnNonPreferredActiveWithTooManyNotificationEvents:
			case IndexStatusErrorCode.CrawlOnPassive:
				return Strings.CatalogReseed;
			case IndexStatusErrorCode.IndexNotEnabled:
				return Strings.IndexNotEnabled;
			case IndexStatusErrorCode.CatalogExcluded:
				return Strings.CatalogExcluded;
			case IndexStatusErrorCode.ActivationPreferenceSkipped:
				return Strings.ActivationPreferenceSkipped;
			case IndexStatusErrorCode.LagCopySkipped:
				return Strings.LagCopySkipped;
			case IndexStatusErrorCode.RecoveryDatabaseSkipped:
				return Strings.RecoveryDatabaseSkipped;
			case IndexStatusErrorCode.FastError:
				return Strings.FastServiceNotRunning(Environment.MachineName);
			case IndexStatusErrorCode.ServiceNotRunning:
				return Strings.SearchServiceNotRunning(Environment.MachineName);
			case IndexStatusErrorCode.IndexStatusTimestampTooOld:
				return Strings.IndexStatusTimestampTooOld;
			default:
				throw new ArgumentException(string.Format("Error code {0} doesn't match any reason string", errorCode));
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"IndexingState=",
				this.IndexingState,
				", ErrorCode=",
				this.ErrorCode,
				", Version=",
				this.Version,
				", MailboxesToCrawl=",
				this.MailboxesToCrawl,
				", SeedingSource=",
				this.SeedingSource ?? "(n/a)",
				", TimeStamp=",
				this.TimeStamp.ToString("u"),
				", AgeOfLastNotificationProcessed=",
				this.AgeOfLastNotificationProcessed,
				", RetriableItemsCount=",
				this.RetriableItemsCount
			});
		}

		public void UpdateValue(IndexStatusIndex indexStatusIndex, long value)
		{
			switch (indexStatusIndex)
			{
			case IndexStatusIndex.AgeOfLastNotificationProcessed:
				this.AgeOfLastNotificationProcessed = value;
				return;
			case IndexStatusIndex.RetriableItemsCount:
				this.RetriableItemsCount = value;
				return;
			default:
				throw new InvalidOperationException("indexStatusIndex");
			}
		}

		internal static readonly long DefaultCounterValue;
	}
}
