using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "Organization", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetOrganization : SetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		private List<ExchangeConfigurationUnit> SharedConfigurationUnits
		{
			get
			{
				if (this.sharedConfigurationUnits == null)
				{
					this.sharedConfigurationUnits = new List<ExchangeConfigurationUnit>();
					foreach (OrganizationIdParameter organization in this.SharedConfiguration)
					{
						ExchangeConfigurationUnit exchangeConfigUnitFromOrganizationId = OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId(organization, DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(base.CurrentOrganizationId.PartitionId), 57, "SharedConfigurationUnits", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\SetOrganizationTask.cs"), new Task.TaskErrorLoggingDelegate(base.WriteError), true);
						this.sharedConfigurationUnits.Add(exchangeConfigUnitFromOrganizationId);
					}
				}
				return this.sharedConfigurationUnits;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOrganization(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SharedConfigurationInfo", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "SharedConfiguration", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "AddRelocationConstraint", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveRelocationConstraint", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "SharedConfigurationRemove", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SharedConfiguration")]
		public MultiValuedProperty<OrganizationIdParameter> SharedConfiguration
		{
			get
			{
				return (MultiValuedProperty<OrganizationIdParameter>)base.Fields[OrganizationSchema.SupportedSharedConfigurations];
			}
			set
			{
				base.Fields[OrganizationSchema.SupportedSharedConfigurations] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "SharedConfiguration")]
		public SwitchParameter ClearPreviousSharedConfigurations { get; set; }

		[Parameter(Mandatory = false, ParameterSetName = "SharedConfigurationRemove")]
		public SwitchParameter RemoveSharedConfigurations { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "SharedConfigurationInfo")]
		public bool EnableAsSharedConfiguration
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.EnableAsSharedConfiguration] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.EnableAsSharedConfiguration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ImmutableConfiguration
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.ImmutableConfiguration] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.ImmutableConfiguration] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDehydrated
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.IsDehydrated] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsDehydrated] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsStaticConfigurationShared
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.IsStaticConfigurationShared] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsStaticConfigurationShared] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsUpdatingServicePlan
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.IsUpdatingServicePlan] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsUpdatingServicePlan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return (MultiValuedProperty<Capability>)base.Fields["TenantSKUCapability"];
			}
			set
			{
				if (value != null)
				{
					base.VerifyValues<Capability>(CapabilityHelper.AllowedSKUCapabilities, value.ToArray());
				}
				base.Fields["TenantSKUCapability"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExchangeUpgradeBucketIdParameter ExchangeUpgradeBucket
		{
			get
			{
				return (ExchangeUpgradeBucketIdParameter)base.Fields[ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket];
			}
			set
			{
				base.Fields[ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ExcludedFromBackSync
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.ExcludedFromBackSync] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.ExcludedFromBackSync] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ExcludedFromForwardSyncEDU2BPOS
		{
			get
			{
				return (SwitchParameter)(base.Fields[OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int DefaultMovePriority
		{
			get
			{
				return (int)base.Fields[OrganizationSchema.DefaultMovePriority];
			}
			set
			{
				base.Fields[OrganizationSchema.DefaultMovePriority] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UpgradeMessage
		{
			get
			{
				return (string)base.Fields[OrganizationSchema.UpgradeMessage];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UpgradeDetails
		{
			get
			{
				return (string)base.Fields[OrganizationSchema.UpgradeDetails];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeDetails] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeConstraintArray UpgradeConstraints
		{
			get
			{
				return (UpgradeConstraintArray)base.Fields[OrganizationSchema.UpgradeConstraints];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeConstraints] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UpgradeStage? UpgradeStage
		{
			get
			{
				return (UpgradeStage?)base.Fields[OrganizationSchema.UpgradeStage];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeStage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? UpgradeStageTimeStamp
		{
			get
			{
				return (DateTime?)base.Fields[OrganizationSchema.UpgradeStageTimeStamp];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeStageTimeStamp] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? UpgradeE14MbxCountForCurrentStage
		{
			get
			{
				return (int?)base.Fields[OrganizationSchema.UpgradeE14MbxCountForCurrentStage];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeE14MbxCountForCurrentStage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? UpgradeE14RequestCountForCurrentStage
		{
			get
			{
				return (int?)base.Fields[OrganizationSchema.UpgradeE14RequestCountForCurrentStage];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeE14RequestCountForCurrentStage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime? UpgradeLastE14CountsUpdateTime
		{
			get
			{
				return (DateTime?)base.Fields[OrganizationSchema.UpgradeLastE14CountsUpdateTime];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeLastE14CountsUpdateTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? UpgradeConstraintsDisabled
		{
			get
			{
				return (bool?)base.Fields[OrganizationSchema.UpgradeConstraintsDisabled];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeConstraintsDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? UpgradeUnitsOverride
		{
			get
			{
				return (int?)base.Fields[OrganizationSchema.UpgradeUnitsOverride];
			}
			set
			{
				base.Fields[OrganizationSchema.UpgradeUnitsOverride] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxOfflineAddressBooks
		{
			get
			{
				return (int)base.Fields[OrganizationSchema.MaxOfflineAddressBooks];
			}
			set
			{
				base.Fields[OrganizationSchema.MaxOfflineAddressBooks] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxAddressBookPolicies
		{
			get
			{
				return (int)base.Fields[OrganizationSchema.MaxAddressBookPolicies];
			}
			set
			{
				base.Fields[OrganizationSchema.MaxAddressBookPolicies] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxRelease MailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)base.Fields[OrganizationSchema.MailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				base.Fields[OrganizationSchema.MailboxRelease] = value.ToString();
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxRelease PreviousMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)base.Fields[OrganizationSchema.PreviousMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				base.Fields[OrganizationSchema.PreviousMailboxRelease] = value.ToString();
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxRelease PilotMailboxRelease
		{
			get
			{
				MailboxRelease result;
				if (!Enum.TryParse<MailboxRelease>((string)base.Fields[OrganizationSchema.PilotMailboxRelease], true, out result))
				{
					return MailboxRelease.None;
				}
				return result;
			}
			set
			{
				base.Fields[OrganizationSchema.PilotMailboxRelease] = value.ToString();
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddRelocationConstraint")]
		public SwitchParameter AddRelocationConstraint
		{
			get
			{
				return (SwitchParameter)(base.Fields["AddRelocationConstraint"] ?? false);
			}
			set
			{
				base.Fields["AddRelocationConstraint"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveRelocationConstraint")]
		public SwitchParameter RemoveRelocationConstraint
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveRelocationConstraint"] ?? false);
			}
			set
			{
				base.Fields["RemoveRelocationConstraint"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddRelocationConstraint")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveRelocationConstraint")]
		[ValidateNotNullOrEmpty]
		public PersistableRelocationConstraintType RelocationConstraintType
		{
			get
			{
				return (PersistableRelocationConstraintType)base.Fields["RelocationConstraintType"];
			}
			set
			{
				base.Fields["RelocationConstraintType"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "AddRelocationConstraint")]
		public int RelocationConstraintExpirationInDays
		{
			get
			{
				return (int)base.Fields["RelocationConstraintExpirationInDays"];
			}
			set
			{
				base.Fields["RelocationConstraintExpirationInDays"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsLicensingEnforced
		{
			get
			{
				return (bool)(base.Fields[OrganizationSchema.IsLicensingEnforced] ?? false);
			}
			set
			{
				base.Fields[OrganizationSchema.IsLicensingEnforced] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ProgramId
		{
			get
			{
				return (string)base.Fields[ExchangeConfigurationUnitSchema.ProgramId];
			}
			set
			{
				base.Fields[ExchangeConfigurationUnitSchema.ProgramId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OfferId
		{
			get
			{
				return (string)base.Fields[ExchangeConfigurationUnitSchema.OfferId];
			}
			set
			{
				base.Fields[ExchangeConfigurationUnitSchema.OfferId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ServicePlan
		{
			get
			{
				return (string)base.Fields[ExchangeConfigurationUnitSchema.ServicePlan];
			}
			set
			{
				base.Fields[ExchangeConfigurationUnitSchema.ServicePlan] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TargetServicePlan
		{
			get
			{
				return (string)base.Fields[ExchangeConfigurationUnitSchema.TargetServicePlan];
			}
			set
			{
				base.Fields[ExchangeConfigurationUnitSchema.TargetServicePlan] = value;
			}
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			if (this.Identity == null)
			{
				OrganizationIdParameter identity = new OrganizationIdParameter(base.CurrentOrgContainerId);
				this.Identity = identity;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsModified(ExchangeConfigurationUnitSchema.DirSyncServiceInstance) && !string.IsNullOrEmpty(this.DataObject.DirSyncServiceInstance) && !ServiceInstanceId.IsValidServiceInstanceId(this.DataObject.DirSyncServiceInstance))
			{
				base.WriteError(new InvalidServiceInstanceIdException(this.DataObject.DirSyncServiceInstance), ExchangeErrorCategory.Client, null);
			}
			if (this.DataObject.IsChanged(OrganizationSchema.SupportedSharedConfigurations) && !this.RemoveSharedConfigurations)
			{
				foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in this.SharedConfigurationUnits)
				{
					if (exchangeConfigurationUnit.SharedConfigurationInfo == null)
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.SharedConfigurationInfoNotPresent(exchangeConfigurationUnit.Identity.ToString())), ExchangeErrorCategory.Client, null);
					}
					ServicePlanConfiguration instance = ServicePlanConfiguration.GetInstance();
					if (!exchangeConfigurationUnit.ProgramId.Equals(this.DataObject.ProgramId, StringComparison.OrdinalIgnoreCase) || (!this.IsHydratedOfferIdMatched(this.DataObject.ProgramId, this.DataObject.OfferId, exchangeConfigurationUnit, instance) && !this.IsPilotOfferIdMatched(this.DataObject.ProgramId, this.DataObject.OfferId, exchangeConfigurationUnit, instance) && !this.IsHydratedPilotOfferIdMatched(this.DataObject.ProgramId, this.DataObject.OfferId, exchangeConfigurationUnit, instance)))
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.OfferIdMatchError(this.Identity.ToString(), this.DataObject.ProgramId, this.DataObject.OfferId, exchangeConfigurationUnit.Identity.ToString(), exchangeConfigurationUnit.ProgramId, exchangeConfigurationUnit.OfferId)), ExchangeErrorCategory.Client, null);
					}
					if (!exchangeConfigurationUnit.EnableAsSharedConfiguration)
					{
						base.WriteError(new SharedConfigurationValidationException(Strings.SharedConfigurationNotEnabled(this.Identity.ToString(), exchangeConfigurationUnit.Identity.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
				if (this.DataObject.SharedConfigurationInfo != null)
				{
					base.WriteError(new SharedConfigurationValidationException(Strings.SharedConfigurationInfoExists(this.Identity.ToString(), this.DataObject.SharedConfigurationInfo.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			if (this.DataObject.IsChanged(OrganizationSchema.EnableAsSharedConfiguration) && !this.DataObject.EnableAsSharedConfiguration)
			{
				if (this.DataObject.SharedConfigurationInfo == null)
				{
					base.WriteError(new SharedConfigurationValidationException(Strings.SharedConfigurationInfoNotPresent(this.DataObject.Identity.ToString())), ExchangeErrorCategory.Client, null);
				}
				ExchangeConfigurationUnit[] array = OrganizationTaskHelper.FindSharedConfigurations(this.DataObject.SharedConfigurationInfo, this.DataObject.OrganizationId.PartitionId);
				if (array == null || array.Length < 2)
				{
					this.confirmLastSharedConfiguration = true;
				}
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADObject adobject = (ADObject)base.ResolveDataObject();
			base.CurrentOrganizationId = adobject.OrganizationId;
			return adobject;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.PrepareDataObject();
			if (base.Fields.IsModified(OrganizationSchema.DefaultMovePriority))
			{
				exchangeConfigurationUnit.DefaultMovePriority = this.DefaultMovePriority;
			}
			if (base.Fields.IsModified("TenantSKUCapability"))
			{
				CapabilityHelper.SetTenantSKUCapabilities(this.PersistedCapabilities, exchangeConfigurationUnit.PersistedCapabilities);
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeMessage))
			{
				exchangeConfigurationUnit.UpgradeMessage = this.UpgradeMessage;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeDetails))
			{
				exchangeConfigurationUnit.UpgradeDetails = this.UpgradeDetails;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeConstraints))
			{
				exchangeConfigurationUnit.UpgradeConstraints = this.UpgradeConstraints;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeStage))
			{
				exchangeConfigurationUnit.UpgradeStage = this.UpgradeStage;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeStageTimeStamp))
			{
				exchangeConfigurationUnit.UpgradeStageTimeStamp = this.UpgradeStageTimeStamp;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeE14RequestCountForCurrentStage))
			{
				exchangeConfigurationUnit.UpgradeE14RequestCountForCurrentStage = this.UpgradeE14RequestCountForCurrentStage;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeE14MbxCountForCurrentStage))
			{
				exchangeConfigurationUnit.UpgradeE14MbxCountForCurrentStage = this.UpgradeE14MbxCountForCurrentStage;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeLastE14CountsUpdateTime))
			{
				exchangeConfigurationUnit.UpgradeLastE14CountsUpdateTime = this.UpgradeLastE14CountsUpdateTime;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeConstraintsDisabled))
			{
				exchangeConfigurationUnit.UpgradeConstraintsDisabled = this.UpgradeConstraintsDisabled;
			}
			if (base.Fields.IsModified(OrganizationSchema.UpgradeUnitsOverride))
			{
				exchangeConfigurationUnit.UpgradeUnitsOverride = this.UpgradeUnitsOverride;
			}
			if (base.Fields.IsModified(OrganizationSchema.MaxAddressBookPolicies))
			{
				exchangeConfigurationUnit.MaxAddressBookPolicies = new int?(this.MaxAddressBookPolicies);
			}
			if (base.Fields.IsModified(OrganizationSchema.MaxOfflineAddressBooks))
			{
				exchangeConfigurationUnit.MaxOfflineAddressBooks = new int?(this.MaxOfflineAddressBooks);
			}
			if (this.RemoveSharedConfigurations)
			{
				exchangeConfigurationUnit.SupportedSharedConfigurations.Clear();
			}
			if (base.Fields.IsModified(OrganizationSchema.SupportedSharedConfigurations))
			{
				if (this.ClearPreviousSharedConfigurations)
				{
					exchangeConfigurationUnit.SupportedSharedConfigurations.Clear();
				}
				foreach (ExchangeConfigurationUnit exchangeConfigurationUnit2 in this.SharedConfigurationUnits)
				{
					if (!exchangeConfigurationUnit.SupportedSharedConfigurations.Contains(exchangeConfigurationUnit2.OrganizationId.ConfigurationUnit))
					{
						exchangeConfigurationUnit.SupportedSharedConfigurations.Add(exchangeConfigurationUnit2.Identity);
					}
				}
			}
			if (base.Fields.IsModified(OrganizationSchema.EnableAsSharedConfiguration))
			{
				exchangeConfigurationUnit.EnableAsSharedConfiguration = this.EnableAsSharedConfiguration;
			}
			if (base.Fields.IsModified(OrganizationSchema.ImmutableConfiguration))
			{
				exchangeConfigurationUnit.ImmutableConfiguration = this.ImmutableConfiguration;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsDehydrated))
			{
				exchangeConfigurationUnit.IsDehydrated = this.IsDehydrated;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsStaticConfigurationShared))
			{
				exchangeConfigurationUnit.IsStaticConfigurationShared = this.IsStaticConfigurationShared;
			}
			if (base.Fields.IsModified(OrganizationSchema.IsUpdatingServicePlan))
			{
				exchangeConfigurationUnit.IsUpdatingServicePlan = this.IsUpdatingServicePlan;
			}
			if (base.Fields.IsModified(ExchangeConfigurationUnitSchema.ProgramId))
			{
				exchangeConfigurationUnit.ProgramId = this.ProgramId;
			}
			if (base.Fields.IsModified(ExchangeConfigurationUnitSchema.OfferId))
			{
				exchangeConfigurationUnit.OfferId = this.OfferId;
			}
			if (base.Fields.IsModified(ExchangeConfigurationUnitSchema.ServicePlan))
			{
				exchangeConfigurationUnit.ServicePlan = this.ServicePlan;
			}
			if (base.Fields.IsModified(ExchangeConfigurationUnitSchema.TargetServicePlan))
			{
				exchangeConfigurationUnit.TargetServicePlan = this.TargetServicePlan;
			}
			if (!exchangeConfigurationUnit.HostingDeploymentEnabled && Datacenter.IsPartnerHostedOnly(false))
			{
				exchangeConfigurationUnit.HostingDeploymentEnabled = true;
			}
			if (base.Fields.IsModified(OrganizationSchema.ExcludedFromBackSync))
			{
				exchangeConfigurationUnit.ExcludedFromBackSync = this.ExcludedFromBackSync;
			}
			if (base.Fields.IsModified(OrganizationSchema.ExcludedFromForwardSyncEDU2BPOS))
			{
				exchangeConfigurationUnit.ExcludedFromForwardSyncEDU2BPOS = this.ExcludedFromForwardSyncEDU2BPOS;
			}
			if (base.Fields.IsModified(ExchangeConfigurationUnitSchema.ExchangeUpgradeBucket))
			{
				if (this.ExchangeUpgradeBucket != null)
				{
					ExchangeUpgradeBucket exchangeUpgradeBucket = (ExchangeUpgradeBucket)base.GetDataObject<ExchangeUpgradeBucket>(this.ExchangeUpgradeBucket, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorExchangeUpgradeBucketNotFound(this.ExchangeUpgradeBucket.ToString())), new LocalizedString?(Strings.ErrorExchangeUpgradeBucketNotUnique(this.ExchangeUpgradeBucket.ToString())));
					UpgradeBucketTaskHelper.ValidateOrganizationVersion(exchangeConfigurationUnit, exchangeUpgradeBucket, new Task.ErrorLoggerDelegate(base.WriteError));
					UpgradeBucketTaskHelper.ValidateOrganizationAddition(base.GlobalConfigSession, exchangeConfigurationUnit.OrganizationId, exchangeUpgradeBucket, new Task.ErrorLoggerDelegate(base.WriteError));
					exchangeConfigurationUnit.ExchangeUpgradeBucket = (ADObjectId)exchangeUpgradeBucket.Identity;
				}
				else
				{
					exchangeConfigurationUnit.ExchangeUpgradeBucket = null;
				}
			}
			if (base.Fields.IsModified(OrganizationSchema.MailboxRelease))
			{
				exchangeConfigurationUnit.MailboxRelease = this.MailboxRelease;
			}
			if (base.Fields.IsModified(OrganizationSchema.PreviousMailboxRelease))
			{
				exchangeConfigurationUnit.PreviousMailboxRelease = this.PreviousMailboxRelease;
			}
			if (base.Fields.IsModified(OrganizationSchema.PilotMailboxRelease))
			{
				exchangeConfigurationUnit.PilotMailboxRelease = this.PilotMailboxRelease;
			}
			if (base.Fields.IsModified("AddRelocationConstraint"))
			{
				RelocationConstraint constraintToAdd = new RelocationConstraint((RelocationConstraintType)this.RelocationConstraintType, DateTime.UtcNow.AddDays((double)this.RelocationConstraintExpirationInDays));
				RelocationConstraintArray persistedRelocationConstraints = SetOrganization.PopulateRelocationConstraintsList(exchangeConfigurationUnit.PersistedRelocationConstraints, this.RelocationConstraintType, constraintToAdd);
				exchangeConfigurationUnit.PersistedRelocationConstraints = persistedRelocationConstraints;
			}
			else if (base.Fields.IsModified("RemoveRelocationConstraint"))
			{
				RelocationConstraintArray persistedRelocationConstraints2 = SetOrganization.PopulateRelocationConstraintsList(exchangeConfigurationUnit.PersistedRelocationConstraints, this.RelocationConstraintType, null);
				exchangeConfigurationUnit.PersistedRelocationConstraints = persistedRelocationConstraints2;
			}
			OrganizationStatus organizationStatus;
			if (exchangeConfigurationUnit.IsModified(ExchangeConfigurationUnitSchema.OrganizationStatus) && exchangeConfigurationUnit.TryGetOriginalValue<OrganizationStatus>(ExchangeConfigurationUnitSchema.OrganizationStatus, out organizationStatus))
			{
				if (OrganizationStatus.Active == exchangeConfigurationUnit.OrganizationStatus && (OrganizationStatus.Suspended == organizationStatus || OrganizationStatus.LockedOut == organizationStatus))
				{
					exchangeConfigurationUnit.IsTenantAccessBlocked = false;
				}
				else if ((OrganizationStatus.Suspended == exchangeConfigurationUnit.OrganizationStatus || OrganizationStatus.LockedOut == exchangeConfigurationUnit.OrganizationStatus) && OrganizationStatus.Active == organizationStatus)
				{
					exchangeConfigurationUnit.IsTenantAccessBlocked = true;
				}
			}
			if (base.Fields.IsModified(OrganizationSchema.IsLicensingEnforced))
			{
				exchangeConfigurationUnit.IsLicensingEnforced = this.IsLicensingEnforced;
			}
			TaskLogger.LogExit();
			return exchangeConfigurationUnit;
		}

		private static RelocationConstraintArray PopulateRelocationConstraintsList(RelocationConstraintArray oldRelocationConstraintsArray, PersistableRelocationConstraintType constraintTypeToSkip, RelocationConstraint constraintToAdd)
		{
			string b = constraintTypeToSkip.ToString();
			List<RelocationConstraint> list = new List<RelocationConstraint>();
			if (oldRelocationConstraintsArray != null && oldRelocationConstraintsArray.RelocationConstraints != null)
			{
				foreach (RelocationConstraint relocationConstraint in oldRelocationConstraintsArray.RelocationConstraints)
				{
					if (relocationConstraint.Name != b)
					{
						list.Add(relocationConstraint);
					}
				}
			}
			if (constraintToAdd != null)
			{
				list.Add(constraintToAdd);
			}
			list.Sort();
			return new RelocationConstraintArray(list.ToArray());
		}

		private bool IsHydratedOfferIdMatched(string programId, string offerId, ExchangeConfigurationUnit sharedConfigurationUnit, ServicePlanConfiguration config)
		{
			string offerId2;
			if (!config.TryGetHydratedOfferId(programId, offerId, out offerId2))
			{
				offerId2 = this.DataObject.OfferId;
			}
			return sharedConfigurationUnit.OfferId.Equals(offerId2, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsPilotOfferIdMatched(string programId, string offerId, ExchangeConfigurationUnit sharedConfigurationUnit, ServicePlanConfiguration config)
		{
			string dehydratedOfferId;
			string offerId2;
			if (!config.TryGetReversePilotOfferId(programId, offerId, out dehydratedOfferId) || !config.TryGetHydratedOfferId(programId, dehydratedOfferId, out offerId2))
			{
				offerId2 = this.DataObject.OfferId;
			}
			return sharedConfigurationUnit.OfferId.Equals(offerId2, StringComparison.OrdinalIgnoreCase);
		}

		private bool IsHydratedPilotOfferIdMatched(string programId, string offerId, ExchangeConfigurationUnit sharedConfigurationUnit, ServicePlanConfiguration config)
		{
			if (this.DataObject.IsStaticConfigurationShared)
			{
				string pilotOfferId;
				string dehydratedOfferId;
				string offerId2;
				if (!config.TryGetReverseHydratedOfferId(programId, offerId, out pilotOfferId) || !config.TryGetReversePilotOfferId(programId, pilotOfferId, out dehydratedOfferId) || !config.TryGetHydratedOfferId(programId, dehydratedOfferId, out offerId2))
				{
					offerId2 = this.DataObject.OfferId;
				}
				return sharedConfigurationUnit.OfferId.Equals(offerId2, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.Force || !this.confirmLastSharedConfiguration || base.ShouldContinue(Strings.ConfirmDisableLastSharedConfiguration(this.Identity.ToString())))
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private const string ParameterSetSharedConfiguration = "SharedConfiguration";

		private const string ParameterSetSharedConfigurationInfo = "SharedConfigurationInfo";

		private const string ParameterSetSharedConfigurationRemove = "SharedConfigurationRemove";

		private const string ParameterSetAddRelocationConstraint = "AddRelocationConstraint";

		private const string ParameterSetRemoveRelocationConstraint = "RemoveRelocationConstraint";

		private const string RelocationConstraintTypeParameter = "RelocationConstraintType";

		private const string RelocationConstraintExpirationInDaysParameter = "RelocationConstraintExpirationInDays";

		private List<ExchangeConfigurationUnit> sharedConfigurationUnits;

		private bool confirmLastSharedConfiguration;
	}
}
