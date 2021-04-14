using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class OwnerPicker : RecipientPickerBase<OwnerPickerFilter, RecipientPickerObject>, IOwnerPicker, IGetListService<OwnerPickerFilter, RecipientPickerObject>
	{
	}
}
