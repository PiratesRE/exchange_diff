using System;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MobileDeviceService
	{
		public static void GetListRawResultAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			PowerShellResults<PSObject> powerShellResults = (PowerShellResults<PSObject>)store.GetDataObject("CASMailbox");
			if (powerShellResults != null && powerShellResults.Output.Length == 1)
			{
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					dataRow["IsLoggingRunning"] = powerShellResults.Output[0].Properties["ActiveSyncDebugLogging"].Value;
				}
			}
		}
	}
}
