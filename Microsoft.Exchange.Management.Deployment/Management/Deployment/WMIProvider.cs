using System;
using System.Collections.Generic;
using System.Management;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class WMIProvider : IWMIDataProvider
	{
		public Dictionary<string, object>[] Run(string wmiQuery)
		{
			if (string.IsNullOrEmpty(wmiQuery))
			{
				throw new ArgumentNullException("wmiQuery");
			}
			WqlObjectQuery query = new WqlObjectQuery(wmiQuery.Replace("\\", "\\\\"));
			Dictionary<string, object>[] result;
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
			{
				if (managementObjectSearcher.Get().Count == 0)
				{
					result = new Dictionary<string, object>[]
					{
						new Dictionary<string, object>()
					};
				}
				else
				{
					Dictionary<string, object>[] array = new Dictionary<string, object>[managementObjectSearcher.Get().Count];
					int num = 0;
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						array[num] = new Dictionary<string, object>();
						foreach (PropertyData propertyData in managementObject.Properties)
						{
							if (!array[num].ContainsKey(propertyData.Name))
							{
								array[num].Add(propertyData.Name, propertyData.Value);
							}
						}
						num++;
					}
					result = array;
				}
			}
			return result;
		}
	}
}
