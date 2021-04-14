using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RoleAssignmentPolicies : DataSourceService, IRoleAssignmentPolicies, IDataSourceService<RoleAssignmentPolicyFilter, RoleAssignmentPolicyRow, RoleAssignmentPolicy, SetRoleAssignmentPolicy, NewRoleAssignmentPolicy>, IDataSourceService<RoleAssignmentPolicyFilter, RoleAssignmentPolicyRow, RoleAssignmentPolicy, SetRoleAssignmentPolicy, NewRoleAssignmentPolicy, BaseWebServiceParameters>, IEditListService<RoleAssignmentPolicyFilter, RoleAssignmentPolicyRow, RoleAssignmentPolicy, NewRoleAssignmentPolicy, BaseWebServiceParameters>, IGetListService<RoleAssignmentPolicyFilter, RoleAssignmentPolicyRow>, INewObjectService<RoleAssignmentPolicyRow, NewRoleAssignmentPolicy>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<RoleAssignmentPolicy, SetRoleAssignmentPolicy, RoleAssignmentPolicyRow>, IGetObjectService<RoleAssignmentPolicy>, IGetObjectForListService<RoleAssignmentPolicyRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleAssignmentPolicy@R:Organization")]
		public PowerShellResults<RoleAssignmentPolicyRow> GetList(RoleAssignmentPolicyFilter filter, SortOptions sort)
		{
			return base.GetList<RoleAssignmentPolicyRow, RoleAssignmentPolicyFilter>("Get-RoleAssignmentPolicy", filter, sort);
		}

		public PowerShellResults<EndUserRoleRow> GetAssignedEndUserRoles(RoleAssignmentPolicyFilter filter, SortOptions sort)
		{
			PowerShellResults<RoleAssignmentPolicy> @object = this.GetObject(filter.Policy);
			PowerShellResults<EndUserRoleRow> powerShellResults = new PowerShellResults<EndUserRoleRow>
			{
				ErrorRecords = @object.ErrorRecords,
				Warnings = @object.Warnings
			};
			if (@object.SucceededWithValue)
			{
				EndUserRoles endUserRoles = new EndUserRoles();
				powerShellResults.MergeAll(endUserRoles.GetList(null, null));
				if (powerShellResults.Succeeded)
				{
					List<EndUserRoleRow> list = new List<EndUserRoleRow>();
					foreach (EndUserRoleRow endUserRoleRow in powerShellResults.Output)
					{
						if (@object.Value.AssignedEndUserRoles.Contains(endUserRoleRow.Identity))
						{
							list.Add(endUserRoleRow);
						}
					}
					powerShellResults.Output = list.ToArray();
				}
			}
			return powerShellResults;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleAssignmentPolicy@R:Organization")]
		public PowerShellResults<RoleAssignmentPolicyRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<RoleAssignmentPolicyRow>("Get-RoleAssignmentPolicy", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Dedicated+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-MailboxPlan@R:Organization+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Enterprise+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization")]
		public PowerShellResults<RoleAssignmentPolicy> GetObject(Identity identity)
		{
			PowerShellResults<RoleAssignmentPolicy> @object = base.GetObject<RoleAssignmentPolicy>("Get-RoleAssignmentPolicy", identity);
			if (!@object.HasValue)
			{
				return @object;
			}
			RoleAssignmentPolicy value = @object.Value;
			@object.Output = new RoleAssignmentPolicy[]
			{
				value
			};
			return @object;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-RoleAssignmentPolicy@W:Organization")]
		public PowerShellResults<RoleAssignmentPolicyRow> NewObject(NewRoleAssignmentPolicy properties)
		{
			return base.NewObject<RoleAssignmentPolicyRow, NewRoleAssignmentPolicy>("New-RoleAssignmentPolicy", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-RoleAssignmentPolicy?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-RoleAssignmentPolicy", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Dedicated+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Set-RoleAssignmentPolicy?Identity+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Get-MailboxPlan@R:Organization+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Enterprise+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Set-RoleAssignmentPolicy?Identity+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization")]
		public PowerShellResults<RoleAssignmentPolicyRow> SetObject(Identity identity, SetRoleAssignmentPolicy properties)
		{
			properties.FaultIfNull();
			PowerShellResults<RoleAssignmentPolicyRow> powerShellResults;
			if (!string.IsNullOrEmpty(properties.Name) || !string.IsNullOrEmpty(properties.Description))
			{
				powerShellResults = base.SetObject<RoleAssignmentPolicyRow, SetRoleAssignmentPolicy>("Set-RoleAssignmentPolicy", identity, properties);
			}
			else
			{
				powerShellResults = base.GetObject<RoleAssignmentPolicyRow>("Get-RoleAssignmentPolicy", identity);
			}
			if (!powerShellResults.SucceededWithValue)
			{
				powerShellResults.Output = null;
				return powerShellResults;
			}
			PowerShellResults<RoleAssignmentPolicy> @object = this.GetObject(identity);
			if (@object.SucceededWithValue && properties.AssignedEndUserRoles != null)
			{
				this.UpdateRoleAssignments(@object, properties);
			}
			if (!@object.SucceededWithValue)
			{
				powerShellResults.MergeErrors<RoleAssignmentPolicy>(@object);
				powerShellResults.Output = null;
			}
			return powerShellResults;
		}

		private void UpdateRoleAssignments(PowerShellResults<RoleAssignmentPolicy> result, SetRoleAssignmentPolicy properties)
		{
			RoleAssignmentPolicy value = result.Value;
			ManagementRoleAssignments managementRoleAssignments = new ManagementRoleAssignments();
			Delta<Identity> delta = value.AssignedEndUserRoles.CalculateDelta(properties.AssignedEndUserRoles);
			if (delta.RemovedObjects != null && delta.RemovedObjects.Count > 0)
			{
				result.MergeErrors(this.RemoveRoleAssignments(delta.RemovedObjects, value, managementRoleAssignments));
				if (result.Failed)
				{
					return;
				}
			}
			foreach (Identity role in delta.AddedObjects)
			{
				result.MergeErrors<ManagementRoleAssignment>(managementRoleAssignments.NewObject(new NewManagementRoleAssignment
				{
					Policy = value.Identity,
					Role = role
				}));
			}
		}

		private PowerShellResults RemoveRoleAssignments(List<Identity> roles, RoleAssignmentPolicy policy, ManagementRoleAssignments service)
		{
			PowerShellResults<ManagementRoleAssignment> list = service.GetList(new ManagementRoleAssignmentFilter
			{
				RoleAssignee = policy.Identity
			}, null);
			if (list.Failed)
			{
				return list;
			}
			List<Identity> list2 = new List<Identity>();
			foreach (ManagementRoleAssignment managementRoleAssignment in list.Output)
			{
				if (roles.Contains(managementRoleAssignment.Role.ToIdentity()))
				{
					list2.Add(managementRoleAssignment.Identity);
				}
			}
			return service.RemoveObjects(list2.ToArray(), null);
		}

		private const string Noun = "RoleAssignmentPolicy";

		internal const string GetCmdlet = "Get-RoleAssignmentPolicy";

		internal const string SetCmdlet = "Set-RoleAssignmentPolicy";

		internal const string NewCmdlet = "New-RoleAssignmentPolicy";

		internal const string RemoveCmdlet = "Remove-RoleAssignmentPolicy";

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		private const string GetListRole = "Get-RoleAssignmentPolicy@R:Organization";

		private const string GetObjectForListRole = "Get-RoleAssignmentPolicy@R:Organization";

		private const string GetObjectRole_Enterprise = "Enterprise+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization";

		private const string GetObjectRole_MultiTenant = "MultiTenant+Get-MailboxPlan@R:Organization+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization";

		private const string GetObjectRole_Dedicated = "Dedicated+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization";

		private const string NewObjectRole = "New-RoleAssignmentPolicy@W:Organization";

		private const string RemoveObjectRole = "Remove-RoleAssignmentPolicy?Identity@W:Organization";

		private const string SetObjectRole_Enterprise = "Enterprise+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Set-RoleAssignmentPolicy?Identity+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization";

		private const string SetObjectRole_MultiTenant = "MultiTenant+Get-MailboxPlan@R:Organization+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization";

		private const string SetObjectRole_Dedicated = "Dedicated+Get-ManagementRoleAssignment@R:Organization+Get-RoleAssignmentPolicy?Identity@R:Organization+Set-RoleAssignmentPolicy?Identity+Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization";
	}
}
