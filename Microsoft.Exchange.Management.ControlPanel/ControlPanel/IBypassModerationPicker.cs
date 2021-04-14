using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "BypassModerationPicker")]
	public interface IBypassModerationPicker : IGetListService<PersonOrGroupPickerFilter, RecipientPickerObject>
	{
	}
}
