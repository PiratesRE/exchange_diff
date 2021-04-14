using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DeviceClassPicker : DataSourceService, IDeviceClassPicker, IGetListService<DeviceClassPickerFilter, DeviceClassPickerObject>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-ActiveSyncDeviceClass@C:OrganizationConfig")]
		public PowerShellResults<DeviceClassPickerObject> GetList(DeviceClassPickerFilter filter, SortOptions sort)
		{
			if (!string.IsNullOrEmpty(filter.DeviceType))
			{
				filter.Filter = string.Format("DeviceType -eq '{0}'", filter.DeviceType);
			}
			PowerShellResults<DeviceClassPickerObject> list = base.GetList<DeviceClassPickerObject, DeviceClassPickerFilter>("Get-ActiveSyncDeviceClass", filter, sort);
			if (filter.GroupDeviceType)
			{
				List<DeviceClassPickerObject> list2 = new List<DeviceClassPickerObject>();
				list2.Add(new DeviceClassPickerObject(DeviceClassPickerObject.AllDeviceTypeQueryString, DeviceClassPickerObject.AllDeviceModelQueryString));
				HashSet<string> hashSet = new HashSet<string>();
				foreach (DeviceClassPickerObject deviceClassPickerObject in list.Output)
				{
					if (!hashSet.Contains(deviceClassPickerObject.DeviceType.QueryString))
					{
						list2.Add(deviceClassPickerObject);
						hashSet.Add(deviceClassPickerObject.DeviceType.QueryString);
					}
				}
				list.Output = list2.ToArray();
			}
			else if (!string.IsNullOrEmpty(filter.DeviceType))
			{
				List<DeviceClassPickerObject> list3 = new List<DeviceClassPickerObject>();
				list3.Add(new DeviceClassPickerObject(new DeviceAccessRuleQueryString
				{
					QueryString = filter.DeviceType
				}, DeviceClassPickerObject.AllDeviceModelQueryString));
				list3.AddRange(list.Output);
				list.Output = list3.ToArray();
			}
			return list;
		}

		internal const string GetCmdlet = "Get-ActiveSyncDeviceClass";

		internal const string ReadScope = "@C:OrganizationConfig";

		private const string GetListRole = "Get-ActiveSyncDeviceClass@C:OrganizationConfig";
	}
}
