using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SourceMailboxPicker")]
	public interface ISourceMailboxPicker : IGetListService<SourceMailboxPickerFilter, RecipientPickerObject>
	{
	}
}
