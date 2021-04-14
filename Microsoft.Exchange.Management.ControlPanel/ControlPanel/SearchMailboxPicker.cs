using System;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SearchMailboxPicker : RecipientPickerBase<SearchMailboxPickerFilter, RecipientPickerObject>, ISearchMailboxPicker, IGetListService<SearchMailboxPickerFilter, RecipientPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter")]
		public new PowerShellResults<RecipientPickerObject> GetList(SearchMailboxPickerFilter filter, SortOptions sort)
		{
			return base.GetList(filter, sort);
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter";
	}
}
