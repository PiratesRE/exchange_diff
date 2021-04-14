using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class RoleGroupMembers : DataSourceService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Update-RoleGroupMember?Identity")]
		public PowerShellResults<RoleGroupMembersRow> SetObject(Identity identity, SetRoleGroupMembersParameter properties)
		{
			return base.SetObject<RoleGroupMembersRow, SetRoleGroupMembersParameter, RoleGroupMembersRow>("Update-RoleGroupMember", identity, properties);
		}

		internal const string UpdateMembersCmdlet = "Update-RoleGroupMember";

		internal const string ReadScope = "@R:Organization";

		private const string SetObjectRole = "Update-RoleGroupMember?Identity";
	}
}
