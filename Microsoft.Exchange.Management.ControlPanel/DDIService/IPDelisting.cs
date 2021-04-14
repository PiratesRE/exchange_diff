using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class IPDelisting
	{
		public static void CheckDateAdded(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				DateTime dateTime = Convert.ToDateTime(dataRow["DateAdded"].ToString());
				DateTime utcNow = DateTime.UtcNow;
				int days = (utcNow - dateTime).Days;
				dataRow["DaysSinceLastDelist"] = days;
				dataRow["DateAdded"] = string.Format("{0:d}", dateTime);
			}
		}
	}
}
