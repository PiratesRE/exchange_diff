using System;
using System.Management.Automation;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class InstalledExtensions : DataSourceService, IInstalledExtensions, IGetListService<ExtensionFilter, ExtensionRow>, IGetObjectService<ExtensionRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App@R:Self")]
		public PowerShellResults<ExtensionRow> GetList(ExtensionFilter filter, SortOptions sort)
		{
			return base.GetList<ExtensionRow, ExtensionFilter>("Get-App", filter, sort);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Remove-App?Identity@W:Self")]
		public PowerShellResults RemoveObjects(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.RemoveObjects("Remove-App", identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App?Identity@R:Self")]
		public PowerShellResults<ExtensionRow> GetObject(Identity identity)
		{
			return base.GetObject<ExtensionRow>("Get-App", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Get-App?Identity@R:Self")]
		public PowerShellResults<ExtensionRow> GetObjectForList(Identity identity)
		{
			return base.GetObjectForList<ExtensionRow>("Get-App", identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Disable-App?Identity@W:Self")]
		public PowerShellResults<ExtensionRow> Disable(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<ExtensionRow>(new PSCommand().AddCommand("Disable-App"), identities, parameters);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Enable-App?Identity@W:Self")]
		public PowerShellResults<ExtensionRow> Enable(Identity[] identities, BaseWebServiceParameters parameters)
		{
			return base.InvokeAndGetObject<ExtensionRow>(new PSCommand().AddCommand("Enable-App"), identities, parameters);
		}

		internal const string ReadScope = "@R:Self";

		internal const string WriteScope = "@W:Self";

		internal const string GetExtension = "Get-App";

		internal const string RemoveExtension = "Remove-App";

		internal const string DisableExtension = "Disable-App";

		internal const string EnableExtension = "Enable-App";

		internal const string NewExtension = "New-App";

		internal const string GetListRole = "Get-App@R:Self";

		internal const string RemoveObjectsRole = "Remove-App?Identity@W:Self";

		internal const string GetObjectRole = "Get-App?Identity@R:Self";

		internal const string DisableExtensionRole = "Disable-App?Identity@W:Self";

		internal const string EnableExtensionRole = "Enable-App?Identity@W:Self";
	}
}
