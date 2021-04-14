using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Management.Migration
{
	public abstract class MigrationObjectTaskBase<TIdentityParameter> : ObjectActionTenantADTask<TIdentityParameter, MigrationBatch> where TIdentityParameter : IIdentityParameter, new()
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override TIdentityParameter Identity
		{
			get
			{
				TIdentityParameter tidentityParameter = (TIdentityParameter)((object)base.Fields["Identity"]);
				if (tidentityParameter == null)
				{
					tidentityParameter = ((default(TIdentityParameter) == null) ? Activator.CreateInstance<TIdentityParameter>() : default(TIdentityParameter));
				}
				return tidentityParameter;
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Partition
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Partition"];
			}
			set
			{
				base.Fields["Partition"] = value;
			}
		}

		public abstract string Action { get; }

		protected string TenantName
		{
			get
			{
				if (!(base.CurrentOrganizationId != null) || base.CurrentOrganizationId.OrganizationalUnit == null)
				{
					return string.Empty;
				}
				return base.CurrentOrganizationId.OrganizationalUnit.Name;
			}
		}

		internal static void RegisterMigrationBatch(Task task, MailboxSession mailboxSession, OrganizationId organizationId, bool failIfNotConnected, bool refresh = false)
		{
			string serverFqdn = mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn;
			Guid mdbGuid = mailboxSession.MdbGuid;
			string mailboxOwnerLegacyDN = mailboxSession.MailboxOwnerLegacyDN;
			ADObjectId organizationName = organizationId.OrganizationalUnit ?? new ADObjectId();
			int num = 2;
			for (int i = 1; i <= num; i++)
			{
				try
				{
					MigrationNotificationRpcStub migrationNotificationRpcStub = new MigrationNotificationRpcStub(serverFqdn);
					migrationNotificationRpcStub.RegisterMigrationBatch(new RegisterMigrationBatchArgs(mdbGuid, mailboxOwnerLegacyDN, organizationName, refresh));
					break;
				}
				catch (MigrationServiceRpcException ex)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_MigrationServiceConnectionError, new string[]
					{
						serverFqdn,
						ex.Message
					});
					if (i == num && failIfNotConnected)
					{
						MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(ex, null), new object[0]);
						task.WriteError(new LocalizedException(Strings.MigrationOperationFailed, null), ExchangeErrorCategory.Client, null);
					}
				}
			}
		}

		internal static ADUser ResolvePartitionMailbox(MailboxIdParameter partitionMailboxIdentity, IRecipientSession tenantGlobalCatalogSession, ADServerSettings serverSettings, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError, bool datacenterFirstOrg)
		{
			ADUser aduser;
			if (partitionMailboxIdentity != null)
			{
				ADObjectId rootID = null;
				if (datacenterFirstOrg)
				{
					rootID = ADSystemConfigurationSession.GetFirstOrgUsersContainerId();
				}
				aduser = (ADUser)getDataObject(partitionMailboxIdentity, tenantGlobalCatalogSession, rootID, null, new LocalizedString?(Strings.MigrationPartitionMailboxNotFound), new LocalizedString?(Strings.MigrationPartitionMailboxAmbiguous), ExchangeErrorCategory.Client);
				if (!aduser.PersistedCapabilities.Contains(Capability.OrganizationCapabilityMigration))
				{
					writeError(new MigrationPartitionMailboxInvalidException(aduser.Alias), ExchangeErrorCategory.Client, partitionMailboxIdentity);
				}
			}
			else
			{
				List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(tenantGlobalCatalogSession, OrganizationCapability.Migration);
				if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
				{
					organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(tenantGlobalCatalogSession, OrganizationCapability.Management);
				}
				if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
				{
					writeError(new MigrationPartitionMailboxNotFoundException(), ExchangeErrorCategory.Client, null);
				}
				else if (organizationMailboxesByCapability.Count > 1)
				{
					writeError(new MigrationPartitionMailboxAmbiguousException(), ExchangeErrorCategory.Client, null);
				}
				aduser = organizationMailboxesByCapability[0];
			}
			if (aduser.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox || aduser.Database == null)
			{
				writeError(new MigrationPartitionMailboxInvalidException(aduser.Alias), ExchangeErrorCategory.Client, partitionMailboxIdentity);
			}
			return aduser;
		}

		internal static MigrationJob GetAndValidateMigrationJob(Task task, MigrationBatchDataProvider batchProvider, MigrationBatchIdParameter identity, bool skipCorrupt, bool failIfNotFound = true)
		{
			MigrationObjectTaskBase<TIdentityParameter>.ValidateIdentity(task, batchProvider, identity);
			return MigrationObjectTaskBase<TIdentityParameter>.GetMigrationJobByBatchId(task, batchProvider, identity.MigrationBatchId, skipCorrupt, failIfNotFound);
		}

		internal static void ValidateIdentity(Task task, MigrationBatchDataProvider batchProvider, MigrationBatchIdParameter identity)
		{
			if (!batchProvider.MigrationSession.Config.IsSupported(MigrationFeature.MultiBatch))
			{
				return;
			}
			if (identity == null || identity.MigrationBatchId == null || identity.MigrationBatchId.Name == MigrationBatchId.Any.ToString())
			{
				task.WriteError(new MigrationPermanentException(Strings.MigrationBatchIdMissing), (ErrorCategory)1000, null);
			}
		}

		internal static SubmittedByUserAdminType GetUserType(ExchangeRunspaceConfiguration configuration, OrganizationId organizationId)
		{
			if (configuration == null)
			{
				return SubmittedByUserAdminType.DataCenterAdmin;
			}
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				if (configuration.PartnerMode)
				{
					return SubmittedByUserAdminType.Partner;
				}
				return SubmittedByUserAdminType.DataCenterAdmin;
			}
			else
			{
				if (configuration.DelegatedPrincipal != null)
				{
					return SubmittedByUserAdminType.PartnerTenant;
				}
				return SubmittedByUserAdminType.TenantAdmin;
			}
		}

		internal static LocalizedException ProcessCsv(IMigrationDataProvider dataProvider, MigrationBatch batch, MigrationCsvSchemaBase schema, byte[] csvData)
		{
			MigrationBatchCsvProcessor migrationBatchCsvProcessor = (batch.MigrationType == MigrationType.PublicFolder) ? new PublicFolderMigrationBatchCsvProcessor((PublicFolderMigrationCsvSchema)schema, dataProvider) : new MigrationBatchCsvProcessor(schema);
			return migrationBatchCsvProcessor.ProcessCsv(batch, csvData);
		}

		internal static void StartJob(Task task, MigrationBatchDataProvider batchProvider, MigrationJob job, MultiValuedProperty<SmtpAddress> notificationEmails, MigrationBatchFlags batchFlags)
		{
			MigrationHelper.RunUpdateOperation(delegate
			{
				job.StartJob(batchProvider.MailboxProvider, notificationEmails, batchFlags, null);
			});
		}

		internal static MultiValuedProperty<SmtpAddress> GetUpdatedNotificationEmails(Task task, IRecipientSession recipientSession, IEnumerable<SmtpAddress> originalEmails)
		{
			MultiValuedProperty<SmtpAddress> multiValuedProperty = new MultiValuedProperty<SmtpAddress>();
			ADObjectId entryId;
			if (task.TryGetExecutingUserId(out entryId))
			{
				ADUser aduser = (ADUser)recipientSession.Read(entryId);
				if (aduser != null)
				{
					SmtpAddress windowsEmailAddress = aduser.WindowsEmailAddress;
					if (windowsEmailAddress.Length > 0)
					{
						multiValuedProperty.Add(windowsEmailAddress);
					}
				}
			}
			else if (task.ExchangeRunspaceConfig.DelegatedPrincipal != null)
			{
				multiValuedProperty.Add(new SmtpAddress(task.ExchangeRunspaceConfig.DelegatedPrincipal.UserId));
			}
			if (originalEmails == null && multiValuedProperty.Count <= 0)
			{
				return null;
			}
			if (originalEmails != null)
			{
				foreach (SmtpAddress item in originalEmails)
				{
					if (!multiValuedProperty.Contains(item))
					{
						multiValuedProperty.Add(item);
					}
				}
			}
			return multiValuedProperty;
		}

		internal static void WriteJobNotFoundError(Task task, string identity)
		{
			MigrationObjectTaskBase<TIdentityParameter>.WriteJobNotFoundError(task, identity, null);
		}

		internal static void WriteJobNotFoundError(Task task, string jobName, Exception ex)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(jobName, "jobName");
			task.WriteError(new MigrationBatchNotFoundException(jobName, ex), ExchangeErrorCategory.Client, null);
		}

		internal MigrationJob GetAndValidateMigrationJob(bool skipCorrupt)
		{
			MigrationBatchDataProvider batchProvider = (MigrationBatchDataProvider)base.DataSession;
			return MigrationObjectTaskBase<TIdentityParameter>.GetAndValidateMigrationJob(this, batchProvider, (MigrationBatchIdParameter)((object)this.Identity), skipCorrupt, true);
		}

		internal MultiValuedProperty<SmtpAddress> GetUpdatedNotificationEmails(MultiValuedProperty<SmtpAddress> originalEmails)
		{
			return MigrationObjectTaskBase<TIdentityParameter>.GetUpdatedNotificationEmails(this, base.TenantGlobalCatalogSession, originalEmails);
		}

		protected override void InternalBeginProcessing()
		{
			if (this.Organization != null)
			{
				base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			}
			else if (!MapiTaskHelper.IsDatacenter)
			{
				base.CurrentOrganizationId = OrganizationId.ForestWideOrgId;
			}
			this.partitionMailbox = MigrationObjectTaskBase<TIdentityParameter>.ResolvePartitionMailbox(this.Partition, base.TenantGlobalCatalogSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.WriteError), base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && MapiTaskHelper.IsDatacenter);
			base.InternalBeginProcessing();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			migrationBatchDataProvider.MailboxProvider.FlushReport(migrationBatchDataProvider.MigrationJob.ReportData);
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = base.MyInvocation.MyCommand.Name;
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationBatchDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, null, this.partitionMailbox);
		}

		protected override void InternalValidate()
		{
			MigrationBatchDataProvider migrationBatchDataProvider = (MigrationBatchDataProvider)base.DataSession;
			IMigrationADProvider adprovider = migrationBatchDataProvider.MailboxProvider.ADProvider;
			if (!adprovider.IsMigrationEnabled)
			{
				this.WriteError(new MigrationPermanentException(Strings.MigrationNotEnabledForTenant(this.TenantName)));
			}
			base.InternalValidate();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalStateReset()
		{
			this.DisposeSession();
			base.InternalStateReset();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.DisposeSession();
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected void WriteError(LocalizedException exception)
		{
			MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(exception, null), new object[0]);
			base.WriteError(exception, (ErrorCategory)1000, null);
		}

		internal static MigrationJob GetMigrationJobByBatchId(Task task, MigrationBatchDataProvider batchProvider, MigrationBatchId migrationBatchId, bool skipCorrupt, bool failIfNotFound = true)
		{
			MigrationJob migrationJob = null;
			Exception ex = null;
			try
			{
				migrationJob = batchProvider.JobCache.GetJob(migrationBatchId);
			}
			catch (PropertyErrorException ex2)
			{
				ex = ex2;
			}
			catch (InvalidDataException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(ex, null), new object[0]);
				task.WriteError(new MigrationPermanentException(Strings.MigrationJobCorrupted, ex), ExchangeErrorCategory.Client, null);
			}
			if (migrationJob != null && migrationJob.Status == MigrationJobStatus.Corrupted && skipCorrupt)
			{
				migrationJob = null;
			}
			if (migrationJob == null && failIfNotFound)
			{
				MigrationObjectTaskBase<TIdentityParameter>.WriteJobNotFoundError(task, migrationBatchId.ToString());
			}
			return migrationJob;
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(base.DomainController, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential);
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 695, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Migration\\MigrationObjectTaskBase.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
			return adorganizationalUnit.OrganizationId;
		}

		private void DisposeSession()
		{
			if (base.DataSession is IDisposable)
			{
				MigrationLogger.Close();
				((IDisposable)base.DataSession).Dispose();
			}
		}

		private bool disposed;

		protected ADUser partitionMailbox;
	}
}
