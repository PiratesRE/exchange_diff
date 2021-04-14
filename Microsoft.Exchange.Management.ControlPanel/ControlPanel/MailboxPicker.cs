using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class MailboxPicker : RecipientPickerBase<MailboxPickerFilter, RecipientPickerObject>, IMailboxPicker, IGetListService<MailboxPickerFilter, RecipientPickerObject>
	{
	}
}
