using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DiscoveryMailboxPicker : RecipientPickerBase<DiscoveryMailboxPickerFilter, RecipientPickerObject>, IDiscoveryMailboxPicker, IGetListService<DiscoveryMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
