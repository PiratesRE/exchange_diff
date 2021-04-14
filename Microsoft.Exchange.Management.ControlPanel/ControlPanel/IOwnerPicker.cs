using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "OwnerPicker")]
	public interface IOwnerPicker : IGetListService<OwnerPickerFilter, RecipientPickerObject>
	{
	}
}
