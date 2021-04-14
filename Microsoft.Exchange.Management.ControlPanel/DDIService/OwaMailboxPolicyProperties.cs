using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class OwaMailboxPolicyProperties
	{
		public static void GetDefaultPolicyPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				if (false.Equals(dataRow["IsDefault"]))
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}
	}
}
