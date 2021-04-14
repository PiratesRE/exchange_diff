using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "AllowedSenderPicker")]
	public interface IAllowedSenderPicker : IGetListService<PersonOrGroupPickerFilter, RecipientPickerObject>
	{
	}
}
