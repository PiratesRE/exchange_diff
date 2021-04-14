using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class TenantOrganizationPresentationObject : ADPresentationObject
	{
		public static ADObjectId DefaultManagementSiteId
		{
			get
			{
				if (TenantOrganizationPresentationObject.defaultManagementSiteId == null)
				{
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(TenantOrganizationPresentationObject.ExchangeCrossForestKey))
					{
						TenantOrganizationPresentationObject.defaultManagementSiteId = new ADObjectId((string)registryKey.GetValue(TenantOrganizationPresentationObject.DefaultManagementSiteLink, null));
					}
				}
				return TenantOrganizationPresentationObject.defaultManagementSiteId;
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return TenantOrganizationPresentationObject.schema;
			}
		}

		public TenantOrganizationPresentationObject()
		{
		}

		public TenantOrganizationPresentationObject(ExchangeConfigurationUnit dataObject) : base(dataObject)
		{
			this.PopulateTaskPopulatedProperties();
		}

		private void PopulateTaskPopulatedProperties()
		{
			RelocationConstraintArray relocationConstraintArray = (RelocationConstraintArray)this[OrganizationSchema.PersistedRelocationConstraints];
			MultiValuedProperty<RelocationConstraint> relocationConstraints = new MultiValuedProperty<RelocationConstraint>();
			if (relocationConstraintArray != null && relocationConstraintArray.RelocationConstraints != null)
			{
				Array.ForEach<RelocationConstraint>(relocationConstraintArray.RelocationConstraints, delegate(RelocationConstraint x)
				{
					relocationConstraints.Add(x);
				});
			}
			if (this.AdminDisplayVersion != null)
			{
				ExchangeBuild exchangeBuild = this.AdminDisplayVersion.ExchangeBuild;
				if (this.AdminDisplayVersion.ExchangeBuild.ToString().StartsWith("14"))
				{
					relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.TenantVersionConstraint, DateTime.MaxValue));
				}
			}
			if (this.OrganizationStatus != OrganizationStatus.Active || this.IsUpgradingOrganization || this.IsPilotingOrganization || this.IsUpgradeOperationInProgress || this.IsFfoMigrationInProgress || this.IsUpdatingServicePlan)
			{
				relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.TenantInTransitionConstraint, DateTime.MaxValue));
			}
			if (!this.IsValid)
			{
				relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.ValidationErrorConstraint, DateTime.MaxValue));
			}
			if (this.EnableAsSharedConfiguration || this.ImmutableConfiguration)
			{
				relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.SCTConstraint, DateTime.MaxValue));
			}
			if (this.UpgradeE14MbxCountForCurrentStage != null && this.UpgradeE14MbxCountForCurrentStage.Value != 0)
			{
				relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.E14MailboxesPresentContraint, DateTime.MaxValue));
			}
			if (!string.IsNullOrEmpty((string)this[ExchangeConfigurationUnitSchema.TargetForest]) || !string.IsNullOrEmpty((string)this[ExchangeConfigurationUnitSchema.RelocationSourceForestRaw]))
			{
				relocationConstraints.Add(new RelocationConstraint(RelocationConstraintType.RelocationInProgressConstraint, DateTime.MaxValue));
			}
			relocationConstraints.Sort();
			this[TenantOrganizationPresentationObjectSchema.RelocationConstraints] = relocationConstraints;
			if (this.IsTemplateTenant)
			{
				this.IsSharingConfiguration = true;
			}
		}

		public OrganizationStatus OrganizationStatus
		{
			get
			{
				return (OrganizationStatus)((int)this[TenantOrganizationPresentationObjectSchema.OrganizationStatus]);
			}
			internal set
			{
				this[TenantOrganizationPresentationObjectSchema.OrganizationStatus] = (int)value;
			}
		}

		public DateTime? WhenOrganizationStatusSet
		{
			get
			{
				return (DateTime?)this[TenantOrganizationPresentationObjectSchema.WhenOrganizationStatusSet];
			}
		}

		public bool IsUpgradingOrganization
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsUpgradingOrganization];
			}
		}

		public bool IsPilotingOrganization
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsPilotingOrganization];
			}
		}

		public bool IsUpgradeOperationInProgress
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsUpgradeOperationInProgress];
			}
		}

		public new OrganizationId OrganizationId
		{
			get
			{
				return (OrganizationId)this[TenantOrganizationPresentationObjectSchema.OrganizationId];
			}
		}

		public string ServicePlan
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.ServicePlan];
			}
		}

		public bool IsUpdatingServicePlan
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsUpdatingServicePlan];
			}
		}

		public string TargetServicePlan
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.TargetServicePlan];
			}
		}

		public bool IsFfoMigrationInProgress
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsFfoMigrationInProgress];
			}
		}

		public string ProgramId
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.ProgramId];
			}
		}

		public string OfferId
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.OfferId];
			}
		}

		public bool ExcludedFromBackSync
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.ExcludedFromBackSync];
			}
		}

		public bool ExcludedFromForwardSyncEDU2BPOS
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.ExcludedFromForwardSyncEDU2BPOS];
			}
		}

		public bool AllowDeleteOfExternalIdentityUponRemove
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.AllowDeleteOfExternalIdentityUponRemove];
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.ExternalDirectoryOrganizationId];
			}
		}

		public new string Name
		{
			get
			{
				return base.DataObject.OrganizationId.OrganizationalUnit.Name;
			}
			internal set
			{
				this[ADObjectSchema.Name] = value;
			}
		}

		public new string DistinguishedName
		{
			get
			{
				return base.DataObject.OrganizationId.OrganizationalUnit.DistinguishedName;
			}
			internal set
			{
				this[ADObjectSchema.DistinguishedName] = value;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				ObjectId objectId = base.DataObject.OrganizationId.OrganizationalUnit;
				object obj;
				if (base.TryConvertOutputProperty(objectId, "Identity", out obj))
				{
					objectId = (ObjectId)obj;
				}
				return objectId;
			}
		}

		public new Guid Guid
		{
			get
			{
				return base.DataObject.OrganizationId.OrganizationalUnit.ObjectGuid;
			}
		}

		public int ObjectVersion
		{
			get
			{
				return (int)this[OrganizationSchema.ObjectVersion];
			}
		}

		public int BuildMajor
		{
			get
			{
				return (int)this[OrganizationSchema.BuildMajor];
			}
		}

		public int BuildMinor
		{
			get
			{
				return (int)this[OrganizationSchema.BuildMinor];
			}
		}

		public bool IsFederated
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsFederated];
			}
		}

		public bool SkipToUAndParentalControlCheck
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.SkipToUAndParentalControlCheck];
			}
		}

		public bool HideAdminAccessWarning
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.HideAdminAccessWarning];
			}
		}

		public bool ShowAdminAccessWarning
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.ShowAdminAccessWarning];
			}
		}

		public bool SMTPAddressCheckWithAcceptedDomain
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.SMTPAddressCheckWithAcceptedDomain];
			}
		}

		public ADObjectId ManagementSiteLink
		{
			get
			{
				ADObjectId adobjectId = (ADObjectId)this[TenantOrganizationPresentationObjectSchema.ManagementSiteLink];
				if (adobjectId != null)
				{
					return adobjectId;
				}
				return TenantOrganizationPresentationObject.DefaultManagementSiteId;
			}
		}

		public bool EnableAsSharedConfiguration
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.EnableAsSharedConfiguration];
			}
		}

		public bool ImmutableConfiguration
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.ImmutableConfiguration];
			}
		}

		public MultiValuedProperty<ADObjectId> SupportedSharedConfigurations
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[TenantOrganizationPresentationObjectSchema.SupportedSharedConfigurations];
			}
		}

		public SharedConfigurationInfo SharedConfigurationInfo
		{
			get
			{
				return (SharedConfigurationInfo)this[TenantOrganizationPresentationObjectSchema.SharedConfigurationInfo];
			}
		}

		public bool IsTemplateTenant
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsTemplateTenant];
			}
		}

		public bool IsSharingConfiguration
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsSharingConfiguration];
			}
			internal set
			{
				this[TenantOrganizationPresentationObjectSchema.IsSharingConfiguration] = value;
			}
		}

		public bool IsDehydrated
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsDehydrated];
			}
		}

		public bool IsStaticConfigurationShared
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsStaticConfigurationShared];
			}
		}

		public bool IsLicensingEnforced
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsLicensingEnforced];
			}
		}

		public bool IsTenantAccessBlocked
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsTenantAccessBlocked];
			}
		}

		public bool MSOSyncEnabled
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.MSOSyncEnabled];
			}
		}

		public int? MaxAddressBookPolicies
		{
			get
			{
				return (int?)this[TenantOrganizationPresentationObjectSchema.MaxAddressBookPolicies];
			}
		}

		public int? MaxOfflineAddressBooks
		{
			get
			{
				return (int?)this[TenantOrganizationPresentationObjectSchema.MaxOfflineAddressBooks];
			}
		}

		public bool UseServicePlanAsCounterInstanceName
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.UseServicePlanAsCounterInstanceName];
			}
		}

		public Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TenantOrganizationPresentationObjectSchema.DefaultPublicFolderIssueWarningQuota];
			}
		}

		public Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TenantOrganizationPresentationObjectSchema.DefaultPublicFolderProhibitPostQuota];
			}
		}

		public MultiValuedProperty<string> AsynchronousOperationIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[TenantOrganizationPresentationObjectSchema.AsynchronousOperationIds];
			}
		}

		public bool IsDirSyncRunning
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsDirSyncRunning];
			}
		}

		public bool IsDirSyncStatusPending
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.IsDirSyncStatusPending];
			}
		}

		public MultiValuedProperty<string> DirSyncStatus
		{
			get
			{
				return (MultiValuedProperty<string>)this[TenantOrganizationPresentationObjectSchema.DirSyncStatus];
			}
		}

		public ADObjectId ExchangeUpgradeBucket
		{
			get
			{
				return (ADObjectId)this[TenantOrganizationPresentationObjectSchema.ExchangeUpgradeBucket];
			}
		}

		public ExchangeObjectVersion AdminDisplayVersion
		{
			get
			{
				return (ExchangeObjectVersion)this[TenantOrganizationPresentationObjectSchema.AdminDisplayVersion];
			}
		}

		public SoftDeletedFeatureStatusFlags SoftDeletedFeatureStatus
		{
			get
			{
				return (SoftDeletedFeatureStatusFlags)this[TenantOrganizationPresentationObjectSchema.SoftDeletedFeatureStatus];
			}
		}

		public string DirSyncServiceInstance
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.DirSyncServiceInstance];
			}
		}

		public string Location
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.Location];
			}
		}

		public MultiValuedProperty<string> CompanyTags
		{
			get
			{
				return (MultiValuedProperty<string>)this[TenantOrganizationPresentationObjectSchema.CompanyTags];
			}
		}

		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)this[TenantOrganizationPresentationObjectSchema.PersistedCapabilities];
			}
		}

		public UpgradeStatusTypes UpgradeStatus
		{
			get
			{
				return (UpgradeStatusTypes)this[TenantOrganizationPresentationObjectSchema.UpgradeStatus];
			}
		}

		public UpgradeRequestTypes UpgradeRequest
		{
			get
			{
				return (UpgradeRequestTypes)this[TenantOrganizationPresentationObjectSchema.UpgradeRequest];
			}
		}

		public UpgradeConstraintArray UpgradeConstraints
		{
			get
			{
				return (UpgradeConstraintArray)this[TenantOrganizationPresentationObjectSchema.UpgradeConstraints];
			}
		}

		public string UpgradeMessage
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.UpgradeMessage];
			}
		}

		public string UpgradeDetails
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.UpgradeDetails];
			}
		}

		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)this[TenantOrganizationPresentationObjectSchema.UpgradeStage];
			}
		}

		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)this[TenantOrganizationPresentationObjectSchema.UpgradeStageTimeStamp];
			}
		}

		public int? UpgradeE14RequestCountForCurrentStage
		{
			get
			{
				return (int?)this[TenantOrganizationPresentationObjectSchema.UpgradeE14RequestCountForCurrentStage];
			}
		}

		public int? UpgradeE14MbxCountForCurrentStage
		{
			get
			{
				return (int?)this[TenantOrganizationPresentationObjectSchema.UpgradeE14MbxCountForCurrentStage];
			}
		}

		public bool? UpgradeConstraintsDisabled
		{
			get
			{
				return (bool?)this[TenantOrganizationPresentationObjectSchema.UpgradeConstraintsDisabled];
			}
		}

		public int? UpgradeUnitsOverride
		{
			get
			{
				return (int?)this[TenantOrganizationPresentationObjectSchema.UpgradeUnitsOverride];
			}
		}

		public DateTime? UpgradeLastE14CountsUpdateTime
		{
			get
			{
				return (DateTime?)this[TenantOrganizationPresentationObjectSchema.UpgradeLastE14CountsUpdateTime];
			}
		}

		public int DefaultMovePriority
		{
			get
			{
				return (int)this[TenantOrganizationPresentationObjectSchema.DefaultMovePriority];
			}
		}

		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[TenantOrganizationPresentationObjectSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
		}

		public MailboxRelease PreviousMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[TenantOrganizationPresentationObjectSchema.PreviousMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
		}

		public MailboxRelease PilotMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)this[TenantOrganizationPresentationObjectSchema.PilotMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
		}

		public ReleaseTrack? ReleaseTrack
		{
			get
			{
				return (ReleaseTrack?)this[TenantOrganizationPresentationObjectSchema.ReleaseTrack];
			}
		}

		public MultiValuedProperty<RelocationConstraint> RelocationConstraints
		{
			get
			{
				return (MultiValuedProperty<RelocationConstraint>)this[TenantOrganizationPresentationObjectSchema.RelocationConstraints];
			}
		}

		public bool OriginatedInDifferentForest
		{
			get
			{
				return (bool)this[TenantOrganizationPresentationObjectSchema.OriginatedInDifferentForest];
			}
		}

		public string AdminDisplayName
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.AdminDisplayName];
			}
		}

		public string IOwnMigrationTenant
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.IOwnMigrationTenant];
			}
		}

		public IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
		{
			get
			{
				return (IOwnMigrationStatusFlagsEnum)this[TenantOrganizationPresentationObjectSchema.IOwnMigrationStatus];
			}
		}

		public string IOwnMigrationStatusReport
		{
			get
			{
				return (string)this[TenantOrganizationPresentationObjectSchema.IOwnMigrationStatusReport];
			}
		}

		private static readonly string ExchangeCrossForestKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExchangeCrossForest\\";

		private static readonly string DefaultManagementSiteLink = "DefaultManagementSiteLink";

		private static ADObjectId defaultManagementSiteId;

		private static TenantOrganizationPresentationObjectSchema schema = ObjectSchema.GetInstance<TenantOrganizationPresentationObjectSchema>();
	}
}
