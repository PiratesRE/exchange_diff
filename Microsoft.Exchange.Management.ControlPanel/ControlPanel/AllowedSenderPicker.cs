using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AllowedSenderPicker : RecipientPickerBase<PersonOrGroupPickerFilter, RecipientPickerObject>, IAllowedSenderPicker, IGetListService<PersonOrGroupPickerFilter, RecipientPickerObject>
	{
	}
}
