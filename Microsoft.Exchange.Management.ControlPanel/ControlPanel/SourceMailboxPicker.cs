using System;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SourceMailboxPicker : RecipientPickerBase<SourceMailboxPickerFilter, RecipientPickerObject>, ISourceMailboxPicker, IGetListService<SourceMailboxPickerFilter, RecipientPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter")]
		public new PowerShellResults<RecipientPickerObject> GetList(SourceMailboxPickerFilter filter, SortOptions sort)
		{
			return base.GetList(filter, sort);
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter";
	}
}
