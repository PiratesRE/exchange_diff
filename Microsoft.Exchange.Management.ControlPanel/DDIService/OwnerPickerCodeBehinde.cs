using System;
using System.Data;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class OwnerPickerCodeBehinde : DDICodeBehind
	{
		public static void GetSecurityGroupFilterPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string text = (string)dataRow["DisplayName"];
				if (DBNull.Value.Equals(text) || string.IsNullOrEmpty(text))
				{
					dataRow["DisplayName"] = dataRow["Name"];
				}
			}
			dataTable.EndLoadData();
		}
	}
}
