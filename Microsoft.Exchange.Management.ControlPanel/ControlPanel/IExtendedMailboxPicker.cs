using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ExtendedMailboxPicker")]
	public interface IExtendedMailboxPicker : IGetListService<ExtendedMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
