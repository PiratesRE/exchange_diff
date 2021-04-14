using System;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AdminPicker : RecipientPickerBase<AdminPickerFilter, RecipientPickerObject>, IAdminPicker, IGetListService<AdminPickerFilter, RecipientPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties")]
		public new PowerShellResults<RecipientPickerObject> GetList(AdminPickerFilter filter, SortOptions sort)
		{
			return base.GetList(filter, sort);
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties";
	}
}
