using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class AdminRoleGroups : DataSourceService, IRoleGroups, IDataSourceService<AdminRoleGroupFilter, AdminRoleGroupRow, AdminRoleGroupObject, SetAdminRoleGroupParameter, NewAdminRoleGroupParameter>, IDataSourceService<AdminRoleGroupFilter, AdminRoleGroupRow, AdminRoleGroupObject, SetAdminRoleGroupParameter, NewAdminRoleGroupParameter, BaseWebServiceParameters>, IEditListService<AdminRoleGroupFilter, AdminRoleGroupRow, AdminRoleGroupObject, NewAdminRoleGroupParameter, BaseWebServiceParameters>, IGetListService<AdminRoleGroupFilter, AdminRoleGroupRow>, INewObjectService<AdminRoleGroupRow, NewAdminRoleGroupParameter>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<AdminRoleGroupObject, SetAdminRoleGroupParameter, AdminRoleGroupRow>, IGetObjectService<AdminRoleGroupObject>, IGetObjectForListService<AdminRoleGroupRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?ResultSize&Filter@R:Organization")]
		public PowerShellResults<AdminRoleGroupRow> GetList(AdminRoleGroupFilter filter, SortOptions sort)
		{
			return base.GetList<AdminRoleGroupRow, AdminRoleGroupFilter>("Get-RoleGroup", filter, sort, "Name");
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?Identity@R:Organization")]
		public PowerShellResults<AdminRoleGroupObject> GetObject(Identity identity)
		{
			return base.GetObject<AdminRoleGroupObject>(new PSCommand().AddCommand("Get-RoleGroup").AddParameter("ReadFromDomainController"), identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?Identity@R:Organization")]
		public PowerShellResults<AdminRoleGroupRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<AdminRoleGroupRow>(new PSCommand().AddCommand("Get-RoleGroup").AddParameter("ReadFromDomainController"), identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?Identity@R:Organization")]
		public PowerShellResults<AdminRoleGroupObject> GetObjectForNew(Identity identity)
		{
			PowerShellResults<AdminRoleGroupObject> @object = base.GetObject<AdminRoleGroupObject>("Get-RoleGroup", identity);
			if (@object.SucceededWithValue && !@object.Value.RoleGroup.IsReadOnly)
			{
				AdminRoleGroupObject value = @object.Value;
				string text = Strings.CopyOf(value.Name);
				if (text.Length > 64)
				{
					text = text.Substring(0, 64).Trim();
				}
				value.Name = text;
				value.Members.Clear();
			}
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-RoleGroup@W:Organization")]
		public PowerShellResults<AdminRoleGroupRow> NewObject(NewAdminRoleGroupParameter properties)
		{
			properties.FaultIfNull();
			PowerShellResults<AdminRoleGroupRow> powerShellResults = new PowerShellResults<AdminRoleGroupRow>();
			if (properties.IsScopeModified)
			{
				if (properties.IsOrganizationalUnit)
				{
					if (string.IsNullOrEmpty(properties.ManagementScopeId))
					{
						throw new FaultException(Strings.InvalidOrganizationalUnit(properties.ManagementScopeId));
					}
					OrganizationalUnits organizationalUnits = new OrganizationalUnits();
					Identity identity = new Identity(properties.ManagementScopeId, properties.ManagementScopeId);
					PowerShellResults<ExtendedOrganizationalUnit> powerShellResults2 = powerShellResults.MergeErrors<ExtendedOrganizationalUnit>(organizationalUnits.GetObject(identity));
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
					ExtendedOrganizationalUnit value = powerShellResults2.Value;
					properties.RecipientOrganizationalUnitScope = new Identity(value.Id, value.Name);
				}
				else
				{
					PowerShellResults<ManagementScopeRow> managementScope = this.GetManagementScope(properties.ManagementScopeId, powerShellResults);
					if (powerShellResults.Failed)
					{
						return powerShellResults;
					}
					if (managementScope != null && managementScope.SucceededWithValue)
					{
						ManagementScopeRow value2 = managementScope.Value;
						if (value2.ScopeRestrictionType == ScopeRestrictionType.RecipientScope)
						{
							properties.RecipientWriteScope = value2.Identity;
						}
						else if (value2.ScopeRestrictionType == ScopeRestrictionType.ServerScope)
						{
							properties.ConfigWriteScope = value2.Identity;
						}
					}
				}
			}
			powerShellResults = base.NewObject<AdminRoleGroupRow, NewAdminRoleGroupParameter>("New-RoleGroup", properties);
			if (powerShellResults.Succeeded && powerShellResults.HasWarnings)
			{
				powerShellResults.Warnings = null;
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-RoleGroup?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-RoleGroup", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "FFO+Get-RoleGroup?Identity@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?Identity@R:Organization+Set-RoleGroup?Identity")]
		public PowerShellResults<AdminRoleGroupRow> SetObject(Identity identity, SetAdminRoleGroupParameter properties)
		{
			properties.FaultIfNull();
			PowerShellResults<AdminRoleGroupRow> powerShellResults = new PowerShellResults<AdminRoleGroupRow>();
			powerShellResults = this.HandleManagementRoleAssignments(identity, properties, powerShellResults);
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			this.UpdateRoleGroupMembers(identity, properties.Members, powerShellResults);
			if (powerShellResults.Failed)
			{
				return powerShellResults;
			}
			powerShellResults = this.SetRoleGroup(identity, properties, powerShellResults);
			if (powerShellResults.HasWarnings)
			{
				powerShellResults.Warnings = null;
			}
			return powerShellResults;
		}

		private void UpdateRoleGroupMembers(Identity identity, Identity[] members, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (members != null)
			{
				RoleGroupMembers roleGroupMembers = new RoleGroupMembers();
				results.MergeErrors<RoleGroupMembersRow>(roleGroupMembers.SetObject(identity, new SetRoleGroupMembersParameter
				{
					Members = members
				}));
			}
		}

		private PowerShellResults<AdminRoleGroupRow> SetRoleGroup(Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (properties.Description != null || properties.Name != null)
			{
				results = base.SetObject<AdminRoleGroupObject, SetAdminRoleGroupParameter, AdminRoleGroupRow>("Set-RoleGroup", identity, properties);
			}
			return results;
		}

		private PowerShellResults<AdminRoleGroupRow> HandleManagementRoleAssignments(Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (!properties.IsRolesModified && !properties.IsScopeModified)
			{
				return results;
			}
			if (properties.IsOrganizationalUnit)
			{
				this.GetOrganizationalUnit(properties, results);
			}
			this.GetOriginalObject(identity, properties, results);
			if (results.Failed)
			{
				return results;
			}
			Identity[] roles = properties.Roles;
			IEnumerable<Identity> newCollection = (roles != null) ? ((IEnumerable<Identity>)roles) : properties.OriginalObject.RoleIdentities;
			Delta<Identity> delta = properties.OriginalObject.RoleIdentities.CalculateDelta(newCollection);
			if (delta.UnchangedObjects.Count == 0 && delta.AddedObjects.Count == 0)
			{
				throw new FaultException(Strings.NoRoles);
			}
			if (!properties.IsScopeModified)
			{
				this.PrepareObjectsForRoleAssignmentWithoutScope(identity, properties, results);
			}
			else
			{
				this.PrepareObjectsForForRoleAssignmentWithScope(identity, properties, results);
			}
			if (results.Failed)
			{
				return results;
			}
			return this.UpdateRoleAssignments(delta, identity, properties, results);
		}

		private void PrepareObjectsForForRoleAssignmentWithScope(Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (!properties.IsOrganizationalUnit)
			{
				PowerShellResults<ManagementScopeRow> managementScope = this.GetManagementScope(properties.ManagementScopeId, results);
				if (results.Failed)
				{
					return;
				}
				if (managementScope != null && managementScope.SucceededWithValue)
				{
					properties.ManagementScopeRow = managementScope.Value;
				}
			}
		}

		private void PrepareObjectsForRoleAssignmentWithoutScope(Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (properties.OriginalObject.IsOrganizationalUnit)
			{
				PowerShellResults<ExtendedOrganizationalUnit> organizationalUnit = this.GetOrganizationalUnit(properties.OriginalObject.ManagementScopeId, results);
				if (results.Failed)
				{
					return;
				}
				if (organizationalUnit.SucceededWithValue)
				{
					properties.OrganizationalUnitRow = organizationalUnit.Value;
					return;
				}
			}
			else
			{
				PowerShellResults<ManagementScopeRow> managementScope = this.GetManagementScope(properties.OriginalObject.ManagementScopeId, results);
				if (results.Failed)
				{
					return;
				}
				if (managementScope != null && managementScope.SucceededWithValue)
				{
					properties.ManagementScopeRow = managementScope.Value;
				}
			}
		}

		private PowerShellResults<AdminRoleGroupRow> UpdateRoleAssignments(Delta<Identity> delta, Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			ManagementScopeRow managementScopeRow = properties.ManagementScopeRow;
			ManagementRoleAssignments roleAssignmentsWebService = new ManagementRoleAssignments();
			results = this.SetRoleAssignments(delta.UnchangedObjects, roleAssignmentsWebService, identity, managementScopeRow, properties.OrganizationalUnitRow, results);
			if (results.Failed)
			{
				return results;
			}
			results = this.AddRoleAssignments(delta.AddedObjects, roleAssignmentsWebService, identity, managementScopeRow, properties.OrganizationalUnitRow, results);
			if (results.Failed)
			{
				return results;
			}
			results = this.RemoveRoleAssignments(delta.RemovedObjects, roleAssignmentsWebService, identity, results);
			if (results.Failed)
			{
				return results;
			}
			return results;
		}

		private PowerShellResults<AdminRoleGroupRow> AddRoleAssignments(IEnumerable<Identity> addedRoles, ManagementRoleAssignments roleAssignmentsWebService, Identity identity, ManagementScopeRow scopeRow, ExtendedOrganizationalUnit ouRow, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (addedRoles != null)
			{
				foreach (Identity identity2 in addedRoles)
				{
					NewManagementRoleAssignment newManagementRoleAssignment = new NewManagementRoleAssignment();
					newManagementRoleAssignment.Role = identity2;
					newManagementRoleAssignment.SecurityGroup = identity.RawIdentity;
					this.SetScopeInfoInParameter(identity2, newManagementRoleAssignment, scopeRow, ouRow, results);
					if (results.Failed)
					{
						return results;
					}
					results.MergeErrors<ManagementRoleAssignment>(roleAssignmentsWebService.NewObject(newManagementRoleAssignment));
				}
				return results;
			}
			return results;
		}

		private PowerShellResults<AdminRoleGroupRow> RemoveRoleAssignments(IEnumerable<Identity> removedRoles, ManagementRoleAssignments roleAssignmentsWebService, Identity identity, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (removedRoles != null)
			{
				foreach (Identity roleIdentity in removedRoles)
				{
					PowerShellResults<ManagementRoleAssignment> roleAssignments = this.GetRoleAssignments(roleIdentity, identity, roleAssignmentsWebService);
					if (roleAssignments.Failed)
					{
						results.MergeErrors<ManagementRoleAssignment>(roleAssignments);
						return results;
					}
					if (roleAssignments != null && roleAssignments.Output != null)
					{
						ManagementRoleAssignment[] output = roleAssignments.Output;
						IEnumerable<Identity> source = from entry in output
						where entry.DelegationType == RoleAssignmentDelegationType.Regular
						select entry.Identity;
						results.MergeErrors(roleAssignmentsWebService.RemoveObjects(source.ToArray<Identity>(), null));
						if (results.Failed)
						{
							return results;
						}
					}
				}
				return results;
			}
			return results;
		}

		private PowerShellResults<AdminRoleGroupRow> SetRoleAssignments(IEnumerable<Identity> unchangedRoles, ManagementRoleAssignments roleAssignmentsWebService, Identity identity, ManagementScopeRow scopeRow, ExtendedOrganizationalUnit ouRow, PowerShellResults<AdminRoleGroupRow> results)
		{
			if ((ouRow != null || scopeRow != null) && unchangedRoles != null)
			{
				foreach (Identity roleIdentity in unchangedRoles)
				{
					PowerShellResults<ManagementRoleAssignment> roleAssignments = this.GetRoleAssignments(roleIdentity, identity, roleAssignmentsWebService);
					if (roleAssignments.Failed)
					{
						results.MergeErrors<ManagementRoleAssignment>(roleAssignments);
						return results;
					}
					SetManagementRoleAssignment properties = this.SetScopeInfoInParameter(roleIdentity, null, scopeRow, ouRow, results);
					if (results.Failed)
					{
						return results;
					}
					ManagementRoleAssignment[] output = roleAssignments.Output;
					foreach (ManagementRoleAssignment managementRoleAssignment in output)
					{
						if (managementRoleAssignment.DelegationType == RoleAssignmentDelegationType.Regular)
						{
							results.MergeErrors<ManagementRoleAssignment>(roleAssignmentsWebService.SetObject(managementRoleAssignment.Identity, properties));
							if (results.Failed)
							{
								return results;
							}
						}
					}
				}
				return results;
			}
			return results;
		}

		private void GetOriginalObject(Identity identity, SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			PowerShellResults<AdminRoleGroupObject> powerShellResults = results.MergeErrors<AdminRoleGroupObject>(base.GetObject<AdminRoleGroupObject>(new PSCommand().AddCommand("Get-RoleGroup").AddParameter("ReadFromDomainController"), identity));
			if (powerShellResults.SucceededWithValue)
			{
				properties.OriginalObject = powerShellResults.Value;
			}
		}

		private void GetOrganizationalUnit(SetAdminRoleGroupParameter properties, PowerShellResults<AdminRoleGroupRow> results)
		{
			if (string.IsNullOrEmpty(properties.ManagementScopeId))
			{
				throw new FaultException(Strings.InvalidOrganizationalUnit(properties.ManagementScopeId));
			}
			PowerShellResults<ExtendedOrganizationalUnit> organizationalUnit = this.GetOrganizationalUnit(properties.ManagementScopeId, results);
			if (results.Failed)
			{
				throw new FaultException(Strings.InvalidOrganizationalUnit(properties.ManagementScopeId));
			}
			if (organizationalUnit.SucceededWithValue)
			{
				properties.OrganizationalUnitRow = organizationalUnit.Value;
			}
		}

		private PowerShellResults<ExtendedOrganizationalUnit> GetOrganizationalUnit(string ou, PowerShellResults<AdminRoleGroupRow> results)
		{
			OrganizationalUnits organizationalUnits = new OrganizationalUnits();
			Identity identity = new Identity(ou, ou);
			return results.MergeErrors<ExtendedOrganizationalUnit>(organizationalUnits.GetObject(identity));
		}

		private PowerShellResults<ManagementScopeRow> GetManagementScope(string managementScope, PowerShellResults<AdminRoleGroupRow> results)
		{
			PowerShellResults<ManagementScopeRow> result = null;
			if (!string.IsNullOrEmpty(managementScope) && !ManagementScopeRow.IsDefaultScope(managementScope))
			{
				ManagementScopes managementScopes = new ManagementScopes();
				Identity identity = new Identity(managementScope, managementScope);
				result = results.MergeErrors<ManagementScopeRow>(managementScopes.GetObject(identity));
			}
			return result;
		}

		private PowerShellResults<ManagementRoleAssignment> GetRoleAssignments(Identity roleIdentity, Identity roleAssignee, ManagementRoleAssignments roleAssignmentsWebService)
		{
			return roleAssignmentsWebService.GetList(new ManagementRoleAssignmentFilter
			{
				Role = roleIdentity,
				Delegating = false,
				RoleAssignee = roleAssignee
			}, null);
		}

		private PowerShellResults<ManagementRoleObject> GetManagementRole(Identity roleIdentity, PowerShellResults<AdminRoleGroupRow> results)
		{
			ManagementRoles managementRoles = new ManagementRoles();
			return results.MergeErrors<ManagementRoleObject>(managementRoles.GetObject(roleIdentity));
		}

		private SetManagementRoleAssignment SetScopeInfoInParameter(Identity roleIdentity, SetManagementRoleAssignment param, ManagementScopeRow scopeRow, ExtendedOrganizationalUnit ouRow, PowerShellResults<AdminRoleGroupRow> results)
		{
			SetManagementRoleAssignment setManagementRoleAssignment = param;
			if (setManagementRoleAssignment == null)
			{
				setManagementRoleAssignment = new SetManagementRoleAssignment();
			}
			if (ouRow != null || scopeRow != null)
			{
				if (scopeRow != null && scopeRow.ScopeRestrictionType == ScopeRestrictionType.ServerScope)
				{
					setManagementRoleAssignment.RecipientWriteScope = null;
					setManagementRoleAssignment.ConfigWriteScope = scopeRow.Identity;
				}
				else
				{
					if (!Util.IsDataCenter)
					{
						if (scopeRow != null && scopeRow.ScopeRestrictionType == ScopeRestrictionType.DatabaseScope)
						{
							setManagementRoleAssignment.ConfigWriteScope = scopeRow.Identity;
						}
						else
						{
							setManagementRoleAssignment.ConfigWriteScope = null;
						}
					}
					if (ouRow != null)
					{
						setManagementRoleAssignment.OrganizationalUnit = new Identity(ouRow.Id, ouRow.Name);
					}
					else if (scopeRow != null && scopeRow.ScopeRestrictionType == ScopeRestrictionType.RecipientScope)
					{
						setManagementRoleAssignment.RecipientWriteScope = scopeRow.Identity;
					}
				}
			}
			return setManagementRoleAssignment;
		}

		internal const string GetCmdlet = "Get-RoleGroup";

		internal const string SetCmdlet = "Set-RoleGroup";

		internal const string NewCmdlet = "New-RoleGroup";

		internal const string RemoveCmdlet = "Remove-RoleGroup";

		internal const int NameMaxLength = 64;

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		private const string GetListRole = "Get-RoleGroup?ResultSize&Filter@R:Organization";

		private const string GetObjectRole = "Get-RoleGroup?Identity@R:Organization";

		private const string NewObjectRole = "New-RoleGroup@W:Organization";

		private const string RemoveObjectsRole = "Remove-RoleGroup?Identity@W:Organization";

		private const string SetObjectRole = "Get-RoleGroup?Identity@R:Organization+Set-RoleGroup?Identity";
	}
}
