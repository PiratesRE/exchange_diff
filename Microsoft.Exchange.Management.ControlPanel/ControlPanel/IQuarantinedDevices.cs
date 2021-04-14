using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "QuarantinedDevices")]
	public interface IQuarantinedDevices : IGetListService<QuarantinedDeviceFilter, QuarantinedDevice>
	{
		[OperationContract]
		PowerShellResults AllowDevice(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults BlockDevice(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
