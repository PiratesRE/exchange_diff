using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ExchangeConfigurationUnit : Organization, IProvisioningCacheInvalidation
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeConfigurationUnit.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeConfigurationUnit.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2007;
			}
		}

		public MultiValuedProperty<DNWithBinary> OtherWellKnownObjects
		{
			get
			{
				return (MultiValuedProperty<DNWithBinary>)this.propertyBag[ExchangeConfigurationUnitSchema.OtherWellKnownObjects];
			}
			internal set
			{
				this.propertyBag[ExchangeConfigurationUnitSchema.OtherWellKnownObjects] = value;
			}
		}

		public bool IsFederated
		{
			get
			{
				return (bool)this[OrganizationSchema.IsFederated];
			}
			internal set
			{
				this[OrganizationSchema.IsFederated] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHotmailMigration
		{
			get
			{
				return (bool)this[OrganizationSchema.IsHotmailMigration];
			}
			set
			{
				this[OrganizationSchema.IsHotmailMigration] = value;
			}
		}

		public bool HideAdminAccessWarning
		{
			get
			{
				return (bool)this[OrganizationSchema.HideAdminAccessWarning];
			}
			internal set
			{
				this[OrganizationSchema.HideAdminAccessWarning] = value;
			}
		}

		public bool SMTPAddressCheckWithAcceptedDomain
		{
			get
			{
				return (bool)this[OrganizationSchema.SMTPAddressCheckWithAcceptedDomain];
			}
			internal set
			{
				this[OrganizationSchema.SMTPAddressCheckWithAcceptedDomain] = value;
			}
		}

		public bool ShowAdminAccessWarning
		{
			get
			{
				return (bool)this[OrganizationSchema.ShowAdminAccessWarning];
			}
		}

		public bool SkipToUAndParentalControlCheck
		{
			get
			{
				return (bool)this[OrganizationSchema.SkipToUAndParentalControlCheck];
			}
			internal set
			{
				this[OrganizationSchema.SkipToUAndParentalControlCheck] = value;
			}
		}

		public bool IsUpgradingOrganization
		{
			get
			{
				return (bool)this[OrganizationSchema.IsUpgradingOrganization];
			}
			internal set
			{
				this[OrganizationSchema.IsUpgradingOrganization] = value;
			}
		}

		public bool IsPilotingOrganization
		{
			get
			{
				return (bool)this[OrganizationSchema.IsPilotingOrganization];
			}
			internal set
			{
				this[OrganizationSchema.IsPilotingOrganization] = value;
			}
		}

		public bool IsUpdatingServicePlan
		{
			get
			{
				return (bool)this[OrganizationSchema.IsUpdatingServicePlan];
			}
			internal set
			{
				this[OrganizationSchema.IsUpdatingServicePlan] = value;
			}
		}

		internal bool IsStaticConfigurationShared
		{
			get
			{
				return (bool)this[OrganizationSchema.IsStaticConfigurationShared];
			}
			set
			{
				this[OrganizationSchema.IsStaticConfigurationShared] = value;
			}
		}

		public bool IsUpgradeOperationInProgress
		{
			get
			{
				return (bool)this[OrganizationSchema.IsUpgradeOperationInProgress];
			}
			internal set
			{
				this[OrganizationSchema.IsUpgradeOperationInProgress] = value;
			}
		}

		public bool SyncMEUSMTPToMServ
		{
			get
			{
				return (int)this[OrganizationSchema.ObjectVersion] < 13000 || (bool)this[OrganizationSchema.SMTPAddressCheckWithAcceptedDomain];
			}
		}

		[Parameter(Mandatory = false)]
		public bool SyncMBXAndDLToMServ
		{
			get
			{
				return (bool)this[OrganizationSchema.SyncMBXAndDLToMServ];
			}
			set
			{
				this[OrganizationSchema.SyncMBXAndDLToMServ] = value;
			}
		}

		public bool IsOrganizationReadyForMservSync
		{
			get
			{
				return this.OrganizationStatus != OrganizationStatus.Invalid && this.OrganizationStatus != OrganizationStatus.PendingCompletion;
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[OrganizationSchema.ReleaseTrack];
			}
			set
			{
				this[OrganizationSchema.ReleaseTrack] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public OrganizationStatus OrganizationStatus
		{
			get
			{
				return (OrganizationStatus)((int)this[ExchangeConfigurationUnitSchema.OrganizationStatus]);
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.OrganizationStatus] = (int)value;
				this.WhenOrganizationStatusSet = new DateTime?(DateTime.UtcNow);
			}
		}

		public DateTime? WhenOrganizationStatusSet
		{
			get
			{
				return (DateTime?)this[ExchangeConfigurationUnitSchema.WhenOrganizationStatusSet];
			}
			private set
			{
				this[ExchangeConfigurationUnitSchema.WhenOrganizationStatusSet] = value;
			}
		}

		public new OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[ExchangeConfigurationUnitSchema.OrganizationId];
			}
		}

		[Parameter(Mandatory = false)]
		public string IOwnMigrationTenant
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.IOwnMigrationTenant];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.IOwnMigrationTenant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string IOwnMigrationStatusReport
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.IOwnMigrationStatusReport];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.IOwnMigrationStatusReport] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
		{
			get
			{
				return (IOwnMigrationStatusFlagsEnum)((byte)this[ExchangeConfigurationUnitSchema.IOwnMigrationStatus]);
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.IOwnMigrationStatus] = value;
			}
		}

		public string ServicePlan
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.ServicePlan];
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.ServicePlan] = value;
			}
		}

		public string TargetServicePlan
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.TargetServicePlan];
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.TargetServicePlan] = value;
			}
		}

		public string ProgramId
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.ProgramId];
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.ProgramId] = value;
			}
		}

		public string OfferId
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.OfferId];
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.OfferId] = value;
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		internal string ResellerId
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.ResellerId];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.ResellerId] = value;
			}
		}

		internal string DirSyncId
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.DirSyncId];
			}
		}

		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[OrganizationSchema.ExcludedFromBackSync];
			}
			set
			{
				this[OrganizationSchema.ExcludedFromBackSync] = value;
			}
		}

		public bool ExcludedFromForwardSyncEDU2BPOS
		{
			get
			{
				return (bool)this[OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS];
			}
			set
			{
				this[OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MSOSyncEnabled
		{
			get
			{
				return (bool)this[OrganizationSchema.MSOSyncEnabled];
			}
			set
			{
				this[OrganizationSchema.MSOSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderIssueWarningQuota];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderIssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderMaxItemSize];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderMaxItemSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[OrganizationSchema.DefaultPublicFolderProhibitPostQuota];
			}
			set
			{
				this[OrganizationSchema.DefaultPublicFolderProhibitPostQuota] = value;
			}
		}

		internal ADObjectId ExchangeUpgradeBucket
		{
			get
			{
				return (ADObjectId)this[ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket] = value;
			}
		}

		internal ExchangeObjectVersion AdminDisplayVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[OrganizationSchema.AdminDisplayVersion];
			}
			set
			{
				this[OrganizationSchema.AdminDisplayVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSyncRunning
		{
			get
			{
				return (bool)this[OrganizationSchema.IsDirSyncRunning];
			}
			set
			{
				this[OrganizationSchema.IsDirSyncRunning] = value;
			}
		}

		public bool IsDirSyncStatusPending
		{
			get
			{
				return (bool)this[OrganizationSchema.IsDirSyncStatusPending];
			}
			private set
			{
				this[OrganizationSchema.IsDirSyncStatusPending] = value;
			}
		}

		internal new OrganizationConfigXML ConfigXML
		{
			get
			{
				if (base.ConfigXML == null)
				{
					base.ConfigXML = new OrganizationConfigXML();
				}
				return base.ConfigXML;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeStatusTypes UpgradeStatus
		{
			get
			{
				return (UpgradeStatusTypes)this[OrganizationSchema.UpgradeStatus];
			}
			set
			{
				this[OrganizationSchema.UpgradeStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return (UpgradeRequestTypes)this[OrganizationSchema.UpgradeRequest];
			}
			set
			{
				this[OrganizationSchema.UpgradeRequest] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> CompanyTags
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.CompanyTags];
			}
			set
			{
				this[OrganizationSchema.CompanyTags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Location
		{
			get
			{
				return (string)this[OrganizationSchema.Location];
			}
			set
			{
				this[OrganizationSchema.Location] = value;
			}
		}

		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[OrganizationSchema.PersistedCapabilities];
			}
			set
			{
				this[OrganizationSchema.PersistedCapabilities] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncStatus
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.DirSyncStatus];
			}
			set
			{
				if (value != null)
				{
					foreach (string text in value)
					{
						if (!string.IsNullOrEmpty(text) && text.StartsWith("Pending", StringComparison.OrdinalIgnoreCase))
						{
							this[OrganizationSchema.IsDirSyncStatusPending] = true;
							break;
						}
						this[OrganizationSchema.IsDirSyncStatusPending] = false;
					}
				}
				this[OrganizationSchema.DirSyncStatus] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AsynchronousOperationIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.AsynchronousOperationIds];
			}
			set
			{
				this[OrganizationSchema.AsynchronousOperationIds] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DirSyncServiceInstance
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.DirSyncServiceInstance];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.DirSyncServiceInstance] = value;
			}
		}

		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[OrganizationSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[OrganizationSchema.MailboxRelease] = value.ToString();
			}
		}

		public MailboxRelease PreviousMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[OrganizationSchema.PreviousMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[OrganizationSchema.PreviousMailboxRelease] = value.ToString();
			}
		}

		public MailboxRelease PilotMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[OrganizationSchema.PilotMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				this[OrganizationSchema.PilotMailboxRelease] = value.ToString();
			}
		}

		internal ADObjectId OrganizationalUnitLink
		{
			get
			{
				return ExchangeConfigurationUnit.OrganizationalUnitLinkGetter(this.propertyBag);
			}
			set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
				if (value == null)
				{
					this[ADObjectSchema.ConfigurationUnit] = null;
				}
				else
				{
					this[ADObjectSchema.ConfigurationUnit] = this[ADObjectSchema.Id];
				}
				MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)this[ExchangeConfigurationUnitSchema.OrganizationalUnitLink];
				if (multiValuedProperty.Count == 0)
				{
					multiValuedProperty.Add(value);
					return;
				}
				multiValuedProperty[0] = value;
			}
		}

		internal new ADObjectId OrganizationalUnitRoot
		{
			get
			{
				return ((OrganizationId)this[ExchangeConfigurationUnitSchema.OrganizationId]).OrganizationalUnit;
			}
		}

		internal new ADObjectId ConfigurationUnit
		{
			get
			{
				return ((OrganizationId)this[ExchangeConfigurationUnitSchema.OrganizationId]).ConfigurationUnit;
			}
		}

		internal bool IsDirSyncEnabled
		{
			get
			{
				List<DirSyncState> list = new List<DirSyncState>();
				list.Add(DirSyncState.Disabled);
				return ExchangeConfigurationUnit.IsOrganizationDirSyncRunning(this.IsDirSyncRunning, this.DirSyncStatus, list);
			}
		}

		internal bool IsTemplateTenant
		{
			get
			{
				return (bool)this[OrganizationSchema.IsTemplateTenant];
			}
			set
			{
				this[OrganizationSchema.IsTemplateTenant] = value;
			}
		}

		internal TenantRelocationStatus RelocationStatus
		{
			get
			{
				return (TenantRelocationStatus)((byte)this[ExchangeConfigurationUnitSchema.RelocationStatus]);
			}
		}

		internal Schedule SafeLockdownSchedule
		{
			get
			{
				return (Schedule)this[ExchangeConfigurationUnitSchema.SafeLockdownSchedule];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.SafeLockdownSchedule] = value;
			}
		}

		internal byte[] TenantRelocationCompletionTargetVector
		{
			get
			{
				return (byte[])this[ExchangeConfigurationUnitSchema.TenantRelocationCompletionTargetVector];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.TenantRelocationCompletionTargetVector] = value;
			}
		}

		internal string TargetForest
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.TargetForest];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.TargetForest] = value;
			}
		}

		internal string SourceForest
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.SourceForest];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.SourceForest] = value;
			}
		}

		internal string RelocationSourceForestRaw
		{
			get
			{
				return (string)this[ExchangeConfigurationUnitSchema.RelocationSourceForestRaw];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.RelocationSourceForestRaw] = value;
			}
		}

		internal static bool RelocationInProgress(ExchangeConfigurationUnit cu)
		{
			return !string.IsNullOrEmpty(cu.TargetForest);
		}

		internal static bool IsBeingDeleted(OrganizationStatus status)
		{
			return status == OrganizationStatus.PendingRemoval || status == OrganizationStatus.SoftDeleted || status == OrganizationStatus.ReadyForRemoval;
		}

		internal static bool IsOrganizationActive(OrganizationStatus status)
		{
			return status == OrganizationStatus.Active || status == OrganizationStatus.Suspended || status == OrganizationStatus.LockedOut;
		}

		internal static bool IsInactiveRelocationNode(ExchangeConfigurationUnit cu)
		{
			TenantRelocationState tenantRelocationState;
			bool flag;
			return (!string.IsNullOrEmpty(cu.TargetForest) && cu.RelocationStatus == TenantRelocationStatus.Retired) || (!string.IsNullOrEmpty(cu.RelocationSourceForestRaw) && cu.RelocationStatusDetailsRaw != RelocationStatusDetails.Active && TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(cu.Id, out tenantRelocationState, out flag) && tenantRelocationState.SourceForestState != TenantRelocationStatus.Retired);
		}

		internal bool AutoCompletionEnabled
		{
			get
			{
				return (bool)this[ExchangeConfigurationUnitSchema.AutoCompletionEnabled];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.AutoCompletionEnabled] = value;
			}
		}

		internal bool LargeTenantModeEnabled
		{
			get
			{
				return (bool)this[ExchangeConfigurationUnitSchema.LargeTenantModeEnabled];
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.LargeTenantModeEnabled] = value;
			}
		}

		internal RelocationStatusDetailsSource RelocationStatusDetailsSource
		{
			get
			{
				return (RelocationStatusDetailsSource)((byte)this[ExchangeConfigurationUnitSchema.RelocationStatusDetailsSource]);
			}
		}

		internal RelocationStatusDetails RelocationStatusDetailsRaw
		{
			get
			{
				return (RelocationStatusDetails)((byte)this[ExchangeConfigurationUnitSchema.RelocationStatusDetailsRaw]);
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.RelocationStatusDetailsRaw] = value;
			}
		}

		public RelocationStatusDetailsDestination RelocationStatusDetailsDestination
		{
			get
			{
				return (RelocationStatusDetailsDestination)((byte)this[ExchangeConfigurationUnitSchema.RelocationStatusDetailsDestination]);
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.RelocationStatusDetailsDestination] = value;
			}
		}

		internal RelocationStateRequested RelocationStateRequested
		{
			get
			{
				return (RelocationStateRequested)((int)this[ExchangeConfigurationUnitSchema.RelocationStateRequested]);
			}
			set
			{
				this[ExchangeConfigurationUnitSchema.RelocationStateRequested] = (int)value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			ADObjectId organizationalUnitLink = this.OrganizationalUnitLink;
			if (organizationalUnitLink != null && !string.IsNullOrEmpty(organizationalUnitLink.DistinguishedName) && !organizationalUnitLink.DistinguishedName.ToUpper().StartsWith("OU="))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidOrganizationId(base.Id.DistinguishedName, organizationalUnitLink.DistinguishedName, base.Id.DistinguishedName), this.Identity, string.Empty));
			}
			if (((base.SupportedSharedConfigurations != null && base.SupportedSharedConfigurations.Count != 0) || base.SharedConfigurationInfo != null) && (string.IsNullOrEmpty(this.OfferId) || string.IsNullOrEmpty(this.ProgramId)))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorOfferProgramIdMandatoryOnSharedConfig, this.Identity, string.Empty));
			}
			if (base.IsDehydrated || this.IsStaticConfigurationShared)
			{
				if (base.SupportedSharedConfigurations.Count == 0)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorIsDehydratedSetOnNonTinyTenant, this.Identity, string.Empty));
					return;
				}
			}
			else if (base.SupportedSharedConfigurations != null && base.SupportedSharedConfigurations.Count != 0)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorNonTinyTenantShouldNotHaveSharedConfig, this.Identity, string.Empty));
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(OrganizationSchema.DefaultPublicFolderProhibitPostQuota))
			{
				this.DefaultPublicFolderProhibitPostQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromGB(2UL));
			}
			if (!base.IsModified(OrganizationSchema.DefaultPublicFolderIssueWarningQuota))
			{
				this.DefaultPublicFolderIssueWarningQuota = this.DefaultPublicFolderProhibitPostQuota * 85 / 100;
			}
			base.StampPersistableDefaultValues();
		}

		internal static object CuOrganizationIdGetter(IPropertyBag propertyBag)
		{
			OrganizationId organizationId = (OrganizationId)ADObject.OrganizationIdGetter(propertyBag);
			if (organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				ADObjectId adobjectId = ExchangeConfigurationUnit.OrganizationalUnitLinkGetter(propertyBag);
				if (adobjectId != null)
				{
					organizationId = new OrganizationId(adobjectId, (ADObjectId)propertyBag[ADObjectSchema.Id]);
				}
			}
			return organizationId;
		}

		internal static SinglePropertyFilter OrganizationNameFilterBuilder(SinglePropertyFilter filter)
		{
			if (!(filter is ComparisonFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			string arg = (string)((ComparisonFilter)filter).PropertyValue;
			string propertyValue = string.Format("/o=First Organization/ou=Exchange Administrative Group (FYDIBOHF23SPDLT)/cn={0}/cn=Configuration", arg);
			return new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.LegacyExchangeDN, propertyValue);
		}

		internal static string OrganizationNameGetter(IPropertyBag propertyBag)
		{
			string result = null;
			if (propertyBag[OrganizationSchema.LegacyExchangeDN] != null)
			{
				result = (string)propertyBag[OrganizationSchema.LegacyExchangeDN];
			}
			return result;
		}

		private static ADObjectId OrganizationalUnitLinkGetter(IPropertyBag propertyBag)
		{
			ADObjectId result = null;
			if (propertyBag[ExchangeConfigurationUnitSchema.OrganizationalUnitLink] != null)
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)propertyBag[ExchangeConfigurationUnitSchema.OrganizationalUnitLink];
				if (multiValuedProperty.Count > 0)
				{
					result = multiValuedProperty[0];
				}
			}
			return result;
		}

		internal static string ProgramIdGetter(IPropertyBag propertyBag)
		{
			string result = null;
			if (propertyBag[ExchangeConfigurationUnitSchema.ResellerId] != null)
			{
				string text = (string)propertyBag[ExchangeConfigurationUnitSchema.ResellerId];
				int num = text.IndexOf(".");
				if (num > 1)
				{
					result = text.Substring(0, num);
				}
			}
			return result;
		}

		internal static QueryFilter ProgramIdFilterBuilder(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
			}
			return new TextFilter(ExchangeConfigurationUnitSchema.ResellerId, (((string)comparisonFilter.PropertyValue) ?? string.Empty) + ".", MatchOptions.Prefix, MatchFlags.IgnoreCase);
		}

		internal static QueryFilter DirSyncServiceInstanceFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				object propertyValue = comparisonFilter.PropertyValue;
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw, (propertyValue != null) ? propertyValue.ToString().ToLower() : null);
			}
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw);
			}
			if (filter is TextFilter)
			{
				TextFilter textFilter = filter as TextFilter;
				string text = textFilter.Text;
				return new TextFilter(ExchangeConfigurationUnitSchema.DirSyncServiceInstanceRaw, (text != null) ? text.ToLower() : null, textFilter.MatchOptions, textFilter.MatchFlags);
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter)));
		}

		internal static string OfferIdGetter(IPropertyBag propertyBag)
		{
			string result = null;
			if (propertyBag[ExchangeConfigurationUnitSchema.ResellerId] != null)
			{
				string text = (string)propertyBag[ExchangeConfigurationUnitSchema.ResellerId];
				int num = text.IndexOf(".");
				if (num > 1 && text.Length > num + 1)
				{
					result = text.Substring(num + 1);
				}
			}
			return result;
		}

		internal static void ProgramIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ExchangeConfigurationUnitSchema.ResellerId] = string.Format("{0}.{1}", (string)value, (string)propertyBag[ExchangeConfigurationUnitSchema.OfferId]);
		}

		internal static void OfferIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ExchangeConfigurationUnitSchema.ResellerId] = string.Format("{0}.{1}", (string)propertyBag[ExchangeConfigurationUnitSchema.ProgramId], (string)value);
		}

		internal static QueryFilter ExternalDirectoryOrganizationIdFilterBuilder(SinglePropertyFilter filter)
		{
			if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				object propertyValue = comparisonFilter.PropertyValue;
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw, (propertyValue != null) ? propertyValue.ToString() : null);
			}
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw);
			}
			if (filter is TextFilter)
			{
				TextFilter textFilter = filter as TextFilter;
				string text = textFilter.Text;
				return new TextFilter(ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw, (text != null) ? text.ToLower() : null, textFilter.MatchOptions, textFilter.MatchFlags);
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId.Name, filter.GetType(), typeof(ComparisonFilter)));
		}

		internal static string ExternalDirectoryOrganizationIdGetter(IPropertyBag propertyBag)
		{
			if ((bool)propertyBag[OrganizationSchema.IsTemplateTenant])
			{
				return TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationId;
			}
			return (string)propertyBag[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw];
		}

		internal static void ExternalDirectoryOrganizationIdSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationIdRaw] = value;
		}

		internal static bool IsOrganizationDirSyncRunning(bool isDirSyncRunning, MultiValuedProperty<string> dirSyncStatus, List<DirSyncState> dirSyncStoppedStates)
		{
			ExTraceGlobals.ADObjectTracer.TraceDebug(0L, "<ExchangeConfigurationUnit::IsOrganizationDirSyncRunning> enter");
			bool flag = false;
			if (dirSyncStatus != null && dirSyncStatus.Count > 0)
			{
				foreach (string text in dirSyncStatus)
				{
					ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "Check dirSyncStatusString \"{0}\"", text);
					DirSyncStatusValue dirSyncStatusValue = SyncValueConvertor.ConvertStringToDirSyncStatus(text);
					if (dirSyncStatusValue != null)
					{
						if (!dirSyncStoppedStates.Contains(dirSyncStatusValue.State))
						{
							ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "Organization is either dirsync enabled or in pending state. (dirSyncStatusValue.State = {0})", dirSyncStatusValue.State.ToString());
							flag = true;
							break;
						}
						ExTraceGlobals.ADObjectTracer.TraceDebug(0L, "DirSyncStatus.State is DirSyncState.Disabled");
					}
					else
					{
						ExTraceGlobals.ADObjectTracer.TraceWarning<string>(0L, "Ignore invalid DirSyncStatus value \"{0}\"", text);
					}
				}
			}
			if (!flag)
			{
				ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "Set return value to be isDirSyncRunning value ({0})", isDirSyncRunning.ToString());
				flag = isDirSyncRunning;
			}
			ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "<ExchangeConfigurationUnit::IsOrganizationDirSyncRunning> return ({0})", flag.ToString());
			return flag;
		}

		public ADObjectId ManagementSiteLink
		{
			get
			{
				return (ADObjectId)this[ExchangeConfigurationUnitSchema.ManagementSiteLink];
			}
			internal set
			{
				this[ExchangeConfigurationUnitSchema.ManagementSiteLink] = value;
			}
		}

		internal void TestOnlySetWhenOrganizationStatusSet(DateTime value)
		{
			this.WhenOrganizationStatusSet = new DateTime?(value);
		}

		internal bool HasSharedConfigurationBL()
		{
			if (base.SharedConfigurationInfo == null)
			{
				return false;
			}
			ADRawEntry[] array = this.m_Session.Find(base.Id, QueryScope.Base, new ExistsFilter(OrganizationSchema.SupportedSharedConfigurationsBL), null, 1, new PropertyDefinition[]
			{
				ADObjectSchema.Id
			});
			return array != null && array.Length == 1;
		}

		internal bool TryGetIdByWellKnownGuid(Guid wkGuid, out ADObjectId id)
		{
			if (wkGuid.Equals(Guid.Empty))
			{
				throw new ArgumentException("wkGuid");
			}
			if (this.wellKnownGuidToDn == null)
			{
				this.InitWellKnownObjectStructures();
			}
			id = null;
			string distinguishedName = null;
			if (this.wellKnownGuidToDn.TryGetValue(wkGuid, out distinguishedName))
			{
				id = new ADObjectId(distinguishedName);
			}
			return id != null;
		}

		internal bool TryGetWellKnownGuidById(ADObjectId id, out Guid wkGuid)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (string.IsNullOrEmpty(id.DistinguishedName))
			{
				throw new ArgumentException("id");
			}
			if (this.dnToWellKnownGuid == null)
			{
				this.InitWellKnownObjectStructures();
			}
			wkGuid = Guid.Empty;
			return this.dnToWellKnownGuid.TryGetValue(id.DistinguishedName, out wkGuid);
		}

		private void InitWellKnownObjectStructures()
		{
			lock (this.wellKnownObjectsLock)
			{
				if (this.dnToWellKnownGuid == null)
				{
					this.dnToWellKnownGuid = new Dictionary<string, Guid>(this.OtherWellKnownObjects.Count);
					this.wellKnownGuidToDn = new Dictionary<Guid, string>(this.OtherWellKnownObjects.Count);
					foreach (DNWithBinary dnwithBinary in this.OtherWellKnownObjects)
					{
						byte[] binary = dnwithBinary.Binary;
						if (binary != null && binary.Length == 16)
						{
							Guid guid = new Guid(binary);
							string distinguishedName = dnwithBinary.DistinguishedName;
							this.dnToWellKnownGuid.Add(distinguishedName, guid);
							this.wellKnownGuidToDn.Add(guid, distinguishedName);
						}
					}
				}
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (this.OrganizationId == null || this.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				return false;
			}
			bool flag = false;
			if (base.ObjectState == ObjectState.Deleted)
			{
				flag = true;
				keys = new Guid[]
				{
					CannedProvisioningCacheKeys.OrganizationCUContainer,
					CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey,
					CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey
				};
			}
			else if (base.ObjectState == ObjectState.Changed)
			{
				if (base.IsChanged(OrganizationSchema.SMTPAddressCheckWithAcceptedDomain) || base.IsChanged(OrganizationSchema.SkipToUAndParentalControlCheck) || base.IsChanged(OrganizationSchema.IsHotmailMigration) || base.IsChanged(OrganizationSchema.AllowDeleteOfExternalIdentityUponRemove) || base.IsChanged(OrganizationSchema.UseServicePlanAsCounterInstanceName) || base.IsChanged(ExchangeConfigurationUnitSchema.ServicePlan) || base.IsChanged(OrganizationSchema.SoftDeletedFeatureStatus))
				{
					flag = true;
					if (this.IsHydratedFlagChanged)
					{
						keys = new Guid[]
						{
							CannedProvisioningCacheKeys.OrganizationCUContainer,
							CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey,
							CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey
						};
					}
					else
					{
						keys = new Guid[]
						{
							CannedProvisioningCacheKeys.OrganizationCUContainer
						};
					}
				}
				else if (this.IsHydratedFlagChanged)
				{
					flag = true;
					keys = new Guid[]
					{
						CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey,
						CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey
					};
				}
			}
			else if (base.ObjectState == ObjectState.New)
			{
				keys = new Guid[]
				{
					CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey,
					CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey
				};
			}
			if (flag)
			{
				orgId = this.OrganizationId;
			}
			return flag;
		}

		private bool IsHydratedFlagChanged
		{
			get
			{
				return base.IsChanged(OrganizationSchema.SupportedSharedConfigurations) || base.IsChanged(OrganizationSchema.SharedConfigurationInfo) || base.IsChanged(OrganizationSchema.IsDehydrated) || base.IsChanged(OrganizationSchema.IsStaticConfigurationShared) || base.IsChanged(OrganizationSchema.EnableAsSharedConfiguration) || base.IsChanged(OrganizationSchema.ImmutableConfiguration);
			}
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		public const MailboxRelease CurrentMailboxRelease = MailboxRelease.E15;

		internal new static readonly string MostDerivedClass = "msExchConfigurationUnitContainer";

		private static readonly ExchangeConfigurationUnitSchema schema = ObjectSchema.GetInstance<ExchangeConfigurationUnitSchema>();

		private Dictionary<string, Guid> dnToWellKnownGuid;

		private Dictionary<Guid, string> wellKnownGuidToDn;

		private object wellKnownObjectsLock = new object();
	}
}
