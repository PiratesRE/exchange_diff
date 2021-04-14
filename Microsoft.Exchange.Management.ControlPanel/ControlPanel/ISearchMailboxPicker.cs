using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SearchMailboxPicker")]
	public interface ISearchMailboxPicker : IGetListService<SearchMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
