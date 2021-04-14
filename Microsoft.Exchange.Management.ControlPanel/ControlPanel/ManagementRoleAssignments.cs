using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ManagementRoleAssignments : DataSourceService
	{
		public int NameMaxLength
		{
			get
			{
				return ManagementRoleAssignments.nameMaxLength;
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ManagementRoleAssignment@R:Organization")]
		public PowerShellResults<ManagementRoleAssignment> GetList(ManagementRoleAssignmentFilter filter, SortOptions sort)
		{
			return base.GetList<ManagementRoleAssignment, ManagementRoleAssignmentFilter>("Get-ManagementRoleAssignment", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-ManagementRoleAssignment?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-ManagementRoleAssignment", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "New-ManagementRoleAssignment@W:Organization")]
		public PowerShellResults<ManagementRoleAssignment> NewObject(NewManagementRoleAssignment properties)
		{
			return base.NewObject<ManagementRoleAssignment, NewManagementRoleAssignment>("New-ManagementRoleAssignment", properties);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Set-ManagementRoleAssignment@W:Organization")]
		public PowerShellResults<ManagementRoleAssignment> SetObject(Identity identity, SetManagementRoleAssignment properties)
		{
			return base.SetObject<ManagementRoleAssignment, SetManagementRoleAssignment, ManagementRoleAssignment>("Set-ManagementRoleAssignment", identity, properties);
		}

		private const string Noun = "ManagementRoleAssignment";

		internal const string NewCmdlet = "New-ManagementRoleAssignment";

		internal const string GetCmdlet = "Get-ManagementRoleAssignment";

		internal const string RemoveCmdlet = "Remove-ManagementRoleAssignment";

		internal const string ReadScope = "@R:Organization";

		internal const string SetCmdlet = "Set-ManagementRoleAssignment";

		internal const string WriteScope = "@W:Organization";

		internal const string GetListRole = "Get-ManagementRoleAssignment@R:Organization";

		private const string RemoveObjectsRole = "Remove-ManagementRoleAssignment?Identity@W:Organization";

		private const string NewObjectRole = "New-ManagementRoleAssignment@W:Organization";

		private const string SetObjectRole = "Set-ManagementRoleAssignment@W:Organization";

		internal const string ChangeRoleAssignmentRole = "Get-ManagementRole@R:Organization+New-ManagementRoleAssignment@W:Organization+Remove-ManagementRoleAssignment?Identity@W:Organization";

		private static int nameMaxLength = Util.GetMaxLengthFromDefinition(ADObjectSchema.Name);
	}
}
