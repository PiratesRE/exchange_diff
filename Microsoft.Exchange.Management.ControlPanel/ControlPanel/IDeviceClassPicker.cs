using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DeviceClassPicker")]
	public interface IDeviceClassPicker : IGetListService<DeviceClassPickerFilter, DeviceClassPickerObject>
	{
	}
}
