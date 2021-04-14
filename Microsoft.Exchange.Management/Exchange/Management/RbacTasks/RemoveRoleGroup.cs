using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Remove", "RoleGroup", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRoleGroup : RemoveRecipientObjectTask<RoleGroupIdParameter, ADGroup>
	{
		[Parameter(Mandatory = false)]
		public new SwitchParameter ForReconciliation
		{
			get
			{
				return base.ForReconciliation;
			}
			set
			{
				base.ForReconciliation = value;
			}
		}

		private ADObjectId RootOrgUSGContainerId
		{
			get
			{
				if (this.rootOrgUSGContainerId == null)
				{
					this.rootOrgUSGContainerId = RoleGroupCommon.GetRootOrgUsgContainerId(this.ConfigurationSession, base.ServerSettings, base.PartitionOrRootOrgGlobalCatalogSession, base.CurrentOrganizationId);
				}
				return this.rootOrgUSGContainerId;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string managedBy = RoleGroupCommon.NamesFromObjects(this.roleGroup.ManagedBy);
				string roles = RoleGroupCommon.NamesFromObjects(this.roleGroup.Roles);
				string roleAssignments = RoleGroupCommon.NamesFromObjects(this.roleGroup.RoleAssignments);
				return Strings.ConfirmationMessageRemoveRoleGroup(this.Identity.ToString(), roles, managedBy, roleAssignments);
			}
		}

		internal new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return new SwitchParameter(false);
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
		public SwitchParameter RemoveWellKnownObjectGuid
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveWellKnownObjectGuid"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveWellKnownObjectGuid"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			this.writableConfigSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 146, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RBAC\\RoleGroup\\RemoveRoleGroup.cs");
			return base.CreateSession();
		}

		protected override void InternalValidate()
		{
			base.OptionalIdentityData.RootOrgDomainContainerId = this.RootOrgUSGContainerId;
			base.InternalValidate();
			if (!this.BypassSecurityGroupManagerCheck)
			{
				ADObjectId user;
				base.TryGetExecutingUserId(out user);
				RoleGroupCommon.ValidateExecutingUserHasGroupManagementRights(user, base.DataObject, base.ExchangeRunspaceConfig, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (RoleGroupCommon.IsPrecannedRoleGroup(base.DataObject, this.ConfigurationSession, new Guid[0]))
			{
				base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotDeletePrecannedRoleGroup(base.DataObject.Name)), ExchangeErrorCategory.Client, null);
			}
			RoleAssignmentsGlobalConstraints roleAssignmentsGlobalConstraints = new RoleAssignmentsGlobalConstraints(this.ConfigurationSession, base.TenantGlobalCatalogSession, new Task.ErrorLoggerDelegate(base.WriteError));
			roleAssignmentsGlobalConstraints.ValidateIsSafeToRemoveRoleGroup(base.DataObject, this.roleAssignmentResults, this);
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADGroup adgroup = (ADGroup)base.ResolveDataObject();
			this.writableConfigSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(this.writableConfigSession, adgroup.OrganizationId, base.OrgWideSessionSettings, true);
			Result<ExchangeRoleAssignment>[] array = this.writableConfigSession.FindRoleAssignmentsByUserIds(new ADObjectId[]
			{
				adgroup.Id
			}, false);
			this.roleAssignmentResults = array;
			this.roleGroup = new RoleGroup(adgroup, array);
			return adgroup;
		}

		protected override void InternalProcessRecord()
		{
			if (!this.Force && SharedConfiguration.IsSharedConfiguration(base.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(base.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.roleAssignmentResults.Length != 0)
			{
				((IRecipientSession)base.DataSession).VerifyIsWithinScopes(base.DataObject, true);
			}
			bool flag = true;
			foreach (Result<ExchangeRoleAssignment> result in this.roleAssignmentResults)
			{
				string id = result.Data.Id.ToString();
				try
				{
					base.WriteVerbose(Strings.VerboseRemovingRoleAssignment(id));
					result.Data.Session.Delete(result.Data);
					base.WriteVerbose(Strings.VerboseRemovedRoleAssignment(id));
				}
				catch (Exception ex)
				{
					flag = false;
					if (!base.IsKnownException(ex))
					{
						throw;
					}
					this.WriteWarning(Strings.WarningCouldNotRemoveRoleAssignment(id, ex.Message));
				}
			}
			if (!flag)
			{
				base.WriteError(new TaskException(Strings.ErrorCouldNotRemoveRoleAssignments(base.DataObject.Id.ToString())), ExchangeErrorCategory.ServerOperation, base.DataObject);
			}
			if (this.RemoveWellKnownObjectGuid)
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit = this.writableConfigSession.Read<ExchangeConfigurationUnit>(base.DataObject.OrganizationId.ConfigurationUnit);
				foreach (DNWithBinary dnwithBinary in exchangeConfigurationUnit.OtherWellKnownObjects)
				{
					if (dnwithBinary.DistinguishedName.Equals(base.DataObject.DistinguishedName, StringComparison.OrdinalIgnoreCase))
					{
						exchangeConfigurationUnit.OtherWellKnownObjects.Remove(dnwithBinary);
						this.writableConfigSession.Save(exchangeConfigurationUnit);
						break;
					}
				}
			}
			base.InternalProcessRecord();
		}

		private ADObjectId rootOrgUSGContainerId;

		private IConfigurationSession writableConfigSession;

		private Result<ExchangeRoleAssignment>[] roleAssignmentResults;

		private RoleGroup roleGroup;
	}
}
