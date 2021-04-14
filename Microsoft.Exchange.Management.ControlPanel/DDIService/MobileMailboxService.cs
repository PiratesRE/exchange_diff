using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class MobileMailboxService
	{
		public static void AddToAllowList(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)dataTable.Rows[0]["ActiveSyncAllowedDeviceIDs"];
			multiValuedProperty.Add((string)store.GetValue("MobileDevice", "DeviceId"));
		}

		public static void AddToBlockList(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)dataTable.Rows[0]["ActiveSyncBlockedDeviceIDs"];
			multiValuedProperty.Add((string)store.GetValue("MobileDevice", "DeviceId"));
		}
	}
}
