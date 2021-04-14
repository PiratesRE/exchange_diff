using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class SecurityPrincipalPicker : DataSourceService, ISecurityPrincipalPicker, IGetListService<SecurityPrincipalPickerFilter, SecurityPrincipalPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-SecurityPrincipal?ResultSize&Filter&Types&RoleGroupAssignable")]
		public PowerShellResults<SecurityPrincipalPickerObject> GetList(SecurityPrincipalPickerFilter filter, SortOptions sort)
		{
			return base.GetList<SecurityPrincipalPickerObject, SecurityPrincipalPickerFilter>("Get-SecurityPrincipal", filter, sort, "Name");
		}

		private const string GetListRole = "Get-SecurityPrincipal?ResultSize&Filter&Types&RoleGroupAssignable";
	}
}
