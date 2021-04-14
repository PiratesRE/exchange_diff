using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DataMigrationStepHandler : IStepHandler
	{
		public DataMigrationStepHandler(IMigrationDataProvider dataProvider, MigrationType migrationType, string jobName)
		{
			this.DataProvider = dataProvider;
			this.SubscriptionAccessor = MigrationServiceFactory.Instance.GetSubscriptionAccessor(this.DataProvider, migrationType, jobName, true, false);
		}

		public bool ExpectMailboxData
		{
			get
			{
				return !(this.SubscriptionAccessor is MRSXO1SyncRequestAccessor);
			}
		}

		private protected IMigrationDataProvider DataProvider { protected get; private set; }

		private protected SubscriptionAccessorBase SubscriptionAccessor { protected get; private set; }

		public IStepSettings Discover(MigrationJobItem jobItem, MailboxData localMailbox)
		{
			if (jobItem.MigrationJob.MigrationType != MigrationType.ExchangeOutlookAnywhere)
			{
				return null;
			}
			ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = jobItem.MigrationJob.SourceEndpoint as ExchangeOutlookAnywhereEndpoint;
			MigrationUtil.AssertOrThrow(exchangeOutlookAnywhereEndpoint != null, "An SEM job should have an ExchangeOutlookAnywhereEndpoint as its source.", new object[0]);
			string text = jobItem.RemoteIdentifier ?? jobItem.Identifier;
			if (!exchangeOutlookAnywhereEndpoint.UseAutoDiscover)
			{
				NspiMigrationDataReader nspiDataReader = exchangeOutlookAnywhereEndpoint.GetNspiDataReader(jobItem.MigrationJob);
				return nspiDataReader.GetSubscriptionSettings(text);
			}
			IMigrationAutodiscoverClient autodiscoverClient = MigrationServiceFactory.Instance.GetAutodiscoverClient();
			AutodiscoverClientResponse userSettings = autodiscoverClient.GetUserSettings(exchangeOutlookAnywhereEndpoint, text);
			if (userSettings.Status == AutodiscoverClientStatus.NoError)
			{
				return ExchangeJobItemSubscriptionSettings.CreateFromAutodiscoverResponse(userSettings);
			}
			MigrationLogger.Log(MigrationEventType.Warning, "job item {0} couldn't get auto-discover settings {1}", new object[]
			{
				this,
				userSettings.ErrorMessage
			});
			if (userSettings.Status == AutodiscoverClientStatus.ConfigurationError)
			{
				throw new AutoDiscoverFailedConfigurationErrorException(userSettings.ErrorMessage);
			}
			throw new AutoDiscoverFailedInternalErrorException(userSettings.ErrorMessage);
		}

		public void Validate(MigrationJobItem jobItem)
		{
			if (jobItem.MigrationType == MigrationType.ExchangeLocalMove || jobItem.MigrationType == MigrationType.ExchangeRemoteMove || jobItem.MigrationType == MigrationType.ExchangeOutlookAnywhere || jobItem.MigrationType == MigrationType.PSTImport || jobItem.MigrationType == MigrationType.IMAP)
			{
				if (jobItem.LocalMailbox == null)
				{
					throw new MigrationObjectNotFoundInADException(jobItem.Identifier, this.DataProvider.ADProvider.GetPreferredDomainController());
				}
				MigrationUserRecipientType recipientType = jobItem.RecipientType;
				if (recipientType != MigrationUserRecipientType.Mailbox)
				{
					switch (recipientType)
					{
					case MigrationUserRecipientType.Mailuser:
						if (jobItem.LocalMailbox.RecipientType != MigrationUserRecipientType.Mailuser)
						{
							throw new InvalidRecipientTypeException(MigrationUserRecipientType.Mailbox.ToString(), MigrationUserRecipientType.Mailuser.ToString());
						}
						break;
					case MigrationUserRecipientType.MailboxOrMailuser:
						break;
					default:
						throw new UnsupportedRecipientTypeForProtocolException(jobItem.RecipientType.ToString(), jobItem.MigrationType.ToString());
					}
				}
				else if (jobItem.LocalMailbox.RecipientType != MigrationUserRecipientType.Mailbox)
				{
					throw new InvalidRecipientTypeException(MigrationUserRecipientType.Mailuser.ToString(), MigrationUserRecipientType.Mailbox.ToString());
				}
			}
			if (jobItem.MigrationJob.UseAdvancedValidation)
			{
				this.SubscriptionAccessor.TestCreateSubscription(jobItem);
			}
		}

		public IStepSnapshot Inject(MigrationJobItem jobItem)
		{
			return this.SubscriptionAccessor.CreateSubscription(jobItem);
		}

		public IStepSnapshot Process(ISnapshotId id, MigrationJobItem jobItem, out bool updated)
		{
			updated = false;
			MigrationEndpointBase migrationEndpointBase = null;
			if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Onboarding)
			{
				migrationEndpointBase = jobItem.MigrationJob.SourceEndpoint;
			}
			else if (jobItem.MigrationJob.JobDirection == MigrationBatchDirection.Offboarding)
			{
				migrationEndpointBase = jobItem.MigrationJob.TargetEndpoint;
			}
			ExDateTime t = jobItem.SubscriptionSettingsLastUpdatedTime ?? ExDateTime.MinValue;
			bool flag = jobItem.SubscriptionSettings != null && jobItem.SubscriptionSettings.LastModifiedTime > t;
			bool flag2 = jobItem.MigrationJob.SubscriptionSettings != null && jobItem.MigrationJob.SubscriptionSettings.LastModifiedTime > t;
			if ((migrationEndpointBase != null && migrationEndpointBase.LastModifiedTime > t) || flag2 || flag)
			{
				updated = this.SubscriptionAccessor.UpdateSubscription((ISubscriptionId)id, migrationEndpointBase, jobItem, false);
			}
			return this.SubscriptionAccessor.RetrieveSubscriptionSnapshot((ISubscriptionId)id);
		}

		public void Start(ISnapshotId id)
		{
			this.SubscriptionAccessor.ResumeSubscription((ISubscriptionId)id, false);
		}

		public IStepSnapshot Stop(ISnapshotId id)
		{
			if (!this.SubscriptionAccessor.SuspendSubscription((ISubscriptionId)id))
			{
				return this.SubscriptionAccessor.RetrieveSubscriptionSnapshot((ISubscriptionId)id);
			}
			return null;
		}

		public void Delete(ISnapshotId id)
		{
			this.SubscriptionAccessor.RemoveSubscription((ISubscriptionId)id);
		}

		public bool CanProcess(MigrationJobItem jobItem)
		{
			return true;
		}

		public MigrationUserStatus ResolvePresentationStatus(MigrationFlags flags, IStepSnapshot stepSnapshot = null)
		{
			MigrationUserStatus? migrationUserStatus = MigrationJobItem.ResolveFlagStatus(flags);
			if (migrationUserStatus != null)
			{
				return migrationUserStatus.Value;
			}
			SubscriptionSnapshot subscriptionSnapshot = stepSnapshot as SubscriptionSnapshot;
			if (subscriptionSnapshot != null && subscriptionSnapshot.IsInitialSyncComplete)
			{
				return MigrationUserStatus.Synced;
			}
			return MigrationUserStatus.Syncing;
		}

		public static readonly MigrationStage[] AllowedStages = new MigrationStage[]
		{
			MigrationStage.Discovery,
			MigrationStage.Validation,
			MigrationStage.Injection,
			MigrationStage.Processing
		};
	}
}
