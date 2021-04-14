using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class OrgExtensions : DataSourceService, IOrgExtensions, IGetListService<ExtensionFilter, ExtensionRow>, IGetObjectService<ExtensionRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App@R:Organization")]
		public PowerShellResults<ExtensionRow> GetList(ExtensionFilter filter, SortOptions sort)
		{
			return base.GetList<ExtensionRow, ExtensionFilter>("Get-App", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-App?Identity@W:Organization")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-App", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App?Identity@R:Organization")]
		public PowerShellResults<ExtensionRow> GetObject(Identity identity)
		{
			return base.GetObject<ExtensionRow>("Get-App", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App?Identity@R:Organization")]
		public PowerShellResults<ExtensionRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<ExtensionRow>("Get-App", identity);
		}

		internal const string ReadScope = "@R:Organization";

		internal const string WriteScope = "@W:Organization";

		internal const string GetExtension = "Get-App";

		internal const string RemoveExtension = "Remove-App";

		internal const string NewExtension = "New-App";

		internal const string GetListRole = "Get-App@R:Organization";

		internal const string RemoveObjectsRole = "Remove-App?Identity@W:Organization";

		internal const string GetObjectRole = "Get-App?Identity@R:Organization";
	}
}
