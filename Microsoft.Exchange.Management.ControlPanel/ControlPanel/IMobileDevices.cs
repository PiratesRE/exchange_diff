using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MobileDevices")]
	public interface IMobileDevices : IGetListService<MobileDeviceFilter, MobileDeviceRow>, IGetObjectService<MobileDevice>, IGetObjectForListService<MobileDeviceRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[OperationContract]
		PowerShellResults<MobileDeviceRow> BlockOrWipeDevice(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<MobileDeviceRow> UnBlockOrCancelWipeDevice(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults StartLogging(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults StopAndRetrieveLog(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
