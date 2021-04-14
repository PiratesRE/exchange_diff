using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetMailboxOrSyncMailbox : GetMailboxBase<MailboxIdParameter>
	{
		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				if (this.Arbitration)
				{
					return GetUser.ArbitrationAllowedRecipientTypeDetails;
				}
				if (this.PublicFolder)
				{
					return GetUser.PublicFolderAllowedRecipientTypeDetails;
				}
				if (this.Monitoring)
				{
					return GetUser.MonitoringAllowedRecipientTypeDetails;
				}
				if (this.AuditLog)
				{
					return GetUser.AuditLogAllowedRecipientTypeDetails;
				}
				return this.RecipientTypeDetails ?? RecipientConstants.GetMailboxOrSyncMailbox_AllowedRecipientTypeDetails;
			}
		}

		protected override string SystemAddressListRdn
		{
			get
			{
				bool flag = false;
				bool flag2 = false;
				if (this.RecipientTypeDetails != null && Array.IndexOf<RecipientTypeDetails>(this.RecipientTypeDetails, Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.TeamMailbox) >= 0)
				{
					flag2 = true;
					flag = (this.RecipientTypeDetails.Length == 1);
				}
				if (this.Arbitration.IsPresent || this.Monitoring.IsPresent || this.AuditLog.IsPresent || flag2)
				{
					return null;
				}
				if (flag)
				{
					return "TeamMailboxes(VLV)";
				}
				if (this.PublicFolder.IsPresent)
				{
					return "PublicFolderMailboxes(VLV)";
				}
				return "All Mailboxes(VLV)";
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = null;
				if (this.scopeObject is MailboxPlan)
				{
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlan, ((MailboxPlan)this.scopeObject).Id);
				}
				else if (this.scopeObject is Server)
				{
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ServerLegacyDN, ((Server)this.scopeObject).ExchangeLegacyDN);
				}
				else if (this.scopeObject is MailboxDatabase)
				{
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, ((MailboxDatabase)this.scopeObject).Id);
				}
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					queryFilter,
					this.Archive.IsPresent ? GetMailboxOrSyncMailbox.ArchiveFilter : null,
					this.RemoteArchive.IsPresent ? GetMailboxOrSyncMailbox.RemoteArchiveFilter : null,
					this.SoftDeletedMailbox.IsPresent ? GetMailboxOrSyncMailbox.SoftDeletedMailboxFilter : null,
					this.InactiveMailboxOnly.IsPresent ? GetMailboxOrSyncMailbox.InactiveMailboxFilter : null,
					this.AuxMailbox.IsPresent ? GetMailboxOrSyncMailbox.AuxMailboxFilter : new NotFilter(GetMailboxOrSyncMailbox.AuxMailboxFilter)
				});
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MailboxSchema>();
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(RecipientConstants.GetMailboxOrSyncMailbox_AllowedRecipientTypeDetails, value);
				base.Fields["RecipientTypeDetails"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "MailboxPlanSet", ValueFromPipeline = true)]
		public MailboxPlanIdParameter MailboxPlan
		{
			get
			{
				return (MailboxPlanIdParameter)base.Fields["MailboxPlan"];
			}
			set
			{
				base.Fields["MailboxPlan"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "ServerSet", ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(ParameterSetName = "DatabaseSet", ValueFromPipeline = true)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? false);
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter PublicFolder
		{
			get
			{
				return (SwitchParameter)(base.Fields["PublicFolder"] ?? false);
			}
			set
			{
				base.Fields["PublicFolder"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AuxMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuxMailbox"] ?? false);
			}
			set
			{
				base.Fields["AuxMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoteArchive
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoteArchive"] ?? false);
			}
			set
			{
				base.Fields["RemoteArchive"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SoftDeletedMailbox
		{
			get
			{
				return base.SoftDeletedObject;
			}
			set
			{
				base.SoftDeletedObject = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeSoftDeletedMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeSoftDeletedMailbox"] ?? false);
			}
			set
			{
				base.Fields["IncludeSoftDeletedMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter InactiveMailboxOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["InactiveMailboxOnly"] ?? false);
			}
			set
			{
				base.Fields["InactiveMailboxOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeInactiveMailbox
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeInactiveMailbox"] ?? false);
			}
			set
			{
				base.Fields["IncludeInactiveMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Monitoring
		{
			get
			{
				return (SwitchParameter)(base.Fields["Monitoring"] ?? false);
			}
			set
			{
				base.Fields["Monitoring"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AuditLog
		{
			get
			{
				return (SwitchParameter)(base.Fields["AuditLog"] ?? false);
			}
			set
			{
				base.Fields["AuditLog"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				base.OptionalIdentityData.AdditionalFilter,
				this.Archive.IsPresent ? GetMailboxOrSyncMailbox.ArchiveFilter : null,
				this.RemoteArchive.IsPresent ? GetMailboxOrSyncMailbox.RemoteArchiveFilter : null,
				this.SoftDeletedMailbox.IsPresent ? GetMailboxOrSyncMailbox.SoftDeletedMailboxFilter : null,
				this.InactiveMailboxOnly.IsPresent ? GetMailboxOrSyncMailbox.InactiveMailboxFilter : null,
				this.AuxMailbox.IsPresent ? GetMailboxOrSyncMailbox.AuxMailboxFilter : new NotFilter(GetMailboxOrSyncMailbox.AuxMailboxFilter)
			});
			base.InternalBeginProcessing();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			ADObjectId searchRoot = recipientSession.SearchRoot;
			if (this.SoftDeletedMailbox.IsPresent && base.CurrentOrganizationId != null && base.CurrentOrganizationId.OrganizationalUnit != null)
			{
				searchRoot = new ADObjectId("OU=Soft Deleted Objects," + base.CurrentOrganizationId.OrganizationalUnit.DistinguishedName);
			}
			if (base.ParameterSetName == "DatabaseSet" || base.ParameterSetName == "ServerSet" || base.ParameterSetName == "MailboxPlanSet" || this.SoftDeletedMailbox.IsPresent || this.IncludeSoftDeletedMailbox.IsPresent)
			{
				if (this.SoftDeletedMailbox.IsPresent || this.IncludeSoftDeletedMailbox.IsPresent)
				{
					recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(recipientSession.DomainController, searchRoot, recipientSession.Lcid, recipientSession.ReadOnly, recipientSession.ConsistencyMode, recipientSession.NetworkCredential, recipientSession.SessionSettings, ConfigScopes.TenantSubTree, 417, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\mailbox\\GetMailbox.cs");
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = recipientSession.EnforceDefaultScope;
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = recipientSession.UseGlobalCatalog;
				tenantOrRootOrgRecipientSession.LinkResolutionServer = recipientSession.LinkResolutionServer;
				recipientSession = tenantOrRootOrgRecipientSession;
			}
			if (this.IncludeInactiveMailbox.IsPresent)
			{
				recipientSession = SoftDeletedTaskHelper.CreateTenantOrRootOrgRecipientSessionIncludeInactiveMailbox(recipientSession, base.CurrentOrganizationId);
			}
			else if (this.InactiveMailboxOnly.IsPresent)
			{
				recipientSession = SoftDeletedTaskHelper.CreateTenantOrRootOrgRecipientSessionInactiveMailboxOnly(recipientSession, base.CurrentOrganizationId);
			}
			return recipientSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.MailboxPlan != null)
			{
				this.scopeObject = new MailboxPlan((ADUser)base.GetDataObject<ADUser>(this.MailboxPlan, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxPlanNotFound(this.MailboxPlan.ToString())), new LocalizedString?(Strings.ErrorMailboxPlanNotUnique(this.MailboxPlan.ToString()))));
			}
			else if (this.Server != null)
			{
				this.scopeObject = (Server)base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			}
			else if (this.Database != null)
			{
				if (MapiTaskHelper.IsDatacenter && base.AccountPartition == null)
				{
					this.WriteWarning(Strings.ImplicitPartitionIdSupplied(base.SessionSettings.PartitionId.ToString()));
				}
				this.Database.AllowInvalid = true;
				this.Database.AllowLegacy = true;
				this.scopeObject = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(this.Database, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(this.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(this.Database.ToString())));
			}
			base.CheckExclusiveParameters(new object[]
			{
				"IncludeSoftDeletedMailbox",
				"SoftDeletedMailbox",
				"IncludeInactiveMailbox",
				"InactiveMailboxOnly"
			});
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			ADUser user = (ADUser)dataObject;
			SharedConfiguration sharedConfig = null;
			if (SharedConfiguration.IsDehydratedConfiguration(user.OrganizationId) || (SharedConfiguration.GetSharedConfigurationState(user.OrganizationId) & SharedTenantConfigurationState.Static) != SharedTenantConfigurationState.UnSupported)
			{
				sharedConfig = base.ProvisioningCache.TryAddAndGetOrganizationData<SharedConfiguration>(CannedProvisioningCacheKeys.MailboxSharedConfigCacheKey, user.OrganizationId, () => SharedConfiguration.GetSharedConfiguration(user.OrganizationId));
			}
			if (null != user.MasterAccountSid)
			{
				user.LinkedMasterAccount = SecurityPrincipalIdParameter.GetFriendlyUserName(user.MasterAccountSid, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				user.ResetChangeTracking();
			}
			Mailbox mailbox = new Mailbox(user);
			mailbox.propertyBag.SetField(MailboxSchema.Database, ADObjectIdResolutionHelper.ResolveDN(mailbox.Database));
			if (sharedConfig != null)
			{
				mailbox.SharedConfiguration = sharedConfig.SharedConfigId.ConfigurationUnit;
				if (mailbox.RoleAssignmentPolicy == null)
				{
					mailbox.RoleAssignmentPolicy = base.ProvisioningCache.TryAddAndGetOrganizationData<ADObjectId>(CannedProvisioningCacheKeys.MailboxRoleAssignmentPolicyCacheKey, user.OrganizationId, () => sharedConfig.GetSharedRoleAssignmentPolicy());
				}
			}
			else if (mailbox.RoleAssignmentPolicy == null && !mailbox.ExchangeVersion.IsOlderThan(MailboxSchema.RoleAssignmentPolicy.VersionAdded))
			{
				ADObjectId defaultRoleAssignmentPolicy = RBACHelper.GetDefaultRoleAssignmentPolicy(user.OrganizationId);
				if (defaultRoleAssignmentPolicy != null)
				{
					mailbox.RoleAssignmentPolicy = defaultRoleAssignmentPolicy;
				}
			}
			if (mailbox.SharingPolicy == null && !mailbox.propertyBag.IsReadOnlyProperty(MailboxSchema.SharingPolicy))
			{
				mailbox.SharingPolicy = base.GetDefaultSharingPolicyId(user, sharedConfig);
			}
			if (mailbox.RetentionPolicy == null && mailbox.ShouldUseDefaultRetentionPolicy && !mailbox.propertyBag.IsReadOnlyProperty(MailboxSchema.RetentionPolicy))
			{
				mailbox.RetentionPolicy = base.GetDefaultRetentionPolicyId(user, sharedConfig);
			}
			if (mailbox.Database != null && mailbox.UseDatabaseRetentionDefaults)
			{
				this.SetDefaultRetentionValues(mailbox);
			}
			mailbox.AdminDisplayVersion = Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.GetServerVersion(mailbox.ServerName);
			return mailbox;
		}

		private void SetDefaultRetentionValues(Mailbox mailbox)
		{
			bool flag = mailbox.propertyBag.IsReadOnlyProperty(MailboxSchema.RetainDeletedItemsFor);
			bool flag2 = mailbox.propertyBag.IsReadOnlyProperty(MailboxSchema.RetainDeletedItemsUntilBackup);
			if (flag && flag2)
			{
				return;
			}
			MailboxDatabase mailboxDatabase;
			if (this.Database != null)
			{
				mailboxDatabase = (MailboxDatabase)this.scopeObject;
			}
			else
			{
				DatabaseIdParameter databaseIdParam = new DatabaseIdParameter(mailbox.Database);
				string subKey = databaseIdParam.ToString();
				mailboxDatabase = base.ProvisioningCache.TryAddAndGetGlobalDictionaryValue<MailboxDatabase, string>(CannedProvisioningCacheKeys.MailboxDatabaseForDefaultRetentionValuesCacheKey, subKey, () => (MailboxDatabase)this.GetDataObject<MailboxDatabase>(databaseIdParam, this.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(mailbox.Database.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(mailbox.Database.ToString()))));
			}
			if (!flag)
			{
				mailbox.RetainDeletedItemsFor = mailboxDatabase.DeletedItemRetention;
			}
			if (!flag2)
			{
				mailbox.RetainDeletedItemsUntilBackup = mailboxDatabase.RetainDeletedItemsUntilBackup;
			}
		}

		internal static readonly QueryFilter RemoteArchiveFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveStatus, ArchiveStatusFlags.Active),
			new BitMaskAndFilter(ADUserSchema.RemoteRecipientType, 2UL)
		});

		private static readonly QueryFilter SoftDeletedMailboxFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.IsSoftDeletedByRemove, true);

		private static readonly QueryFilter InactiveMailboxFilter = new BitMaskAndFilter(ADRecipientSchema.RecipientSoftDeletedStatus, 8UL);

		private static readonly QueryFilter ArchiveFilter = new ExistsFilter(ADUserSchema.ArchiveGuid);

		private static readonly QueryFilter AuxMailboxFilter = new BitMaskAndFilter(ADRecipientSchema.ProvisioningFlags, 131072UL);

		private ADObject scopeObject;
	}
}
