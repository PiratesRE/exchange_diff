using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "AdminPicker")]
	public interface IAdminPicker : IGetListService<AdminPickerFilter, RecipientPickerObject>
	{
	}
}
