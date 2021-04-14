using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class SupervisionListEntryService : DataSourceService, ISupervisionListEntryService, IGetListService<SupervisionListEntryFilter, SupervisionListEntryRow>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-SupervisionListEntry?Tag&Identity@R:Self")]
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-SupervisionListEntry?Tag&Identity@R:Organization")]
		public PowerShellResults<SupervisionListEntryRow> GetList(SupervisionListEntryFilter filter, SortOptions sort)
		{
			filter.Identity = Identity.FromExecutingUserId();
			return base.GetList<SupervisionListEntryRow, SupervisionListEntryFilter>("Get-SupervisionListEntry", filter, sort);
		}

		internal const string GetSupervisionListEntry = "Get-SupervisionListEntry";

		internal const string AddSupervisionListEntry = "Add-SupervisionListEntry";

		internal const string RemoveSupervisionListEntry = "Remove-SupervisionListEntry";

		internal const string ReadScopeOrg = "@R:Organization";

		internal const string ReadScopeSelf = "@R:Self";

		internal const string WriteScope = "@W:Organization";

		internal const string Allow = "allow";

		internal const string Reject = "reject";

		private const string GetListRoleOrg = "Get-SupervisionListEntry?Tag&Identity@R:Organization";

		private const string GetListRoleSelf = "Get-SupervisionListEntry?Tag&Identity@R:Self";
	}
}
