using System;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PreviewPicker : RecipientPickerBase<PreviewPickerFilter, RecipientPickerObject>, IPreviewPicker, IGetListService<PreviewPickerFilter, RecipientPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter&OrganizationalUnit")]
		public new PowerShellResults<RecipientPickerObject> GetList(PreviewPickerFilter filter, SortOptions sort)
		{
			if (filter.HasCondition)
			{
				return base.GetList(filter, sort);
			}
			return new PowerShellResults<RecipientPickerObject>();
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter&OrganizationalUnit";
	}
}
