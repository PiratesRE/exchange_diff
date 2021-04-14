using System;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Migration.Test;
using Microsoft.Exchange.Rpc.MigrationService;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceFactory
	{
		protected MigrationServiceFactory()
		{
			this.cryptoAdapter = new Lazy<ICryptoAdapter>(new Func<ICryptoAdapter>(MigrationServiceFactory.CreateCryptoAdapter), LazyThreadSafetyMode.PublicationOnly);
		}

		public static MigrationServiceFactory Instance
		{
			get
			{
				return MigrationServiceFactory.instance;
			}
			protected set
			{
				MigrationServiceFactory.instance = value;
			}
		}

		public virtual bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		public virtual string GetLocalServerFqdn()
		{
			return LocalServer.GetServer().Fqdn;
		}

		public virtual IMigrationProxyRpc GetMigrationProxyRpcClient(string server)
		{
			return new MigrationProxyRpcClient(server);
		}

		public virtual IMigrationDataProvider CreateProviderForMigrationMailbox(TenantPartitionHint tenantPartitionHint, string migrationMailboxLegacyDN)
		{
			return MigrationDataProvider.CreateProviderForMigrationMailbox(tenantPartitionHint, migrationMailboxLegacyDN);
		}

		public virtual IMigrationDataProvider CreateProviderForSystemMailbox(Guid mdbGuid)
		{
			return MigrationDataProvider.CreateProviderForSystemMailbox(mdbGuid);
		}

		public virtual JobProcessor CreateJobProcessor(MigrationJob job)
		{
			return JobProcessor.CreateJobProcessor(job);
		}

		public virtual IStepHandler CreateStepHandler(MigrationWorkflowPosition position, IMigrationDataProvider dataProvider, MigrationJob migrationJob)
		{
			return MigrationWorkflowPosition.CreateStepHandler(position, dataProvider, migrationJob);
		}

		public virtual ISnapshotId GetStepSnapshotId(MigrationWorkflowPosition position, MigrationJobItem jobItem)
		{
			return MigrationWorkflowPosition.GetStepSnapshotId(position, jobItem);
		}

		public virtual MigrationWorkflow GetMigrationWorkflow(MigrationType type)
		{
			switch (type)
			{
			case MigrationType.XO1:
			case MigrationType.ExchangeOutlookAnywhere:
				return new MigrationWorkflow(MigrationWorkflow.DefaultProvisionAndMigrateWorkflowSteps);
			}
			return new MigrationWorkflow(MigrationWorkflow.DefaultMigrationWorkflowSteps);
		}

		public virtual IMigrationDataRowProvider GetMigrationDataRowProvider(MigrationJob job, IMigrationDataProvider dataProvider)
		{
			if (job.OriginalJobId != null)
			{
				return new MigrationPreexistingBatchCsvDataRowProvider(job, dataProvider);
			}
			MigrationType migrationType = job.MigrationType;
			if (migrationType <= MigrationType.ExchangeRemoteMove)
			{
				switch (migrationType)
				{
				case MigrationType.IMAP:
					return new IMAPCSVDataRowProvider(job, dataProvider);
				case MigrationType.XO1:
					return new XO1CSVDataRowProvider(job, dataProvider);
				case MigrationType.IMAP | MigrationType.XO1:
					goto IL_93;
				case MigrationType.ExchangeOutlookAnywhere:
					if (job.IsStaged)
					{
						return new NspiCsvMigrationDataRowProvider(job, dataProvider, false);
					}
					return new NspiMigrationDataRowProvider((ExchangeOutlookAnywhereEndpoint)job.SourceEndpoint, job, false);
				default:
					if (migrationType != MigrationType.ExchangeRemoteMove)
					{
						goto IL_93;
					}
					break;
				}
			}
			else if (migrationType != MigrationType.ExchangeLocalMove)
			{
				if (migrationType != MigrationType.PSTImport)
				{
					goto IL_93;
				}
				return new PSTCSVDataRowProvider(job, dataProvider);
			}
			return new MoveCsvDataRowProvider(job, dataProvider);
			IL_93:
			throw new NotSupportedException("Type " + job.MigrationType + " is unknown.");
		}

		public virtual IAutodiscoverService GetAutodiscoverService(ExchangeVersion exchangeVersion, NetworkCredential credentials)
		{
			MigrationUtil.ThrowOnNullArgument(credentials, "credentials");
			return new AutodiscoverProxyService(exchangeVersion, credentials);
		}

		public virtual IMigrationAutodiscoverClient GetAutodiscoverClient()
		{
			if (MigrationTestIntegration.Instance.IsMigrationProxyRpcClientEnabled)
			{
				return new MigrationExchangeProxyRpcClient();
			}
			return new MigrationAutodiscoverClient();
		}

		public virtual IMigrationNspiClient GetNspiClient(ReportData reportData)
		{
			if (MigrationTestIntegration.Instance.IsMigrationProxyRpcClientEnabled)
			{
				return new MigrationExchangeProxyRpcClient();
			}
			return new MigrationNspiClient(reportData);
		}

		public virtual IMigrationService GetMigrationServiceClient(string serverName)
		{
			return new MigrationServiceRpcStub(serverName);
		}

		public virtual IMigrationNotification GetMigrationNotificationClient(string serverName)
		{
			return new MigrationNotificationRpcStub(serverName);
		}

		internal virtual IMigrationRunspaceProxy CreateRunspaceForDatacenterAdmin(OrganizationId organizationId)
		{
			return MigrationRunspaceProxy.CreateRunspaceForDatacenterAdmin(organizationId);
		}

		internal virtual IMigrationMrsClient GetMigrationMrsClient()
		{
			return new MigrationExchangeProxyRpcClient();
		}

		internal virtual ICryptoAdapter GetCryptoAdapter()
		{
			return this.cryptoAdapter.Value;
		}

		internal virtual IAsyncNotificationAdapter GetAsyncNotificationAdapter()
		{
			if (!ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("MigrationAsyncNotificationEnabled"))
			{
				return AsyncNotificationAdapter.Empty;
			}
			return AsyncNotificationAdapter.Instance;
		}

		internal virtual SubscriptionAccessorBase GetSubscriptionAccessor(IMigrationDataProvider dataProvider, MigrationType migrationType, string jobName, bool isPAW, bool legacyManualSyncs = false)
		{
			SubscriptionAccessorBase subscriptionAccessorBase;
			if (TestSubscriptionProxyAccessor.TryCreate(out subscriptionAccessorBase))
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "MigrationServiceFactory:: overriding normal accessor with test accessor {0}", new object[]
				{
					subscriptionAccessorBase
				});
				return subscriptionAccessorBase;
			}
			return SubscriptionAccessorBase.CreateAccessor(dataProvider, migrationType, jobName, isPAW, legacyManualSyncs);
		}

		internal virtual MailboxSession.MailboxItemCountsAndSizes GetMailboxCountsAndSizes(Guid mailboxGuid, IMigrationADProvider adProvider)
		{
			ExchangePrincipal exchangePrincipalFromMbxGuid = adProvider.GetExchangePrincipalFromMbxGuid(mailboxGuid);
			MailboxSession.MailboxItemCountsAndSizes result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(exchangePrincipalFromMbxGuid, CultureInfo.InvariantCulture, "Client=MSExchangeSimpleMigration;Privilege:OpenAsSystemService"))
			{
				result = mailboxSession.ReadMailboxCountsAndSizes();
			}
			return result;
		}

		internal virtual void PublishNotification(string notification, string message)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.MailboxMigration.Name, ExchangeComponent.MailboxMigration.Name, notification, message, ResultSeverityLevel.Error);
			eventNotificationItem.Publish(false);
		}

		internal virtual IUpgradeConstraintAdapter GetUpgradeConstraintAdapter(MigrationSession migrationSession)
		{
			MigrationUtil.ThrowOnNullArgument(migrationSession, "migrationSession");
			if (migrationSession.IsSupported(MigrationFeature.UpgradeBlock))
			{
				return new OrganizationUpgradeConstraintAdapter();
			}
			return new NullUpgradeConstraintAdapter();
		}

		internal virtual IMigrationEmailHandler CreateEmailHandler(IMigrationDataProvider dataProvider)
		{
			IMigrationEmailHandler result;
			if (MigrationEmailHandlerProxy.TryCreate(MigrationTestIntegration.Instance.ReportMessageEndpoint, out result))
			{
				return result;
			}
			return new MigrationDataProviderEmailHandler(dataProvider);
		}

		internal virtual bool IsMultiTenantEnabled()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
		}

		private static ICryptoAdapter CreateCryptoAdapter()
		{
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationUseDKMForEncryption");
			bool flag;
			if (config > 0)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Setting encryption type based on MigrationUseDKMForEncryption configuration setting = {0}.", new object[]
				{
					config
				});
				flag = (config == 1);
			}
			else
			{
				MigrationLogger.Log(MigrationEventType.Information, "Setting encryption type based on runtime check", new object[0]);
				flag = VariantConfiguration.InvariantNoFlightingSnapshot.Global.DistributedKeyManagement.Enabled;
			}
			if (flag)
			{
				MigrationLogger.Log(MigrationEventType.Information, "Using DKM encryption (in Datacenter).", new object[0]);
				return new DkmAdapter();
			}
			MigrationLogger.Log(MigrationEventType.Information, "Using non-DKM encryption (in Enterprise).", new object[0]);
			return new CryptoAdapter();
		}

		private static MigrationServiceFactory instance = new MigrationServiceFactory();

		private readonly Lazy<ICryptoAdapter> cryptoAdapter;
	}
}
