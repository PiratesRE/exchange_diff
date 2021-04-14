using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOrganizationCommand : SyntheticCommandWithPipelineInputNoOutput<ExchangeConfigurationUnit>
	{
		private SetOrganizationCommand() : base("Set-Organization")
		{
		}

		public SetOrganizationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.SharedConfigurationInfoParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.SharedConfigurationParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.AddRelocationConstraintParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.RemoveRelocationConstraintParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.SharedConfigurationRemoveParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationCommand SetParameters(SetOrganizationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class SharedConfigurationInfoParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual bool EnableAsSharedConfiguration
			{
				set
				{
					base.PowerSharpParameters["EnableAsSharedConfiguration"] = value;
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class SharedConfigurationParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<OrganizationIdParameter> SharedConfiguration
			{
				set
				{
					base.PowerSharpParameters["SharedConfiguration"] = value;
				}
			}

			public virtual SwitchParameter ClearPreviousSharedConfigurations
			{
				set
				{
					base.PowerSharpParameters["ClearPreviousSharedConfigurations"] = value;
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class AddRelocationConstraintParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter AddRelocationConstraint
			{
				set
				{
					base.PowerSharpParameters["AddRelocationConstraint"] = value;
				}
			}

			public virtual PersistableRelocationConstraintType RelocationConstraintType
			{
				set
				{
					base.PowerSharpParameters["RelocationConstraintType"] = value;
				}
			}

			public virtual int RelocationConstraintExpirationInDays
			{
				set
				{
					base.PowerSharpParameters["RelocationConstraintExpirationInDays"] = value;
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class RemoveRelocationConstraintParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter RemoveRelocationConstraint
			{
				set
				{
					base.PowerSharpParameters["RemoveRelocationConstraint"] = value;
				}
			}

			public virtual PersistableRelocationConstraintType RelocationConstraintType
			{
				set
				{
					base.PowerSharpParameters["RelocationConstraintType"] = value;
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class SharedConfigurationRemoveParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter RemoveSharedConfigurations
			{
				set
				{
					base.PowerSharpParameters["RemoveSharedConfigurations"] = value;
				}
			}

			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual bool ImmutableConfiguration
			{
				set
				{
					base.PowerSharpParameters["ImmutableConfiguration"] = value;
				}
			}

			public virtual bool IsDehydrated
			{
				set
				{
					base.PowerSharpParameters["IsDehydrated"] = value;
				}
			}

			public virtual bool IsStaticConfigurationShared
			{
				set
				{
					base.PowerSharpParameters["IsStaticConfigurationShared"] = value;
				}
			}

			public virtual bool IsUpdatingServicePlan
			{
				set
				{
					base.PowerSharpParameters["IsUpdatingServicePlan"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual MultiValuedProperty<Capability> PersistedCapabilities
			{
				set
				{
					base.PowerSharpParameters["PersistedCapabilities"] = value;
				}
			}

			public virtual string ExchangeUpgradeBucket
			{
				set
				{
					base.PowerSharpParameters["ExchangeUpgradeBucket"] = ((value != null) ? new ExchangeUpgradeBucketIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ExcludedFromBackSync
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromBackSync"] = value;
				}
			}

			public virtual SwitchParameter ExcludedFromForwardSyncEDU2BPOS
			{
				set
				{
					base.PowerSharpParameters["ExcludedFromForwardSyncEDU2BPOS"] = value;
				}
			}

			public virtual int DefaultMovePriority
			{
				set
				{
					base.PowerSharpParameters["DefaultMovePriority"] = value;
				}
			}

			public virtual string UpgradeMessage
			{
				set
				{
					base.PowerSharpParameters["UpgradeMessage"] = value;
				}
			}

			public virtual string UpgradeDetails
			{
				set
				{
					base.PowerSharpParameters["UpgradeDetails"] = value;
				}
			}

			public virtual UpgradeConstraintArray UpgradeConstraints
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraints"] = value;
				}
			}

			public virtual UpgradeStage? UpgradeStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeStage"] = value;
				}
			}

			public virtual DateTime? UpgradeStageTimeStamp
			{
				set
				{
					base.PowerSharpParameters["UpgradeStageTimeStamp"] = value;
				}
			}

			public virtual int? UpgradeE14MbxCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14MbxCountForCurrentStage"] = value;
				}
			}

			public virtual int? UpgradeE14RequestCountForCurrentStage
			{
				set
				{
					base.PowerSharpParameters["UpgradeE14RequestCountForCurrentStage"] = value;
				}
			}

			public virtual DateTime? UpgradeLastE14CountsUpdateTime
			{
				set
				{
					base.PowerSharpParameters["UpgradeLastE14CountsUpdateTime"] = value;
				}
			}

			public virtual bool? UpgradeConstraintsDisabled
			{
				set
				{
					base.PowerSharpParameters["UpgradeConstraintsDisabled"] = value;
				}
			}

			public virtual int? UpgradeUnitsOverride
			{
				set
				{
					base.PowerSharpParameters["UpgradeUnitsOverride"] = value;
				}
			}

			public virtual int MaxOfflineAddressBooks
			{
				set
				{
					base.PowerSharpParameters["MaxOfflineAddressBooks"] = value;
				}
			}

			public virtual int MaxAddressBookPolicies
			{
				set
				{
					base.PowerSharpParameters["MaxAddressBookPolicies"] = value;
				}
			}

			public virtual MailboxRelease MailboxRelease
			{
				set
				{
					base.PowerSharpParameters["MailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PreviousMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PreviousMailboxRelease"] = value;
				}
			}

			public virtual MailboxRelease PilotMailboxRelease
			{
				set
				{
					base.PowerSharpParameters["PilotMailboxRelease"] = value;
				}
			}

			public virtual bool IsLicensingEnforced
			{
				set
				{
					base.PowerSharpParameters["IsLicensingEnforced"] = value;
				}
			}

			public virtual string ProgramId
			{
				set
				{
					base.PowerSharpParameters["ProgramId"] = value;
				}
			}

			public virtual string OfferId
			{
				set
				{
					base.PowerSharpParameters["OfferId"] = value;
				}
			}

			public virtual string ServicePlan
			{
				set
				{
					base.PowerSharpParameters["ServicePlan"] = value;
				}
			}

			public virtual string TargetServicePlan
			{
				set
				{
					base.PowerSharpParameters["TargetServicePlan"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool IsHotmailMigration
			{
				set
				{
					base.PowerSharpParameters["IsHotmailMigration"] = value;
				}
			}

			public virtual bool SyncMBXAndDLToMServ
			{
				set
				{
					base.PowerSharpParameters["SyncMBXAndDLToMServ"] = value;
				}
			}

			public virtual OrganizationStatus OrganizationStatus
			{
				set
				{
					base.PowerSharpParameters["OrganizationStatus"] = value;
				}
			}

			public virtual string IOwnMigrationTenant
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationTenant"] = value;
				}
			}

			public virtual string IOwnMigrationStatusReport
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatusReport"] = value;
				}
			}

			public virtual IOwnMigrationStatusFlagsEnum IOwnMigrationStatus
			{
				set
				{
					base.PowerSharpParameters["IOwnMigrationStatus"] = value;
				}
			}

			public virtual bool MSOSyncEnabled
			{
				set
				{
					base.PowerSharpParameters["MSOSyncEnabled"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderIssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderIssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderMaxItemSize
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderMaxItemSize"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> DefaultPublicFolderProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["DefaultPublicFolderProhibitPostQuota"] = value;
				}
			}

			public virtual bool IsDirSyncRunning
			{
				set
				{
					base.PowerSharpParameters["IsDirSyncRunning"] = value;
				}
			}

			public virtual UpgradeStatusTypes UpgradeStatus
			{
				set
				{
					base.PowerSharpParameters["UpgradeStatus"] = value;
				}
			}

			public virtual UpgradeRequestTypes UpgradeRequest
			{
				set
				{
					base.PowerSharpParameters["UpgradeRequest"] = value;
				}
			}

			public virtual MultiValuedProperty<string> CompanyTags
			{
				set
				{
					base.PowerSharpParameters["CompanyTags"] = value;
				}
			}

			public virtual string Location
			{
				set
				{
					base.PowerSharpParameters["Location"] = value;
				}
			}

			public virtual MultiValuedProperty<string> DirSyncStatus
			{
				set
				{
					base.PowerSharpParameters["DirSyncStatus"] = value;
				}
			}

			public virtual MultiValuedProperty<string> AsynchronousOperationIds
			{
				set
				{
					base.PowerSharpParameters["AsynchronousOperationIds"] = value;
				}
			}

			public virtual string DirSyncServiceInstance
			{
				set
				{
					base.PowerSharpParameters["DirSyncServiceInstance"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
