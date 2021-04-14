using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ManagementScopes : DataSourceService, IManagementScopes, IGetListService<ManagementScopeFilter, ManagementScopeRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementScope?Exclusive@R:Organization")]
		public PowerShellResults<ManagementScopeRow> GetList(ManagementScopeFilter filter, SortOptions sort)
		{
			PowerShellResults<ManagementScopeRow> list = base.GetList<ManagementScopeRow, ManagementScopeFilter>("Get-ManagementScope", filter, null, "Name");
			if (list.Succeeded)
			{
				ManagementScopeRow[] output = list.Output;
				List<ManagementScopeRow> list2 = new List<ManagementScopeRow>(output.Length + 1);
				list2.Add(ManagementScopeRow.DefaultRow);
				list2.AddRange(output);
				list.Output = list2.ToArray();
			}
			return list;
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementScope?Identity@R:Organization")]
		public PowerShellResults<ManagementScopeRow> GetObject(Identity identity)
		{
			return base.GetObject<ManagementScopeRow>("Get-ManagementScope", identity);
		}

		internal const string NameProperty = "Name";

		internal const string ReadScope = "@R:Organization";

		internal const string RbacParameters = "?Exclusive";

		internal const string GetListScope = "Get-ManagementScope?Exclusive@R:Organization";

		private const string GetObjectRole = "Get-ManagementScope?Identity@R:Organization";
	}
}
