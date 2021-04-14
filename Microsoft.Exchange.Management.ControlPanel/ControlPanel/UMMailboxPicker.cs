using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class UMMailboxPicker : DataSourceService, IUMMailboxPicker, IGetListService<UMMailboxPickerFilter, UMMailboxPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties")]
		public PowerShellResults<UMMailboxPickerObject> GetList(UMMailboxPickerFilter filter, SortOptions sort)
		{
			return base.GetList<UMMailboxPickerObject, UMMailboxPickerFilter>("Get-Recipient", filter, sort, "DisplayName");
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties";
	}
}
