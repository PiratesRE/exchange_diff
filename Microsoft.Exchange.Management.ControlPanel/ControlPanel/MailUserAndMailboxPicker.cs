using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class MailUserAndMailboxPicker : RecipientPickerBase<MailUserAndMailboxPickerFilter, RecipientPickerObject>, IMailUserAndMailboxPicker, IGetListService<MailUserAndMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
