using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "PreviewPicker")]
	public interface IPreviewPicker : IGetListService<PreviewPickerFilter, RecipientPickerObject>
	{
	}
}
