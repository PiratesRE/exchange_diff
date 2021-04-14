using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LegacyExchangeSubscriptionHandler : LegacyMrsSubscriptionHandlerBase
	{
		internal LegacyExchangeSubscriptionHandler(IMigrationDataProvider dataProvider, MigrationJob job) : base(dataProvider, job, MRSSubscriptionArbiter.Instance)
		{
		}

		public override MigrationType SupportedMigrationType
		{
			get
			{
				return MigrationType.ExchangeOutlookAnywhere;
			}
		}

		public override bool SupportsAdvancedValidation
		{
			get
			{
				return false;
			}
		}

		protected override MigrationUserStatus PostTestStatus
		{
			get
			{
				return MigrationUserStatus.Provisioning;
			}
		}

		public override bool CreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			return base.InternalCreate(jobItem, false);
		}

		public override bool TestCreateUnderlyingSubscriptions(MigrationJobItem jobItem)
		{
			return base.InternalCreate(jobItem, true);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LegacyExchangeSubscriptionHandler>(this);
		}

		protected override bool DiscoverAndSetSubscriptionSettings(MigrationJobItem jobItem)
		{
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = jobItem.MigrationJob.SourceEndpoint as ExchangeOutlookAnywhereEndpoint;
			MigrationUtil.AssertOrThrow(exchangeOutlookAnywhereEndpoint != null, "An SEM job should have an ExchangeOutlookAnywhereEndpoint as its source.", new object[0]);
			if (exchangeOutlookAnywhereEndpoint.UseAutoDiscover)
			{
				IMigrationAutodiscoverClient autodiscoverClient = MigrationServiceFactory.Instance.GetAutodiscoverClient();
				AutodiscoverClientResponse userSettings = autodiscoverClient.GetUserSettings(exchangeOutlookAnywhereEndpoint, jobItem.RemoteIdentifier ?? jobItem.Identifier);
				if (userSettings.Status != AutodiscoverClientStatus.NoError)
				{
					MigrationLogger.Log(MigrationEventType.Warning, "job item {0} couldn't get auto-discover settings {1}", new object[]
					{
						this,
						userSettings.ErrorMessage
					});
					LocalizedException localizedError;
					if (userSettings.Status == AutodiscoverClientStatus.ConfigurationError)
					{
						localizedError = new AutoDiscoverFailedConfigurationErrorException(userSettings.ErrorMessage);
					}
					else
					{
						localizedError = new AutoDiscoverFailedInternalErrorException(userSettings.ErrorMessage);
					}
					jobItem.SetSubscriptionFailed(base.DataProvider, MigrationUserStatus.Failed, localizedError);
					return false;
				}
				jobItem.SetSubscriptionSettings(base.DataProvider, ExchangeJobItemSubscriptionSettings.CreateFromAutodiscoverResponse(userSettings));
			}
			else
			{
				jobItem.SetSubscriptionSettings(base.DataProvider, null);
			}
			return true;
		}
	}
}
