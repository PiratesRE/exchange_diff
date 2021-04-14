using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailUserAndMailboxPicker")]
	public interface IMailUserAndMailboxPicker : IGetListService<MailUserAndMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
