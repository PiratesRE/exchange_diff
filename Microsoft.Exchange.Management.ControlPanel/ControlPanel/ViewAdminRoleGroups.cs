using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ViewAdminRoleGroups : DataSourceService, IViewAdminRoleGroups, IGetObjectService<AdminRoleGroupObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-RoleGroup?Identity@R:Organization")]
		public PowerShellResults<AdminRoleGroupObject> GetObject(Identity identity)
		{
			PowerShellResults<AdminRoleGroupObject> @object = base.GetObject<AdminRoleGroupObject>(new PSCommand().AddCommand("Get-RoleGroup").AddParameter("ReadFromDomainController"), identity);
			if (@object.SucceededWithoutWarnings && @object.Value.IsMultipleScopesScenario)
			{
				@object.Warnings = @object.Warnings.Concat(new string[]
				{
					Strings.CannotCopyWarning
				}).ToArray<string>();
			}
			return @object;
		}

		internal const string GetCmdlet = "Get-RoleGroup";

		internal const string ReadScope = "@R:Organization";

		private const string GetObjectRole = "Get-RoleGroup?Identity@R:Organization";
	}
}
