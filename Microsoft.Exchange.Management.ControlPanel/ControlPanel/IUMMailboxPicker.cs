using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMMailboxPicker")]
	public interface IUMMailboxPicker : IGetListService<UMMailboxPickerFilter, UMMailboxPickerObject>
	{
	}
}
