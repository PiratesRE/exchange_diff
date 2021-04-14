using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ModeratorPicker")]
	public interface IModeratorPicker : IGetListService<ModeratorPickerFilter, RecipientPickerObject>
	{
	}
}
