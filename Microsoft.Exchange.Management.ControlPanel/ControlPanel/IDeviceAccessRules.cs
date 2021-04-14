using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DeviceAccessRules")]
	public interface IDeviceAccessRules : IDataSourceService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, SetDeviceAccessRule, NewDeviceAccessRule>, IDataSourceService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, SetDeviceAccessRule, NewDeviceAccessRule, BaseWebServiceParameters>, IEditListService<DeviceAccessRuleFilter, DeviceAccessRuleRow, DeviceAccessRule, NewDeviceAccessRule, BaseWebServiceParameters>, IGetListService<DeviceAccessRuleFilter, DeviceAccessRuleRow>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<DeviceAccessRule, SetDeviceAccessRule, DeviceAccessRuleRow>, IGetObjectService<DeviceAccessRule>, IGetObjectForListService<DeviceAccessRuleRow>, INewGetObjectService<DeviceAccessRuleRow, NewDeviceAccessRule, NewDeviceAccessRuleData>, INewObjectService<DeviceAccessRuleRow, NewDeviceAccessRule>
	{
	}
}
