using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ModeratorPicker : RecipientPickerBase<ModeratorPickerFilter, RecipientPickerObject>, IModeratorPicker, IGetListService<ModeratorPickerFilter, RecipientPickerObject>
	{
	}
}
