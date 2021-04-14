using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RecipientPicker : RecipientPickerBase<RecipientPickerFilter, RecipientPickerObject>, IRecipientPicker, IGetListService<RecipientPickerFilter, RecipientPickerObject>
	{
	}
}
