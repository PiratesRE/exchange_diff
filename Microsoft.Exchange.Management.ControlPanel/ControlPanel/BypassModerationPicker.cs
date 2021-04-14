using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class BypassModerationPicker : RecipientPickerBase<PersonOrGroupPickerFilter, RecipientPickerObject>, IBypassModerationPicker, IGetListService<PersonOrGroupPickerFilter, RecipientPickerObject>
	{
	}
}
