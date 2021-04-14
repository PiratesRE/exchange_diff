using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DiscoveryMailboxPicker")]
	public interface IDiscoveryMailboxPicker : IGetListService<DiscoveryMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
