using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ExtendedMailboxPicker : RecipientPickerBase<ExtendedMailboxPickerFilter, RecipientPickerObject>, IExtendedMailboxPicker, IGetListService<ExtendedMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
