using System;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Set", "RoleGroup", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRoleGroup : SetRecipientObjectTask<RoleGroupIdParameter, RoleGroup, ADGroup>
	{
		private ADObjectId RootOrgUSGContainerId
		{
			get
			{
				if (this.rootOrgUSGContainerId == null)
				{
					IRecipientSession partitionOrRootOrgGlobalCatalogSession = base.PartitionOrRootOrgGlobalCatalogSession;
					this.rootOrgUSGContainerId = RoleGroupCommon.RoleGroupContainerId(DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, partitionOrRootOrgGlobalCatalogSession.ConsistencyMode, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionOrRootOrgGlobalCatalogSession.SessionSettings.PartitionId), 53, "RootOrgUSGContainerId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleGroup\\SetRoleGroup.cs"), this.ConfigurationSession);
				}
				return this.rootOrgUSGContainerId;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string managedBy = RoleGroupCommon.NamesFromObjects(base.DynamicParametersInstance.ManagedBy);
				string roles = RoleGroupCommon.NamesFromObjects(this.roleGroup.Roles);
				return Strings.ConfirmationMessageSetRoleGroup(this.Identity.ToString(), roles, managedBy);
			}
		}

		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override RoleGroupIdParameter Identity
		{
			get
			{
				return (RoleGroupIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public MultiValuedProperty<SecurityPrincipalIdParameter> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalIdParameter>)base.Fields[WindowsGroupSchema.ManagedBy];
			}
			set
			{
				base.Fields[WindowsGroupSchema.ManagedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string DisplayName
		{
			get
			{
				return (string)base.Fields["DisplayName"];
			}
			set
			{
				base.Fields["DisplayName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassSecurityGroupManagerCheck
		{
			get
			{
				if (DatacenterRegistry.IsForefrontForOffice())
				{
					return true;
				}
				return (SwitchParameter)(base.Fields["BypassSecurityGroupManagerCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassSecurityGroupManagerCheck"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "ExchangeDatacenterCrossForestParameterSet")]
		public SecurityIdentifier LinkedForeignGroupSid
		{
			get
			{
				return (SecurityIdentifier)base.Fields[RoleGroupParameters.ParameterLinkedForeignGroupSid];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedForeignGroupSid] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "crossforest")]
		[ValidateNotNull]
		public UniversalSecurityGroupIdParameter LinkedForeignGroup
		{
			get
			{
				return (UniversalSecurityGroupIdParameter)base.Fields[RoleGroupParameters.ParameterLinkedForeignGroup];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedForeignGroup] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "crossforest")]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields[RoleGroupParameters.ParameterLinkedDomainController];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedDomainController] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "crossforest")]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields[RoleGroupParameters.ParameterLinkedCredential];
			}
			set
			{
				base.Fields[RoleGroupParameters.ParameterLinkedCredential] = value;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDirectoryObjectId
		{
			get
			{
				if (base.Fields["ExternalDirectoryObjectId"] == null)
				{
					return Guid.Empty;
				}
				return (Guid)base.Fields["ExternalDirectoryObjectId"];
			}
			set
			{
				base.Fields["ExternalDirectoryObjectId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid WellKnownObjectGuid
		{
			get
			{
				if (base.Fields["WellKnownObjectGuid"] == null)
				{
					return Guid.Empty;
				}
				return (Guid)base.Fields["WellKnownObjectGuid"];
			}
			set
			{
				base.Fields["WellKnownObjectGuid"] = value;
			}
		}

		internal new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return new SwitchParameter(false);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if ("crossforest" == base.ParameterSetName)
			{
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					this.linkedGroupSid = MailboxTaskHelper.GetSidFromAnotherForest<ADGroup>(this.LinkedForeignGroup, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADGroup>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorLinkedGroupInTheCurrentForest), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotFoundOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotFoundOnDomainController), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotUniqueOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorGroupNotUniqueOnDomainController), new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorVerifyLinkedGroupForest));
				}
				catch (PSArgumentException exception)
				{
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			if (base.Fields.IsModified(WindowsGroupSchema.ManagedBy))
			{
				base.DynamicParametersInstance.ManagedBy = base.ResolveIdParameterCollection<SecurityPrincipalIdParameter, ADRecipient, ADObjectId>(this.ManagedBy, base.TenantGlobalCatalogSession, null, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorRecipientNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorRecipientNotUnique), null, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			if (!this.BypassSecurityGroupManagerCheck)
			{
				ADObjectId user;
				base.TryGetExecutingUserId(out user);
				RoleGroupCommon.ValidateExecutingUserHasGroupManagementRights(user, adgroup, base.ExchangeRunspaceConfig, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if ("crossforest" == base.ParameterSetName && adgroup.RoleGroupType == RoleGroupType.Standard)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotChangeRoleGroupType), (ErrorCategory)1000, null);
			}
			if ("ExchangeDatacenterCrossForestParameterSet" == base.ParameterSetName)
			{
				if (Datacenter.ExchangeSku.ExchangeDatacenter != Datacenter.GetExchangeSku() && Datacenter.ExchangeSku.DatacenterDedicated != Datacenter.GetExchangeSku())
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorLinkedSidParameterNotAllowed(RoleGroupParameters.ParameterLinkedForeignGroupSid)), (ErrorCategory)1000, null);
				}
				this.linkedGroupSid = this.LinkedForeignGroupSid;
			}
			if ("crossforest" == base.ParameterSetName || "ExchangeDatacenterCrossForestParameterSet" == base.ParameterSetName)
			{
				adgroup.ForeignGroupSid = this.linkedGroupSid;
				if (adgroup.Members.Count > 0)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorLinkedRoleGroupCannotHaveMembers), (ErrorCategory)1000, null);
				}
			}
			if (base.Fields.IsModified("DisplayName"))
			{
				adgroup[RoleGroupSchema.DisplayName] = this.DisplayName;
			}
			this.roleGroup = RoleGroupCommon.PopulateRoleAssignmentsAndConvert(adgroup, this.ConfigurationSession);
			if (base.Fields.IsModified("Description"))
			{
				adgroup[ADGroupSchema.RoleGroupDescription] = (string.IsNullOrEmpty(this.Description) ? null : this.Description);
			}
			if (this.ExternalDirectoryObjectId != Guid.Empty)
			{
				adgroup.ExternalDirectoryObjectId = this.ExternalDirectoryObjectId.ToString();
			}
			TaskLogger.LogExit();
			return adgroup;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.OptionalIdentityData.RootOrgDomainContainerId = this.RootOrgUSGContainerId;
			base.InternalValidate();
			MailboxTaskHelper.ValidateGroupManagedBy(base.TenantGlobalCatalogSession, this.DataObject, null, RoleGroupCommon.OwnerRecipientTypeDetails, true, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			if (this.WellKnownObjectGuid != Guid.Empty)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 424, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleGroup\\SetRoleGroup.cs");
				RoleGroupCommon.StampWellKnownObjectGuid(tenantOrTopologyConfigurationSession, this.DataObject.OrganizationId, this.DataObject.DistinguishedName, this.WellKnownObjectGuid);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject != this.roleGroup.DataObject)
			{
				throw new ArgumentException("dataObject");
			}
			return this.roleGroup;
		}

		private ADObjectId rootOrgUSGContainerId;

		private RoleGroup roleGroup;

		private SecurityIdentifier linkedGroupSid;
	}
}
